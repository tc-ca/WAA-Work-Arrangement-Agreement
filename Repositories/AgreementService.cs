using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class AgreementService
    {
        private readonly DomainContext _context;

        public AgreementService(DomainContext context)
        {
            _context = context;
        }
        public Agreement GetAcrchivedByUsername(string username)
        {
            return _context.Agreements.FirstOrDefault(x => x.TcUserId.ToLower() == username.ToLower() && x.ArchivedInd);
        }
        public Agreement GetActiveByUsername(string username)
        {
            return _context.Agreements.Where(x => x.TcUserId.ToLower() == username.ToLower() && !x.ArchivedInd && (x.StatusCode == "6" || x.StatusCode == "4")).OrderBy(x => x.StartDate).FirstOrDefault();
        }
        public List<Agreement> GetAgreementByUsername(string username)
        {
            //return approved by default; if approved status =6 (renew) then return draft
            var agmt = _context.Agreements
                .Include(x => x.WorkType)
                .Include(x => x.TcUser)
                .Include(x => x.Recommender)
                .Include(x => x.Approver)
                .Include(x => x.ApproveRejectedBy)
                .Include(x => x.TcWorksite).ThenInclude(x => x.TcRegion)
                .Include(x => x.UnmetOHSItems).ThenInclude(x => x.OHSChecklist).ThenInclude(x => x.OHSCategory)
                .Include(x => x.AltWorkSites)
                .Include(x => x.DenyReason)
                .Include(x => x.HybridOption)
                .Where(x => x.TcUserId.ToLower() == username.ToLower() && !x.ArchivedInd)
                .OrderBy(x => x.StartDate).ToList();
            //foreach (var a in agmt)
            //{
            //    if (a.WorkTypeId == "2" && a.HybridOptionId.Value == 5)
            //    {
            //        a.SupportDocumentInfo = GetSupportDocInfoByAgreementId(a.AgreementId);
            //    }
            //}
           return agmt; 

        }
        public Agreement GetAgreementById(int id)
        {
            var deletedOhsItemIds = _context.OHSChecklists.Where(x => x.DeleteDate != null).Select(x => x.CheckListId).ToList();
            var agmt = _context.Agreements
                .Include(x => x.WorkType)
                .Include(x => x.TcUser)
                .Include(x => x.Recommender)
                .Include(x => x.Approver)
                .Include(x => x.TcWorksite).ThenInclude(x => x.TcRegion)
                .Include(x => x.DenyReason)
                .Include(x => x.UnmetOHSItems.Where(u => !deletedOhsItemIds.Contains(u.UnMetOHSItemId))).ThenInclude(x => x.OHSChecklist).ThenInclude(x => x.OHSCategory)
                .Include(x => x.AltWorkSites)
                .Where(x => x.AgreementId == id)
                .FirstOrDefault();
            return agmt;
        }
        public async Task<List<OHSCategory>> GetOHSCategoryList()
        {
            return await _context.OHSCategories.Include("OHSChecklist").ToListAsync();
        }
        public List<ListItem> GetOHSChecklist()
        {
            var list = _context.OHSChecklists.AsNoTracking().Where(x => x.DeleteDate == null)
                                                                                    .Select(x => new ListItem() { Id = x.CheckListId, Value = "" })
                                                                                    .OrderBy(x => x.Id).ToList();
            return list;
          }
        public async Task<List<Agreement>> GetMyEmpsAgreements(List<DirectReportsModel> directReports, string manager)
        {
           var lst = await _context.Agreements.AsNoTracking()
                 .Include(x => x.WorkType)
                .Include(x => x.TcUser)
                .Include(x => x.Recommender)
                .Include(x => x.Approver)
                .Include(x=>x.ApproveRejectedBy)
                .Include(x=>x.HybridOption)
                .Include(x=>x.UnmetOHSItems)
                 .Include(x => x.TcWorksite)
                .Where(x => directReports.Select(x => x.UserName.ToLower()).Contains(x.TcUserId.ToLower()) || x.RecommenderId.ToLower() == manager.ToLower())
                .Where(x => x.ArchivedInd == false)
                .ToListAsync();
            //remove duplicated
            List<Agreement> ret = new List<Agreement>();
            foreach (Agreement a in lst)
            {
                if (!ret.Any(x => x.AgreementId == a.AgreementId))
                    ret.Add(a);
            }
            return ret;
        }

        public bool CreateAgreement(Agreement posted, string modifiedBy)
        {
            posted.StatusCode = "2";
            posted.LastUpdateByUserId = modifiedBy;
            posted.LastUpdateDate = DateTime.Now;
            posted.SubmittedDate = DateTime.Now;
            posted.TeleworkAddrPostal = posted.TeleworkAddrPostal?.ToUpper();
            posted.TeleworkAddrProvince = posted.TeleworkAddrProvince?.ToUpper();
            posted.TeleworkAddrCity = ToTitleCase(posted.TeleworkAddrCity);
            posted.TeleworkAddrStreet = ToTitleCase(posted.TeleworkAddrStreet);

            _context.Agreements.Add(posted);
            if (posted.UnmetOHSItems != null)
            {
                _context.UserUnmetOHSItems.AddRange(posted.UnmetOHSItems);
            }
            if (posted.AltWorkSites != null && posted.AltWorkSites.Count>0)
            {
                _context.AltWorkSites.AddRange(posted.AltWorkSites);
            }

            return _context.SaveChanges()>0;

            
        }
        public bool UpdateAgreement(Agreement posted, string modifiedBy)
        {
            Agreement agreement = _context.Agreements.Find(posted.AgreementId);
            if (agreement != null)
            {
                var approver = _context.UserManagers.Find(agreement.TcUserId)?.ManagerId;
                agreement.StatusCode = "2";
                agreement.LastUpdateByUserId = modifiedBy;
                agreement.LastUpdateDate = DateTime.Now;
                agreement.ApproverId = approver;
                agreement.RecommenderId = null;
                agreement.ObligationInd = true;
                agreement.ConditionInd = true;
                agreement.MutualAgreementInd = true;
                agreement.WorkTypeId = posted.WorkTypeId;
                agreement.SubmittedDate = DateTime.Now;
                agreement.StartDate = posted.StartDate;
                agreement.EndDate = posted.EndDate;
                agreement.Comments = null;
                agreement.ApprovedRejectedById = null;
                agreement.ApprovedRejectedDate = null;
                agreement.RenewNotified = null;
                agreement.ArchivedInd = false;
                agreement.TeleworkAddrPostal = posted.TeleworkAddrPostal?.ToUpper();
                agreement.TeleworkAddrProvince = posted.TeleworkAddrProvince?.ToUpper();
                agreement.TeleworkAddrCity = ToTitleCase(posted.TeleworkAddrCity);
                agreement.TeleworkAddrStreet = ToTitleCase(posted.TeleworkAddrStreet);

                agreement.EmergencyContactName = posted.EmergencyContactName;
                agreement.EmergencyContactPhone = posted.EmergencyContactPhone;
                agreement.TcWorksiteId = posted.TcWorksiteId;
                agreement.EmergencyContactPhone = posted.EmergencyContactPhone;
                agreement.IsAccommodateDuty = posted.IsAccommodateDuty;
                agreement.IsVirtual = posted.IsVirtual;
                agreement.VariableScheduleDetails = posted.VariableScheduleDetails;
                agreement.HybridOptionId = posted.HybridOptionId; //mockup
                _context.Agreements.Update(agreement);

                var currentList = _context.UserUnmetOHSItems.Where(x => x.AgreementId == agreement.AgreementId);
                _context.UserUnmetOHSItems.RemoveRange(currentList);
                if (posted.UnmetOHSItems != null)
                {
                    _context.UserUnmetOHSItems.AddRange(posted.UnmetOHSItems);
                }
                var currentAltWkList = _context.AltWorkSites.Where(x => x.AgreementId == agreement.AgreementId);
                _context.AltWorkSites.RemoveRange(currentAltWkList);
                if (posted.AltWorkSites != null && posted.AltWorkSites.Count > 0)
                {
                    _context.AltWorkSites.AddRange(posted.AltWorkSites);
                }
                if (agreement.HybridOptionId != 5) //clean FTR table if changed from FTR
                {
                    var hist = _context.FTRRecommenders.FirstOrDefault(x => x.AgreementId == agreement.AgreementId);
                    if (hist != null) _context.FTRRecommenders.Remove(hist);
                }
            }
            return _context.SaveChanges() > 0;
        }
        // reopen condition:
        // 1. open by EE after denied -- 0
        // 2. open by manager for renew -- 1
        // 3. open by manager for update -- 2
        public bool Reopen(int agreementID, string comments, string modifiedBy)
        {
            int reason = 2;
            Agreement agreement = _context.Agreements.FirstOrDefault(x => x.AgreementId == agreementID);           
            
            if (agreement != null)
            { 
                var all_agreements = _context.Agreements.Where(x => x.TcUserId == agreement.TcUserId).ToList();
                //EE reopen
                if (agreement.StatusCode == "5") { 
                    reason = 0; 
                } else if (agreement.StatusCode == "4") {
                    //return for update
                    var another_approved_agreement = all_agreements.FirstOrDefault(x => x.AgreementId != agreementID && x.StatusCode == "4" && x.EndDate >= DateTime.Today);
                    if (another_approved_agreement!=null)
                    {
                        reason = 2;
                        another_approved_agreement.StatusCode = "6";
                    }
                    else
                    {
                        reason = 1;
                    }
                }
                if (reason == 1)
                {
                    agreement.StatusCode = "6";
                    //agreement.Comments = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "en" ? "Renewal required" : "Renouvellement requis";
                    agreement.RenewNotified = 2;
                }
                else {

                    if (reason == 2)
                    {
                        //string newComments = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "en" ? "Returned for Updating" : "Renvoyé pour être mis à jour";
                        agreement.Comments =  comments;

                    }
                    agreement.StatusCode = "1";
                    agreement.SubmittedDate = null;
                    agreement.LastUpdateByUserId = modifiedBy;
                    agreement.LastUpdateDate = DateTime.Now;
                    agreement.RecommenderId = null;
                    agreement.ObligationInd = false;
                    agreement.ConditionInd = false;
                    agreement.MutualAgreementInd = false;
                    agreement.ApprovedRejectedDate = null;
                    agreement.ApprovedRejectedById = null;
                    agreement.DenyReasonCd = null;
                }

                //_context.Update(agreement);                
            }
            return _context.SaveChanges() > 0;
        }
        public int Renew(int agreementID, string modifiedBy)
        {
            //var myAgreements = _context.Agreements.Where(x => x.TcUserId == modifiedBy && !x.ArchivedInd).ToList();
            //if (myAgreements.Count == 0)
            //{
            //    return -1;
            //}
            Agreement active_agreement = _context.Agreements.FirstOrDefault(x => x.AgreementId == agreementID);
            Agreement draft_agreement = _context.Agreements.FirstOrDefault(x => x.AgreementId != agreementID && x.TcUserId == modifiedBy);         
            
            if (draft_agreement == null)
            {
                //create draft if not exist
                draft_agreement = new Agreement();
            }
            

            //copy to draft

            draft_agreement.TcUserId = modifiedBy;
            draft_agreement.StatusCode = "1";
            draft_agreement.LastUpdateByUserId = modifiedBy;
            draft_agreement.LastUpdateDate = DateTime.Now;
            draft_agreement.SubmittedDate = null;
            draft_agreement.DeleteDate = null;
            draft_agreement.StartDate = DateTime.MinValue;
            draft_agreement.EndDate = DateTime.MinValue;
            draft_agreement.ApprovedRejectedDate = null;
            draft_agreement.ApprovedRejectedById = null;
            draft_agreement.RecommenderId = null;
            draft_agreement.ObligationInd = false;
            draft_agreement.ConditionInd = false;
            draft_agreement.MutualAgreementInd= false;
            draft_agreement.WorkTypeId = active_agreement.WorkTypeId;
            draft_agreement.TeleworkAddrPostal = active_agreement.TeleworkAddrPostal?.ToUpper();
            draft_agreement.TeleworkAddrProvince = active_agreement.TeleworkAddrProvince?.ToUpper();
            draft_agreement.TeleworkAddrCity = active_agreement.TeleworkAddrCity;
            draft_agreement.TeleworkAddrStreet = active_agreement.TeleworkAddrStreet;
            draft_agreement.IsVirtual = active_agreement.IsVirtual;
            draft_agreement.VariableScheduleDetails = active_agreement.VariableScheduleDetails;
            draft_agreement.HybridOptionId = active_agreement.HybridOptionId;
            draft_agreement.TcWorksiteId = active_agreement.TcWorksiteId;
            draft_agreement.IsAccommodateDuty = active_agreement.IsAccommodateDuty;
            draft_agreement.ApproverId = active_agreement.ApproverId;//              
            draft_agreement.EmergencyContactName = null;
            draft_agreement.EmergencyContactPhone = null;
            draft_agreement.Comments = null;
            draft_agreement.DenyReasonCd = null;
            draft_agreement.RenewNotified = 0;
            draft_agreement.ArchivedInd = false;

            if (active_agreement.RenewNotified!=null && active_agreement.RenewNotified ==2) //notified by manager returned
            {
                draft_agreement.StartDate = active_agreement.StartDate;
                draft_agreement.EndDate = active_agreement.EndDate;
            }
            if (draft_agreement.AgreementId == 0) //new added
            {
                _context.Agreements.Add(draft_agreement);
            }
            else
            {
                _context.Agreements.Update(draft_agreement);
            }
            _context.SaveChanges();

            var draftOhsList = _context.UserUnmetOHSItems.Where(x => x.AgreementId == draft_agreement.AgreementId);
            _context.UserUnmetOHSItems.RemoveRange(draftOhsList);
            var approvedOhstList = _context.UserUnmetOHSItems.Where(x => x.AgreementId == active_agreement.AgreementId).ToList();
            if (approvedOhstList != null && approvedOhstList.Count() > 0)
            {
                var newList = new List<UserUnmetOHSItem>();
                approvedOhstList.ForEach(x => {
                    newList.Add(new UserUnmetOHSItem() { AgreementId = draft_agreement.AgreementId, UnMetOHSItemId = x.UnMetOHSItemId, LastUpdateByUserId = modifiedBy });
                });
                _context.UserUnmetOHSItems.AddRange(newList);
            }
            var currentAltWkList = _context.AltWorkSites.Where(x => x.AgreementId == draft_agreement.AgreementId);
            _context.AltWorkSites.RemoveRange(currentAltWkList);
            
            var approvedAltWkList = _context.AltWorkSites.Where(x => x.AgreementId == active_agreement.AgreementId).ToList();
            if (approvedAltWkList != null && approvedAltWkList.Count() > 0)
            {   
                foreach (var site in approvedAltWkList)
                {
                    site.AgreementId = draft_agreement.AgreementId;
                    site.LastUpdateByUserId = modifiedBy;
                }
                _context.AltWorkSites.AddRange(approvedAltWkList);
            }

            _context.SaveChanges();

            return draft_agreement.AgreementId;
        }
        public bool Approve(int agreementID, string approvedBy, string comments, string modifiedBy)
        {            
            Agreement agreement = _context.Agreements.FirstOrDefault(x => x.AgreementId == agreementID);
            if (agreement != null)
            {
                agreement.StatusCode = "4";
                agreement.LastUpdateByUserId = modifiedBy;
                agreement.LastUpdateDate = DateTime.Now;

                agreement.ApprovedRejectedDate = DateTime.Now;
                agreement.ApprovedRejectedById = approvedBy;
                agreement.Comments = comments;

                var old_agreements = _context.Agreements.FirstOrDefault(x => x.TcUserId == agreement.TcUserId && x.AgreementId != agreementID && x.StatusCode =="6");
                if (old_agreements != null)
                {
                    if (agreement.StartDate <= DateTime.Today) //new version started
                    {
                        old_agreements.StatusCode = "1";
                        old_agreements.ArchivedInd = true;
                        old_agreements.SubmittedDate = null;
                        old_agreements.ApprovedRejectedDate = null;
                        old_agreements.ApprovedRejectedById = null;
                        old_agreements.RecommenderId = null;
                        old_agreements.RenewNotified = 0;
                        if (old_agreements.HybridOptionId == 5)
                        {
                            var hist = _context.FTRRecommenders.FirstOrDefault(x => x.AgreementId == old_agreements.AgreementId);
                            if (hist != null) _context.FTRRecommenders.Remove(hist);
                        }
                    }
                    else
                    {
                        old_agreements.StatusCode = "4";

                        //old agreement expires when new one starts -- if overlapped
                        if (old_agreements.EndDate >= agreement.StartDate)
                        {
                            old_agreements.EndDate = agreement.StartDate.AddDays(-1);
                        }
                    }
                   
                    _context.Agreements.Update(old_agreements);
                }
                
                _context.Agreements.Update(agreement);
            }

           return _context.SaveChanges() > 0;
        }

        public bool Deny(int agreementID, string denyReasonCd, string deniedBy, string comments, string modifiedBy)
        {
            Agreement agreement = _context.Agreements.Find(agreementID);
            if (agreement != null)
            {
                agreement.StatusCode = "5";
                agreement.LastUpdateByUserId = modifiedBy;
                agreement.LastUpdateDate = DateTime.Now;

                agreement.ApprovedRejectedDate = DateTime.Now;
                agreement.ApprovedRejectedById = deniedBy;
                agreement.Comments = comments;
                agreement.DenyReasonCd = denyReasonCd;

                _context.Update(agreement);               
            }

            return _context.SaveChanges() > 0;
        }

        public bool Recommend(int agreementID, string recommendedToUsername, string comments, string modifiedBy)
        {
            Agreement agreement = _context.Agreements.Find(agreementID);
            if (agreement != null)
            {
                agreement.StatusCode = "3";
                agreement.LastUpdateByUserId = modifiedBy;
                agreement.LastUpdateDate = DateTime.Now;

                agreement.RecommenderId = recommendedToUsername;
                agreement.Comments = comments;
                if (agreement.HybridOptionId.HasValue && agreement.HybridOptionId ==5)
                {
                    FTRRecommender ftr = _context.FTRRecommenders.Find(agreementID);
                    if (ftr != null)
                    {
                        string[] rs = { ftr.RecommerLevel1, ftr.RecommerLevel2, ftr.RecommerLevel3, ftr.RecommerLevel4 };
                        int index = Array.IndexOf(rs, modifiedBy);
                        switch (index) {
                            case 0:
                                ftr.RecommerLevel2 = recommendedToUsername;
                                break;
                            case 1:
                                ftr.RecommerLevel3 = recommendedToUsername;
                                break;
                            case 2:
                                ftr.RecommerLevel4 = recommendedToUsername;
                                break;
                            case 3:
                                throw new Exception("TMX approver is the final recommender");
                            default:
                                break;
                        }
                        _context.Update(ftr);
                    }
                    else
                    {
                        ftr = new FTRRecommender() { AgreementId = agreementID, RecommerLevel1 = recommendedToUsername };
                        _context.Add(ftr);
                    }
                }
                _context.Update(agreement);
            }
            

            return _context.SaveChanges()>0;
        }
        //FTR agreements
        public bool ReturnToRecommender(int agreementID, string returndToUsername, string comments, string modifiedBy)
        {
            Agreement agreement = _context.Agreements.Find(agreementID);
            agreement.LastUpdateByUserId = modifiedBy;
            agreement.LastUpdateDate = DateTime.Now;
            agreement.RecommenderId = returndToUsername;
            agreement.Comments = comments;
            if (agreement != null && agreement.HybridOptionId.HasValue && agreement.HybridOptionId.Value == 5)
            {   
                FTRRecommender ftr = _context.FTRRecommenders.Find(agreementID);
                if (ftr != null)
                {
                    string[] rs = { ftr.RecommerLevel1, ftr.RecommerLevel2, ftr.RecommerLevel3, ftr.RecommerLevel4 };
                    int index = Array.IndexOf(rs, modifiedBy);
                    switch (index)
                    {
                        case 0:
                            _context.FTRRecommenders.Remove(ftr);
                            agreement.StatusCode = "2";
                            agreement.RecommenderId = null;
                            break;
                        case 1:
                            ftr.RecommerLevel1 = returndToUsername;
                            ftr.RecommerLevel2 = null;
                            _context.FTRRecommenders.Update(ftr);
                            break;
                        case 2:
                            ftr.RecommerLevel2 = returndToUsername;
                            ftr.RecommerLevel3 = null;
                            _context.FTRRecommenders.Update(ftr);
                            break;
                        case 3:
                            ftr.RecommerLevel3 = returndToUsername;
                            ftr.RecommerLevel4 = null;
                            _context.FTRRecommenders.Update(ftr);
                            break;
                        default:
                            break;
                    }
                }
                else if (agreement.ApproverId!=null && agreement.ApproverId == returndToUsername) //handle version 1.0 records
                {
                    agreement.StatusCode = "2";
                    agreement.RecommenderId = null;
                }               
            }
            _context.Agreements.Update(agreement);
            return _context.SaveChanges() > 0;
        }

        public void Archive(int id, string username)
        {
            Agreement agreement = _context.Agreements.Find(id);

            agreement.ArchivedInd = true;
            agreement.LastUpdateDate = DateTime.Now;
            agreement.LastUpdateByUserId = username;

            _context.SaveChanges();
        }
        public List<DenyReason> GetDenyReasonList()
        {
            return _context.DenyReasons.ToList();
        }
        private string ToTitleCase(string title)
        {
            if (string.IsNullOrEmpty(title)) 
                return string.Empty;
            else
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }

        // for admin module
        public List<AgreementInfo> GetAgreementInfoByUsername(string username)
        {
            var lan = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var agmts = _context.Agreements
                .Include(x => x.TcUser)
                .Include(x => x.Recommender)
                .Include(x => x.Approver)
                .Include(x => x.Status)
                .Where(x => x.TcUserId.ToLower() == username.ToLower() && !x.ArchivedInd && x.SubmittedDate !=null)
                .Select(x => new AgreementInfo
                 {
                     id = x.AgreementId,
                     employee = x.TcUser.FullName,
                     tcUserId = x.TcUserId,
                     status = lan == "en" ? x.Status.English : x.Status.French,
                     statusCode = x.StatusCode,
                     decisionDate = x.ApprovedRejectedDate,
                     approver = x.Approver.FullName,
                     approverId = x.ApproverId,
                     recommender = x.Recommender.FullName,

                     canReturn = (x.StatusCode == "4" && x.StartDate < DateTime.Today) ? 1 : 0
                 }).OrderByDescending(x => x.statusCode).ToList();

            if (agmts.Count == 1 && agmts[0].statusCode == "4") agmts[0].canReturn = 1;

            return agmts;
        }
        public List<AgreementInfo> UpdateAgreementStatus(int agreementId, string returnToCode, string updatedById)
        {
            var agmt = _context.Agreements.Find(agreementId);

            if (agmt != null)
            {
                if (returnToCode == "a")
                {
                    agmt.StatusCode = "2";
                    agmt.ApprovedRejectedDate = null;
                    agmt.ApprovedRejectedById = null;
                    agmt.RecommenderId = null;
                } 
                else if (returnToCode == "r")
                {
                    agmt.StatusCode = "3";
                    agmt.ApprovedRejectedDate = null;
                    agmt.ApprovedRejectedById = null;
                }
                else if (returnToCode == "e")
                {
                    agmt.StatusCode = "1";
                    agmt.ApprovedRejectedDate = null;
                    agmt.ApprovedRejectedById = null;
                    agmt.RecommenderId = null;
                }
                agmt.LastUpdateByUserId = updatedById;

            }
            _context.SaveChanges();

            return GetAgreementInfoByUsername(agmt.TcUserId);
        }

        //public async Task<int>  UploadDoc(SupportDocument doc)
        //{
        //    int docId = -1;
        //    if (doc != null)
        //    {
        //        var curDoc = await _context.SupportDocuments.FirstOrDefaultAsync(x => x.AgreementId == doc.AgreementId);
        //        if (curDoc != null)
        //        {
        //            curDoc.FileName = doc.FileName;
        //            curDoc.FileExtension = doc.FileExtension;
        //            curDoc.LastUpdateByUserId = doc.LastUpdateByUserId;
        //            curDoc.Content = doc.Content;
        //            curDoc.DeleteDate = null;
        //            _context.SupportDocuments.Update(curDoc);
        //            docId = curDoc.DocId;
        //            await _context.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            _context.SupportDocuments.Add(doc);                    
        //            await _context.SaveChangesAsync();
        //            docId = doc.DocId;
        //        } 
        //    }
        //    return docId;
        //}
        //public async Task<SupportDocument>  GetSupportDocById(int docId)
        //{

        //    return await _context.SupportDocuments.FindAsync(docId);
        //}
        //public SupportDocumentView GetSupportDocInfoByAgreementId(int agreementId)
        //{

        //    return  _context.SupportDocuments
        //                .Where(x=>x.AgreementId == agreementId)
        //                .Select(y=>new SupportDocumentView() { DocId =y.DocId, FileName = y.FileName})
        //                .FirstOrDefault();
        //}

    }
}

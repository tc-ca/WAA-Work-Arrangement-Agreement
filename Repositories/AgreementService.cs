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

        public Agreement GetAgreementByUsername(string username)
        {
            var agmt= _context.Agreements
                .Include(x => x.WorkType)
                .Include(x=>x.TcUser)
                .Include(x=>x.Recommender)
                .Include(x => x.Approver)
                .Include(x=>x.TcWorksite)
                .Include(x => x.UnmetOHSItems).ThenInclude(x => x.OHSChecklist).ThenInclude(x => x.OHSCategory)
                .Include(x => x.AltWorkSites)
                .Where(x => x.TcUserId.ToLower() == username.ToLower() && !x.ArchivedInd)
                .FirstOrDefault();
            return agmt;
        }
        public Agreement GetAgreementById(int id)
        {
            var agmt = _context.Agreements
                .Include(x => x.WorkType)
                .Include(x => x.TcUser)
                .Include(x => x.Recommender)
                .Include(x => x.Approver)
                .Include(x => x.TcWorksite)
                .Include(x => x.DenyReason)
                .Include(x => x.UnmetOHSItems).ThenInclude(x=>x.OHSChecklist).ThenInclude(x=>x.OHSCategory)               
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
            return await _context.Agreements.AsNoTracking()
                 .Include(x => x.WorkType)
                .Include(x => x.TcUser)
                .Include(x => x.Recommender)
                .Include(x => x.Approver)
                .Include(x=>x.ApproveRejectedBy)
                .Where(x => directReports.Select(x => x.UserName.ToLower()).Contains(x.TcUserId.ToLower()) || x.RecommenderId.ToLower() == manager.ToLower())
                .Where(x => x.ArchivedInd == false)
                .ToListAsync();
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
                agreement.StatusCode = "2";
                agreement.LastUpdateByUserId = modifiedBy;
                agreement.LastUpdateDate = DateTime.Now;

                agreement.RecommenderId = null;
                agreement.ObligationInd = true;
                agreement.ConditionInd = true;
                agreement.MutualAgreementInd = true;
                agreement.WorkTypeId = posted.WorkTypeId;
                agreement.SubmittedDate = DateTime.Now;
                agreement.StartDate = posted.StartDate;
                agreement.EndDate = posted.EndDate;
                agreement.Comments = null;
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
            }
            return _context.SaveChanges() > 0;
        }
        
        public bool Reopen(int agreementID, string modifiedBy)
        {
            Agreement agreement = _context.Agreements.Find(agreementID);
            if (agreement != null)
            {
                agreement.StatusCode = "1";
                agreement.LastUpdateByUserId = modifiedBy;
                agreement.LastUpdateDate = DateTime.Now;

                agreement.RecommenderId = null;                
                agreement.ObligationInd = false;
                agreement.ConditionInd = false;
                agreement.MutualAgreementInd = false;
                agreement.ApprovedRejectedDate = null;
                agreement.ApprovedRejectedById = null;
                agreement.Comments = null;
                agreement.DenyReasonCd = null;

                _context.Update(agreement);                
            }
            return _context.SaveChanges() > 0;
        }


        public bool Approve(int agreementID, string approvedBy, string comments, string modifiedBy)
        {
            Agreement agreement = _context.Agreements.Find(agreementID);
            if (agreement != null)
            {
                agreement.StatusCode = "4";
                agreement.LastUpdateByUserId = modifiedBy;
                agreement.LastUpdateDate = DateTime.Now;

                agreement.ApprovedRejectedDate = DateTime.Now;
                agreement.ApprovedRejectedById = approvedBy;
                agreement.Comments = comments;

                _context.Update(agreement);
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

                _context.Update(agreement);
            }

            return _context.SaveChanges()>0;
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
        public Agreement GetAgreementInfoByUsername(string username)
        {
            var agmt = _context.Agreements
                .Include(x => x.TcUser)
                .Include(x => x.Recommender)
                .Include(x => x.Approver)
                .Include(x => x.Status)
                .Where(x => x.TcUserId.ToLower() == username.ToLower() && !x.ArchivedInd)
                .FirstOrDefault();
            return agmt;
        }
        public Agreement UpdateAgreementStatus(int agreementId, string returnToCode, string updatedById)
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
                } else if (returnToCode == "r")
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

    }
}

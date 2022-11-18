using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Data;
using Microsoft.AspNetCore.Http;
using Web.Pages.Components.Toast;
using Resources;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using Web.Classes;
using System.Linq;
using System.Text.Json;

namespace Web.Pages
{
    public class ApproveModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private readonly AgreementService _agreementService;
        private readonly EmployeeService _employeeService;
        private readonly string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        private readonly Notifications _notifications;


        public ApproveModel(IHttpContextAccessor httpContextAccessor, AgreementService agreementService, EmployeeService employeeService, Notifications notifications)
        {
            _httpContextAccessor = httpContextAccessor;
            _agreementService = agreementService;
            _employeeService = employeeService;
            _notifications = notifications;
        }

        public TcUser EmpInfo { get; set; }
        //public SelectList DenyReasons { get; set; }
        public string sDenyReasons { get; set; }
        public TcUser MgrInfo { get; set; }
        public bool IsTMX { get; set; }
        public bool IsRecommender { get; set; }
        public TcUser RecommendByUserInfo { get; set; }
        [BindProperty]
        public Data.Agreement EmpAgreement { get; set; }
        public TcRegion Region { get; set; }
        public bool canRenew { get; set; }
        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task<IActionResult> OnGet(int id)
        {
            EmpAgreement = _agreementService.GetAgreementById(id);
            if (EmpAgreement == null)
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.InvalidID);
                return RedirectToPage("/MyEmployees");
            }
            var username = Session.GetString("Username");//.ToLower();
            
            //
            var approved_active_agreements = _agreementService.GetAgreementByUsername(EmpAgreement.TcUserId).Where(x=>x.EndDate >= DateTime.Today && x.StatusCode =="4").ToList();
            //canRenew = approved_active_agreements.Count <= 1 || (approved_active_agreements.Count == 2 && approved_active_agreements[1].AgreementId == id);
            canRenew = approved_active_agreements.Count <= 1 || (EmpAgreement.StartDate > DateTime.Today);
            //
            EmpInfo = await _employeeService.GetTcUserInfo(EmpAgreement.TcUserId);
            
            MgrInfo = await _employeeService.GetTcUserInfo(username);
            IsRecommender = MgrInfo.UserId == EmpAgreement.RecommenderId;
            var IsMgr = EmpInfo.Manager!=null && EmpInfo.Manager.ManagerId.Equals(username);
            var IsRecommendee = EmpAgreement.RecommenderId?.Equals(username) ?? false;
            if (!IsMgr && !IsRecommendee) 
            {
                //this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.NotAuthorized);               
                return  RedirectToPage("/Error", new {rs = (int) ErrorMessages.unauthorized });
            }
            if (EmpAgreement.StatusCode == "3" && IsRecommender)
            {
                IsTMX = _employeeService.IsTMX(username);
                RecommendByUserInfo = _employeeService.GetRecommendBy(id, MgrInfo.UserId);
            }
            var denyReasonList = _agreementService.GetDenyReasonList();
            //DenyReasons = new SelectList(_agreementService.GetDenyReasonList(), nameof(DenyReason.DenyReasonId), lang == "en" ? nameof(DenyReason.English) : nameof(DenyReason.French));
            sDenyReasons = JsonSerializer.Serialize (denyReasonList.Select(x => new { name = lang == "en" ? x.English : x.French, value = x.DenyReasonId }).ToList());
            return Page();
        }

        public async Task<IActionResult> OnPostDenyAgreement()
        {
            _agreementService.Deny(
                agreementID: EmpAgreement.AgreementId, 
                denyReasonCd: EmpAgreement.DenyReasonCd,
                deniedBy: Session.GetString("Username"), 
                comments: EmpAgreement.Comments, 
                modifiedBy: Session.GetString("Username"));

            Data.Agreement updatedAgreement = _agreementService.GetAgreementById(EmpAgreement.AgreementId);
            await _notifications.EmailOnCompletion(updatedAgreement);
            this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.RecordSaved);
            return RedirectToPage("/MyEmployees");
        }

        public async Task<IActionResult> OnPostApproveAgreement()
        {
            //if (Upload !=null && (Upload.Length/1024000) <= 40) //max 40Mb
            //{
            //    string ext = Upload.ContentType.Substring(Upload.ContentType.IndexOf('/')+1);
            //    using (Stream fs = Upload.OpenReadStream())
            //    {
            //        using (BinaryReader br = new BinaryReader(fs))
            //        {
            //            byte[] bytes = br.ReadBytes((int)fs.Length);
            //            SupportDocument doc = new SupportDocument() { 
            //                 AgreementId= EmpAgreement.AgreementId,
            //                 FileName = Upload.FileName,
            //                 FileExtension= ext,
            //                 Content= bytes,
            //                 LastUpdateByUserId = Session.GetString("Username")
            //            };
            //            await _agreementService.UploadDoc(doc);

            //        }
            //    }
            //}
            _agreementService.Approve(
            agreementID: EmpAgreement.AgreementId, 
            approvedBy: Session.GetString("Username"), 
            comments: EmpAgreement.Comments, 
            modifiedBy: Session.GetString("Username"));

            Data.Agreement updatedAgreement = _agreementService.GetAgreementById(EmpAgreement.AgreementId);
            await _notifications.EmailOnCompletion(updatedAgreement);
            this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.RecordSaved);
            return RedirectToPage("/MyEmployees");
        }

        public async Task<IActionResult> OnPostRecommendAgreement()
        {
            _agreementService.Recommend(
                agreementID: EmpAgreement.AgreementId,
                recommendedToUsername: EmpAgreement.RecommenderId,
                comments: EmpAgreement.Comments,
                modifiedBy: Session.GetString("Username"));

            Data.Agreement updatedAgreement = _agreementService.GetAgreementById(EmpAgreement.AgreementId);
            await _notifications.EmailOnRecommend(updatedAgreement, EmpAgreement.RecommenderId, Session.GetString("Username"));
            this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.RecordSaved);
            return RedirectToPage("/MyEmployees");
        }
        public async Task<IActionResult> OnPostReopen()
        {
            if (_agreementService.Reopen(agreementID: EmpAgreement.AgreementId, comments: EmpAgreement.Comments, Session.GetString("Username")))
            {
                Data.Agreement updatedAgreement = _agreementService.GetAgreementById(EmpAgreement.AgreementId);
                await _notifications.EmailOnReopen(updatedAgreement, Session.GetString("Username"));
                this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.AgreementReopened);
            }
            else
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.InvalidID);
            }
            return RedirectToPage("/MyEmployees");
            //return Redirect( lang + "/Agreement/Index");
        }
        public async Task<IActionResult> OnPostReturnToRecommender()
        {
            if (_agreementService.ReturnToRecommender(agreementID: EmpAgreement.AgreementId, returndToUsername: EmpAgreement.RecommenderId, comments: EmpAgreement.Comments, Session.GetString("Username")))
            {
                Data.Agreement updatedAgreement = _agreementService.GetAgreementById(EmpAgreement.AgreementId);
                await _notifications.EmailOnReturnToRecommender(EmpAgreement, Session.GetString("Username"));
                //this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.AgreementReopened);
            }

            return RedirectToPage("/MyEmployees");
            //return Redirect( lang + "/Agreement/Index");
        }
    }
}

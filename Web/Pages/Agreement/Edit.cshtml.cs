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

namespace Web.Pages
{
    public class EditAgreementModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private readonly AgreementService _agreementService;
        private readonly EmployeeService _employeeService;
        private readonly string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        private readonly Notifications _notifications;


        public EditAgreementModel(IHttpContextAccessor httpContextAccessor, AgreementService agreementService, EmployeeService employeeService, Notifications notifications)
        {
            _httpContextAccessor = httpContextAccessor;
            _agreementService = agreementService;
            _employeeService = employeeService;
            _notifications = notifications;
        }

        public TcUser EmpInfo { get; set; }
        public SelectList DenyReasons { get; set; }
        public TcUser MgrInfo { get; set; }
        [BindProperty]
        public Data.Agreement EmpAgreement { get; set; }
        public TcRegion Region { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            EmpAgreement = _agreementService.GetAgreementById(id); 

            if (EmpAgreement == null)
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.InvalidID);
                return RedirectToPage("/MyEmployees");
            }
            var username = Session.GetString("Username");//.ToLower();

            EmpInfo = await _employeeService.GetTcUserInfo(EmpAgreement.TcUserId);
            
            MgrInfo = await _employeeService.GetTcUserInfo(username);

            Region = _employeeService.RegionById(EmpAgreement.TcWorksite.RegionCode);

            var IsMgr = EmpInfo.Manager!=null && EmpInfo.Manager.ManagerId.Equals(username);
            var IsRecommendee = EmpAgreement.RecommenderId?.Equals(username) ?? false;
            if (!IsMgr && !IsRecommendee) 
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.NotAuthorized);
                return  RedirectToPage("/MyEmployees");
            }
            DenyReasons = new SelectList(_agreementService.GetDenyReasonList(), nameof(DenyReason.DenyReasonId), lang == "en" ? nameof(DenyReason.English) : nameof(DenyReason.French));

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
        public async Task<IActionResult> OnPostReopen(int id = 0)
        {
            if (_agreementService.Reopen(id, Session.GetString("Username")))
            {
                Data.Agreement updatedAgreement = _agreementService.GetAgreementById(id);
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
    }
}

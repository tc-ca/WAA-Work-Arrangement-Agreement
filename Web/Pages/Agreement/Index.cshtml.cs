using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Web.Pages.Components.Toast;
using Resources;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Web.Pages
{
    public class HomeModel : PageModel
    {
        private ILogger _logger;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private readonly AgreementService _agreementService;
        private readonly EmployeeService _employeeService;        
        private readonly Notifications _notifications;
        private readonly string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        public TcUser EmpInfo { get; set; }
        public bool ShowStartAgreement { get; set; }
        public SelectList Regions { get; set; }
        public SelectList TcWorkSites { get; set; }

        public string WorkSiteGeoCode { get; set; }
        [BindProperty]
        [RequiredIfVisible]
        public string SelectedRegion { get; set; }
        [BindProperty]
        public List<ListItem> OhsSelected { get; set; }
        public bool hasAdHocTeleworkAddress { get; set; }
        [BindProperty]
        public Data.Agreement MyAgreement { get; set; }
        [BindProperty]
        [RequiredIfVisible]
        public string StartDate { get; set; }
        [BindProperty]
        [RequiredIfVisible]
        public string EndDate { get; set; }

        public List<OHSCategory> OhsCheckboxList { get; set; }
        [BindProperty]
        public List<AltWorkSite> AltWorkSiteList { get; set; }

        public TcRegion Region { get; set; }
        public string AdminEmail { get; set; }

        public HomeModel(IConfiguration config, IHttpContextAccessor httpContextAccessor, AgreementService agreementService,
            EmployeeService employeeService, Notifications notifications, ILogger logger)

        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _agreementService = agreementService;
            _employeeService = employeeService;            
            _notifications = notifications;
            _logger = logger;
            AdminEmail = config["EmailSettings:AdminGroup"];
        }

        public async Task OnGet()
        {
            var lang = Template.Language_2Char;
            var username = Session.GetString("Username");
            
            EmpInfo = await _employeeService.GetTcUserInfo(username);
            OhsCheckboxList = await _agreementService.GetOHSCategoryList();

            MyAgreement = _agreementService.GetAgreementByUsername(username);
            OhsSelected =  _agreementService.GetOHSChecklist();
            if (MyAgreement == null)
            {
                MyAgreement = new Data.Agreement();
                MyAgreement.ApproverId = EmpInfo?.Manager?.ManagerId;
                MyAgreement.EmergencyContactPhone = formatTelephone(EmpInfo?.EmergencyContact?.Telephone);
                MyAgreement.EmergencyContactName = EmpInfo?.EmergencyContact?.ContactName;
                MyAgreement.StartDate = DateTime.Today; 
                MyAgreement.EndDate = DateTime.Today.AddYears(1);
                StartDate = MyAgreement.StartDate.ToString("yyyy-MM-dd");
                EndDate = MyAgreement.EndDate.ToString("yyyy-MM-dd");
                hasAdHocTeleworkAddress = false;
            }
            else
            {
                Session.SetInt32("ShowStartAgreement", 0);
                //fulltime on-site no OHS checklist
                if ( !string.IsNullOrEmpty(MyAgreement.TeleworkAddrStreet)) 
                {
                    OhsSelected = OhsSelected.Select(x => new ListItem() { Id = x.Id, Value = "yes" }).OrderBy(x => x.Id).ToList();

                    if (MyAgreement.UnmetOHSItems.Count > 0)
                    {
                        foreach (var o in MyAgreement.UnmetOHSItems)
                        {
                            var foundItem = OhsSelected.Where(x => x.Id == o.UnMetOHSItemId).FirstOrDefault();
                            if (foundItem != null)
                            {
                                foundItem.Value = "no";
                            }
                        }
                    }
                }
                StartDate = MyAgreement.StartDate.ToString("MM/dd/yyyy");
                EndDate = MyAgreement.EndDate.ToString("MM/dd/yyyy");
                hasAdHocTeleworkAddress = !string.IsNullOrEmpty(MyAgreement.TeleworkAddrStreet);
            }
            AltWorkSiteList = new List<AltWorkSite>();
            if (MyAgreement.AltWorkSites!=null) AltWorkSiteList.AddRange(MyAgreement.AltWorkSites);
            int cnt = 2 - AltWorkSiteList.Count;
            for (int i=0; i < cnt; i++)
            {
                AltWorkSiteList.Add(new AltWorkSite() {AgreementId= MyAgreement.AgreementId });
            }

            ShowStartAgreement = Session.GetInt32("ShowStartAgreement").Value == 1 ? true : false;
            Regions = new SelectList(_employeeService.GetTcRegions(), nameof(TcRegion.Id), lang == "en" ? nameof(TcRegion.English) : nameof(TcRegion.French));
            //SelectedRegion = "-1";
            if (MyAgreement.TcWorksite != null){
                SelectedRegion =  MyAgreement.TcWorksite.RegionCode ;
                Region = _employeeService.RegionById(SelectedRegion);
                TcWorkSites = new SelectList(_employeeService.GetWorksites(SelectedRegion), nameof(WorkSite.WorksiteId), lang == "en" ? nameof(WorkSite.English) : nameof(WorkSite.French));
            }

            if (MyAgreement.StatusCode == "5" && !string.IsNullOrEmpty(MyAgreement.DenyReasonCd))
            {
                MyAgreement.DenyReason = _agreementService.GetDenyReasonList().Where(x => x.DenyReasonId == MyAgreement.DenyReasonCd).FirstOrDefault();
            }
            WorkSiteGeoCode = JsonConvert.SerializeObject(_employeeService.GetWorksites(null).ToDictionary(Y => Y.WorksiteId, Z => Z.ProvinceCode));
        }

        public async Task<IActionResult> OnPost()
        {
            // Double check model state.
            if (!ModelState.IsValid)
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.RecordNotSaved);
                var invalidFields = ModelState.Where(x => x.Value.ValidationState == ModelValidationState.Invalid).Select(x => x.Key).ToArray();
                _logger.Error("Cannot save because of invalid model data -- " + String.Join(", ", invalidFields));
                _logger.Error("related info -- user: " + MyAgreement.TcUserId);
                return RedirectToPage("/Error");
            }
            else
            {
                var ohsNoList = OhsSelected.Where(x => x.Value == "no").ToList();
                if (ohsNoList.Count > 0) { 
                    List<UserUnmetOHSItem> unmetOHSItems = new List<UserUnmetOHSItem>();
                    foreach (var noItem in ohsNoList)
                    {
                        unmetOHSItems.Add(new UserUnmetOHSItem() { AgreementId = MyAgreement.AgreementId, UnMetOHSItemId = noItem.Id, LastUpdateByUserId = Session.GetString("Username") });
                    }
                    MyAgreement.UnmetOHSItems = unmetOHSItems;
                }
                AltWorkSiteList.RemoveAll(x=>string.IsNullOrEmpty(x.AltWorkSiteAddrStreet));
                if (AltWorkSiteList.Count > 0)
                {
                    AltWorkSiteList.ForEach(x => x.LastUpdateByUserId = Session.GetString("Username"));
                    MyAgreement.AltWorkSites =  AltWorkSiteList;
                    //MyAgreement.AltWorkSites.AddRange(AltWorkSiteList);
                }
                
                // Update the agreement
                var cultureInfo = new CultureInfo("en-CA");
                MyAgreement.StartDate = DateTime.Parse(StartDate, cultureInfo);
                MyAgreement.EndDate= DateTime.Parse(EndDate, cultureInfo);
                if (MyAgreement.AgreementId == 0)
                {
                    if (_agreementService.CreateAgreement(MyAgreement, Session.GetString("Username")))
                        await _notifications.EmailOnCreation(MyAgreement);
                }
                else
                {
                    if (_agreementService.UpdateAgreement(MyAgreement, Session.GetString("Username")))
                        await _notifications.EmailOnCreation(MyAgreement);
                }

                this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.RecordSaved);

                return RedirectToPage();
            }
        }

        //public IActionResult OnPostReopen(int id = 0)
        //{
        //    if (_agreementService.Reopen(id, Session.GetString("Username")))
        //    {
        //        this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.AgreementReopened);
        //    }
        //    else
        //    {
        //        this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.InvalidID);
        //    }
        //    return RedirectToPage();
        //    //return Redirect( lang + "/Agreement/Index");
        //}
        public IActionResult OnPostUpdateManager(string FullName, string UserName)
        {
            if (_employeeService.UpdateManager( Session.GetString("Username"), UserName, FullName))
            {
                this.CreateToast(ToastStyles.Success, ToastMsgs.Success, "manager updated");
            }
            else
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, "manager updated failed");
            }
            return RedirectToPage();
            //return Redirect( lang + "/Agreement/Index");
        }
        public IActionResult OnPostReopen(int id = 0)
        {
            if (_agreementService.Reopen(id, Session.GetString("Username")))
            {
                this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.AgreementReopened);
            }
            else
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.InvalidID);
            }
            return RedirectToPage();
            //return Redirect( lang + "/Agreement/Index");
        }
        private long? formatTelephone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return null;
            string sPhone = Regex.Replace(phone,"\\D+", "");
            return long.Parse(sPhone);

        }
    }
}

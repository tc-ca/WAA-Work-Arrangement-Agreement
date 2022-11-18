using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Microsoft.Extensions.Configuration;


namespace Web.Pages
{
    public class HomeModel : PageModel
    {
        private readonly AgreementService _agreementService;
        private readonly EmployeeService _employeeService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        public List<Data.Agreement> MyAgreements { get; set; }
        public TcUser TcDirInfo { get; set; } // to be deleted
        public TcRegion Region { get; set; }
        public string AdminEmail { get; set; }
        public HomeModel(IConfiguration config, IHttpContextAccessor httpContextAccessor, AgreementService agreementService,
            EmployeeService employeeService )

        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _agreementService = agreementService;
            _employeeService = employeeService;
            AdminEmail = config["EmailSettings:AdminGroup"];
        }
        public async Task OnGet()
        {
            var username = Session.GetString("Username");
            MyAgreements = _agreementService.GetAgreementByUsername(username).Where(x=>!x.ArchivedInd && x.SubmittedDate!=null).OrderBy(x=>x.SubmittedDate).ToList();
            TcDirInfo = await _employeeService.GetTcDirUserInfo(username);
        }

        public IActionResult OnPostRenew(int id = 0)
        {
            int renewid = _agreementService.Renew(id, Session.GetString("Username"));
            //if renwid<0 --> error
            //return RedirectToPage();
            return Redirect( "Edit?id="+ renewid);
        }
        public IActionResult OnPostReopen(int id = 0)
        {
            _agreementService.Reopen(id,"", Session.GetString("Username"));
            //if renwid<0 --> error
            //return RedirectToPage();
            return Redirect("Edit?id=" + id);
        }
    }
}

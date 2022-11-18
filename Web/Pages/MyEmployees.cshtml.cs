using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Data;
using Microsoft.AspNetCore.Http;
using Web.Pages.Components.Toast;
using Resources;
using System.Globalization;
using System.Linq;

namespace Web.Pages
{
    public class MyEmployeesModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private readonly EmployeeService _employeeService;
        private readonly AgreementService _agreementService;
        private readonly Notifications _notifications;

        public List<DirectReportsModel> DirectReports { get; set; }
        public List<Data.Agreement> MyEmpsAgreements { get; set; }

        public MyEmployeesModel(IHttpContextAccessor httpContextAccessor, EmployeeService employeeService, AgreementService agreementService, Notifications notifications)
        {
            _httpContextAccessor = httpContextAccessor;
            _employeeService = employeeService;
            _agreementService = agreementService;
            _notifications = notifications;
        }

        public async Task OnGet()
        {
            var username = Session.GetString("Username");
           // DirectReports = await _employeeService.GetDirectReports(username);
            DirectReports = await _employeeService.GetMyEmployees(username);
            DirectReports = DirectReports.OrderBy(e => e.FullName).ToList();
            MyEmpsAgreements = await _agreementService.GetMyEmpsAgreements(DirectReports, username);
            //remove expired ones if renewed
            foreach (var ee in DirectReports)
            {
                var agmts = MyEmpsAgreements.Where(x => x.TcUserId == ee.UserName && (x.StatusCode == "4" || x.StatusCode =="6")).ToList();
                if (agmts!=null && agmts.Count == 2)
                {
                    MyEmpsAgreements.Remove(MyEmpsAgreements.FirstOrDefault(x => x.TcUserId == ee.UserName && x.EndDate <= System.DateTime.Today));
                }

            }
        }
        public async Task<IActionResult> OnPostDeleteEmp(string empId)
        {
            var username = Session.GetString("Username");
            //notification  -- 1. if user is still active 2.has agreement sent
            await _employeeService.DeleteFromMyEmployees(empId, username);
            return RedirectToPage();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Resources;
using Web.Pages.Components.Toast;

namespace Web.Pages.Admin
{
    public class SuperuserModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        private readonly EmployeeService _employeeService;
        public List<SuperUser> SuperUsers;

        public SuperuserModel(IHttpContextAccessor httpContextAccessor, EmployeeService employeeService, AgreementService agreementService, Notifications notifications)
        {
            _httpContextAccessor = httpContextAccessor;
            _employeeService = employeeService;
        }

        public async Task OnGet()
        {
            //var username = Session.GetString("Username");

            SuperUsers = await _employeeService.GetAllSuperUser();
        }
        public IActionResult OnPostAddUser(string UserName)
        {
            if (_employeeService.AddSuperUser(UserName, Session.GetString("Username")))
            {
                this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.UserAddSuccess);
            }
            else
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.UserAddFailed);
            }
            return RedirectToPage();
        }
        public IActionResult OnPostDeleteUser(string id)
        {
            if (_employeeService.DeleteSuperUser(id))
            {
                this.CreateToast(ToastStyles.Success, ToastMsgs.Success, ToastMsgs.UserDelSuccess);
            }
            else
            {
                this.CreateToast(ToastStyles.Error, ToastMsgs.Error, ToastMsgs.UserDelFailed);
            }
            return RedirectToPage();
        }
    }
}

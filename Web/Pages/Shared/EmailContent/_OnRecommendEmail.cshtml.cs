using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data;

namespace Web.Pages.Shared.EmailContent
{
    public class _OnRecommendEmailModel : PageModel
    {
        public string BaseURL { get; set; }
        public string EmployeeFullName { get; set; }
        public string SupervisorFullName { get; set; }

        public string ManagerFullName { get; set; }

        public Data.Agreement Agreement { get; set; }

        public void OnGet()
        {
        }
    }
}

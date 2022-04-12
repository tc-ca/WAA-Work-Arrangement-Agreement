using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Shared.EmailContent
{
    public class _OnCompletionApprovedEmailModel : PageModel
    {
        public string EmployeeFullName { get; set; }
        public string ManagerFullName { get; set; }
        public Data.Agreement Agreement { get; set; }
        public string BaseURL { get; set; }

        public string TeleworkScheduleEng { get; set; }
        public string TeleworkScheduleFra { get; set; }
        public string AccommodationIndEng { get; set; }
        public string AccommodationIndFra { get; set; }

        public TcRegion Region { get; set; }

        public void OnGet()
        {
        }
    }
}

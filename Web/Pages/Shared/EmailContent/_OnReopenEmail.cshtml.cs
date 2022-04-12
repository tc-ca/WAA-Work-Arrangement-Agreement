using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data;

namespace Web.Pages.Shared.EmailContent
{
    public class _OnReopenEmailModel : PageModel
    {
        
        public string BaseURL { get; set; }
        public string EmployeeFullName { get; set; }

        public string ManagerFullName { get; set; }
        public Data.Agreement Agreement { get; set; }

        public void OnGet()
        {
        }
    }
}

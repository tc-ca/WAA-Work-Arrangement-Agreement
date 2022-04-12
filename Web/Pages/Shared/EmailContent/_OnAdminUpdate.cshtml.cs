using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data;

namespace Web.Pages.Shared.EmailContent
{
    public class _OnAdminUpdateModel : PageModel
    {
        
        public string BaseURL { get; set; }
        public string EmployeeFullName { get; set; }

        public string ManagerFullName { get; set; }

        public bool isReturnedToEmployee { get; set; }

        public void OnGet()
        {
        }
    }
}

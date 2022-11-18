using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Resources;
using Web.Classes;

namespace Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    public class ErrorModel : PageModel
    {
        public string ErrMsg { get; set; }
        
        public void OnGet()
        {
            int i ;
            int.TryParse( Request.Query["rs"], out i) ;
            switch (i)
            {
                case (int)ErrorMessages.unauthorized:
                    ErrMsg = "You are not authorized to access the requested resource";
                    break;
                case (int)ErrorMessages.RecordNotSaved:
                    ErrMsg = "Failed to update the record";
                    break;
                default:
                    ErrMsg = Global.ErrorMessage;
                    break;
            }
        }
    }
}

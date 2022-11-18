using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

namespace Web.Pages.Agreement
{
    public class ViewDocModel : PageModel
    {
        private AgreementService _agreementService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;
        public ViewDocModel(IHttpContextAccessor httpContextAccessor, AgreementService agreementService)
        {
            _httpContextAccessor = httpContextAccessor;
            _agreementService = agreementService;
        }
        //public async Task<IActionResult> OnGet(int id)
        //{
        //    var username = Session.GetString("Username");

        //    var doc = await _agreementService.GetSupportDocById(id);
        //    if (doc != null)
        //    {
        //        var agreement = _agreementService.GetAgreementById(doc.AgreementId);
        //        if (agreement != null && (agreement.TcUserId == username || agreement.ApproverId == username || agreement.RecommenderId == username))
        //        {
        //            return File(doc.Content, "application/pdf");
        //        }
        //        else
        //        {
        //            //byte[] content = Encoding.UTF8.GetBytes("Error -- You are not authrized to view this file");
        //            //return File(content, "text/plain");
        //            return Unauthorized();
        //        }
        //    }
        //    else
        //    {
        //        //byte[] content = Encoding.UTF8.GetBytes("Error -- File not found");
        //        //return File(content, "text/plain");
        //        return NotFound();
        //    }            
            
        //}
    }
}

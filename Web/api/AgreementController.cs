using Microsoft.AspNetCore.Mvc;
using Repositories;
using Data;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Web.api
{
    [Route("api/[controller]")]
    public class AgreementController : Controller
    {
        private readonly AgreementService _agreementService;
        private readonly EmployeeService _employeeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Notifications _notifications;
        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public AgreementController(AgreementService agreementService, IHttpContextAccessor httpContextAccessor, EmployeeService employeeService, Notifications notifications)
        {
            _agreementService = agreementService;           
            _httpContextAccessor = httpContextAccessor;
            _employeeService = employeeService;
            _notifications = notifications;
        }

        [HttpPost("searchmanager")]
        public ActionResult Searchmanager(string prefix)
        {
            var username = Session.GetString("Username"); //remove user self from approver list
            var managers = _employeeService.GetManagers(prefix).Where(x=>x.UserName!= username).Select(o=>new {label = o.FullName,value=o.UserName });

            return new JsonResult(managers);
        }

        [HttpGet("{lan}/worksites/{region}")]
        public ActionResult worksites(string lan, string region)
        {
            var sites = _employeeService.GetWorksites(region).Select(o => new { id = o.WorksiteId, name = lan == "en" ? o.English : o.French });

            return new JsonResult(sites);
        }
        [HttpPost("searchuser")]
        public async Task<ActionResult> Searchuser(string prefix)
        {
            var username = Session.GetString("Username"); //remove user self from approver list
            var users = await _employeeService.SearchTcUserInfo(prefix);

            return new JsonResult(users.Where(x => x.UserId != username).Select(o => new { label = o.FullNameAndOfficeCode, value = o.UserId }));
        }
        [HttpPost("searchagreement")]
        public ActionResult Searchagreement(string userId, string lan)
        {
            //string[] filter = { "1", "5", "6" };//indicate it's already returned to employee

            var agmts = _agreementService.GetAgreementInfoByUsername(userId);
            return new JsonResult(agmts);
        }
        [HttpPost("updateagreement")]
        public async Task<ActionResult> updateagreement(int id, string returnTo, string lan)
        {
            var username = Session.GetString("Username");
            string returnToUserId;
            bool isAdmin = await _employeeService.IsSuperUser(username);
            if (!isAdmin) return Unauthorized();

            var agmts = _agreementService.UpdateAgreementStatus(id, returnTo, username);
            var returnedAgmt = agmts.FirstOrDefault(x => x.id == id);
            if (returnedAgmt != null)
            {
                if (returnTo == "r")
                {
                    returnToUserId = returnedAgmt.recommenderId;
                }
                if (returnTo == "a")
                {
                    returnToUserId = returnedAgmt.approverId;
                }
                else
                {
                    returnToUserId = returnedAgmt.tcUserId;
                }
                await _notifications.EmailOnAdminUpdate(returnedAgmt, returnToUserId);
            } 
            return new JsonResult(agmts);
        }
        [HttpPost("tmxupdate")]
        public async Task<ActionResult> tmxupdate(string id, string sdate, string edate)
        {
            string message = "updated successfully";
            var username = Session.GetString("Username");

            bool isAdmin = await _employeeService.IsSuperUser(username);
            if (!isAdmin) return Unauthorized();

            if (!_employeeService.UpdateTMXUser(id, sdate, edate, username))
            {
                message = "updated failed";
            }
            return new JsonResult(message);
        }
        //[HttpPost("upload")]
        //public async Task<ActionResult> Upload([FromForm]FileFormData form)
        //{
        //    int docId = -1;
        //    var username = Session.GetString("Username");
        //    var Upload = form?.UpFile;
        //    int.TryParse(form?.Id, out int agreementId);
        //    if (Upload != null && (Upload.Length / 1024000) <= 40 && agreementId > 0) //max 40Mb
        //    {
        //        string ext = Upload.ContentType.Substring(Upload.ContentType.IndexOf('/') + 1);
        //        using (Stream fs = Upload.OpenReadStream())
        //        {
        //            using (BinaryReader br = new BinaryReader(fs))
        //            {
        //                byte[] bytes = br.ReadBytes((int)fs.Length);
        //                SupportDocument doc = new SupportDocument()
        //                {
        //                    AgreementId = agreementId,
        //                    FileName = Upload.FileName,
        //                    FileExtension = ext,
        //                    Content = bytes,
        //                    LastUpdateByUserId = username
        //                };
        //                docId = await _agreementService.UploadDoc(doc);

        //            }
        //        }
        //    }
        //    return new JsonResult(docId);
        //}
        //updaterecommender
        [HttpPost("updaterecommender")]
        public ActionResult updaterecommender(string recommenderId)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(recommenderId))
            {
                var username = Session.GetString("Username");

                ret = _employeeService.SetRecommender(username, recommenderId);
            }
            return new JsonResult(ret);

        }
    }
    public class FileFormData
    {
        public string Id { get; set; }

        public IFormFile UpFile { get; set; }
    }
}

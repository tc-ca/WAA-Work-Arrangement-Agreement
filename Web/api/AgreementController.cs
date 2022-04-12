using Microsoft.AspNetCore.Mvc;
using Repositories;
using Data;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost("create")]
        public ActionResult Create()
        {
            var username = Session.GetString("Username");

            //make sure an agreement doesn't already exist for this user
            if (_agreementService.GetAgreementByUsername(username) == null)
            {
                Session.SetInt32("ShowStartAgreement", 0);
            }
            return Ok();
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

            return new JsonResult(users.Where(x => x.UserId != username).Select(o => new { label = o.FullName, value = o.UserId }));
        }
        [HttpPost("searchagreement")]
        public ActionResult Searchagreement(string userId, string lan)        {
            
            var agmt = _agreementService.GetAgreementInfoByUsername(userId);
            if (agmt != null)
            {
                var agmtView = new
                {
                    id = agmt.AgreementId,
                    //userId= agmt.TcUserId, 
                    employee = agmt.TcUser?.FullName,
                    status = lan == "en" ? agmt.Status.English : agmt.Status.French,
                    statusCode = int.Parse(agmt.StatusCode),
                    decisionDate = agmt.ApprovedRejectedDate,
                    approver = agmt.Approver?.FullName,
                    recommender = agmt.Recommender?.FullName
                };

                return new JsonResult(agmtView);
            }
            else {
                var agmtView = new
                {
                    id = -1
                };
                return new JsonResult(agmtView); 
            };

        }
        [HttpPost("updateagreement")]
        public async Task<ActionResult> updateagreement(int id, string returnTo, string lan)
        {
            var username = Session.GetString("Username");
            string returnToUserId;
            bool isAdmin = await _employeeService.IsSuperUser(username);
            if (!isAdmin) return Unauthorized();

            var agmt = _agreementService.UpdateAgreementStatus(id, returnTo, username);


            if (agmt != null)
            {
                if (returnTo == "r")
                {
                    returnToUserId = agmt.RecommenderId;
                }
                if (returnTo == "a")
                {
                    returnToUserId = agmt.ApproverId;
                }
                else
                {
                    returnToUserId = agmt.TcUserId;
                }
                await _notifications.EmailOnAdminUpdate(agmt, returnToUserId);
                var agmtView = new
                {
                    id = agmt.AgreementId,
                    //userId= agmt.TcUserId, 
                    employee = agmt.TcUser?.FullName,
                    status = lan == "en" ? agmt.Status.English : agmt.Status.French,
                    statusCode= int.Parse(agmt.StatusCode),
                    decisionDate = agmt.ApprovedRejectedDate,
                    approver = agmt.Approver?.FullName,
                    recommender = agmt.Recommender?.FullName
                };

                return new JsonResult(agmtView);
            }
            else
            {
                var agmtView = new
                {
                    id = -1
                };
                return new JsonResult(agmtView);
            };
        }
    }
}

using System.Threading.Tasks;
using Repositories;
using Data;
using Microsoft.Extensions.Configuration;
using Web.Pages.Shared.EmailContent;
using Microsoft.AspNetCore.Http;
using GoC.TC.SecureMailer;
using Web.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Web
{
    public class Notifications
    {
        private readonly EmployeeService _employeeService;
        private readonly RazorPartialToStringRenderer _renderer;
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly string env = "";
        private string subject;
        private string body;
        private string sender;
        private string recipient_to;
        private string recipient_cc = "";
        private string test_recipient;
        public string BaseURL;
        public bool isTesting;

        public Notifications(
            IConfiguration config,
            EmployeeService employeeService,
            AgreementService agreementService,
            SecureSmtpClient emailClient,
            RazorPartialToStringRenderer renderer,
            IHttpContextAccessor accessor,
            IWebHostEnvironment environment)
        {
            _employeeService = employeeService;
            _renderer = renderer;
            _accessor = accessor;
            _config = config;
            _environment = environment;
            env = _environment.IsProduction() ? "" : "(" + _environment.EnvironmentName + ")";
            sender = _config["EmailSettings:EmailFrom"].ToString();
            test_recipient = accessor.HttpContext.Session.GetString("Email");
            BaseURL = $"{_accessor.HttpContext.Request.Scheme}://{_accessor.HttpContext.Request.Host}";
            if (!string.IsNullOrEmpty(_accessor.HttpContext.Request.PathBase))
            {
                BaseURL += $"{_accessor.HttpContext.Request.PathBase}";
            }
            isTesting = (config["EmailSettings:RedirectEmails"].ToString() == "True");
        }
        public async Task EmailOnCreation(Agreement agreement)
        {
            TcUser employee = await _employeeService.GetTcUserInfo(agreement.TcUserId);
            TcUser supervisor = await _employeeService.GetTcUserInfo(employee.Manager.ManagerId);
            subject = $"Work Arrangement Agreement submitted for recommendation/approval / Entente d’aménagement de travail envoyée aux fins de recommandation/approbation";
            recipient_to = supervisor.Email;

            _OnCreationEmailModel partialModel = new _OnCreationEmailModel()
            {
                Agreement = agreement,
                EmployeeFullName = employee.FullName,
                SupervisorFullName = supervisor.FullName,
                BaseURL = BaseURL
            };

            body = await _renderer.RenderPartialToStringAsync("/Pages/Shared/EmailContent/_OnCreationEmail.cshtml", partialModel);
            SendEmail();
        }


        public async Task EmailOnCompletion(Agreement agreement)
        {
            TcUser employee = await _employeeService.GetTcUserInfo(agreement.TcUserId);
            TcUser recommender = await _employeeService.GetTcUserInfo(agreement.RecommenderId);
            TcUser manager = await _employeeService.GetTcUserInfo(employee.Manager.ManagerId);
            recipient_to= employee.Email;

            bool actionByByRecommender = (recommender != null && agreement.ApprovedRejectedById == recommender.UserId);

            if (agreement.StatusCode == "4")
            {
                subject = $"Work Arrangement Agreement – Approved by Delegated Manager / Entente d’aménagement de travail – Approuvée par gestionnaire délégué";
            }
            else if (agreement.StatusCode == "5")
            {
                subject = $"Work Arrangement Agreement - Denied / Décision sur l’Entente d’aménagement de travail - refusée";
            }

            if (agreement.StatusCode == "5")
            {
                if (actionByByRecommender)
                {
                    recipient_cc = recommender.Email;
                }
                
                _OnCompletionDeniedEmailModel deniedModel = new _OnCompletionDeniedEmailModel()
                {
                    Agreement = agreement,
                    EmployeeFullName = employee.FullName,
                    ManagerFullName = actionByByRecommender ? recommender.FullName : manager.FullName,
                    BaseURL = BaseURL
                };
                body = await _renderer.RenderPartialToStringAsync("/Pages/Shared/EmailContent/_OnCompletionDeniedEmail.cshtml", deniedModel);
               
            }
            else
            {
                var teleworkSheduleEng = "Less than 5 days";
                var teleworkSheduleFra = "Moins de 5 jours";
                if (agreement.WorkTypeId == "2")
                {
                    switch (agreement.HybridOptionId)
                    {
                        case 1:
                            teleworkSheduleEng = "Less than 5 days";
                            teleworkSheduleFra = "Moins de 5 jours";
                            break;
                        case 2:
                            teleworkSheduleEng = "5 to 9 days";
                            teleworkSheduleFra = "5 à 9 jours";
                            break;
                        case 3:
                            teleworkSheduleEng = "10 to 14 days";
                            teleworkSheduleFra = "10 à 14 jours";
                            break;
                        case 4:
                            teleworkSheduleEng = "15 to 19 days";
                            teleworkSheduleFra = "15 à 19 jours";
                            break;
                        case 5:
                            teleworkSheduleEng = "20 (and over) days";
                            teleworkSheduleFra = "20 jours (et plus)";
                            break;
                        default:
                            break;
                    }
                }

                var accommodationIndEng = agreement.IsAccommodateDuty == 0 ? "No" : "Yes";
                var accommodationIndFra = agreement.IsAccommodateDuty == 0 ? "Non" : "Oui";
                var region = new TcRegion();
                if (!string.IsNullOrWhiteSpace(agreement.TcWorksite.RegionCode))
                {
                    region = _employeeService.RegionById(agreement.TcWorksite.RegionCode);
                }

                _OnCompletionApprovedEmailModel approvedModel = new _OnCompletionApprovedEmailModel()
                {
                    Agreement = agreement,
                    EmployeeFullName = employee.FullName,
                    ManagerFullName = actionByByRecommender? recommender.FullName: manager.FullName,
                    TeleworkScheduleEng = teleworkSheduleEng,
                    TeleworkScheduleFra = teleworkSheduleFra,
                    AccommodationIndEng = accommodationIndEng,
                    AccommodationIndFra = accommodationIndFra,
                    Region = region,
                    BaseURL = BaseURL,
                };

                body = await _renderer.RenderPartialToStringAsync("/Pages/Shared/EmailContent/_OnCompletionApprovedEmail.cshtml", approvedModel);
            }

            SendEmail(); 
        }


        public async Task EmailOnRecommend(Agreement agreement, string recommenderUsername, string recommendedById)
        {
            TcUser employee = await _employeeService.GetTcUserInfo(agreement.TcUserId);
            TcUser recommendedTo = await _employeeService.GetTcUserInfo(recommenderUsername);
            TcUser recommendedBy = await _employeeService.GetTcUserInfo(recommendedById);
            recipient_to = recommendedTo.Email;
            recipient_cc = employee.Email;
            subject = $"Work Arrangement Agreement for review and approval / Entente d’aménagement de travail pour revue et approbation";

            _OnRecommendEmailModel partialModel = new _OnRecommendEmailModel()
            {
                Agreement = agreement,
                EmployeeFullName = employee.FullName,
                SupervisorFullName = recommendedBy.FullName,
                ManagerFullName = recommendedTo.FullName,                
                BaseURL = BaseURL
            };

            body = await _renderer.RenderPartialToStringAsync("/Pages/Shared/EmailContent/_OnRecommendEmail.cshtml", partialModel);

            SendEmail();

        }

        public async Task EmailOnReopen(Agreement agreement, string updatedBy)
        {
            TcUser employee = await _employeeService.GetTcUserInfo(agreement.TcUserId);
            TcUser manager = await _employeeService.GetTcUserInfo(updatedBy);
            
            if (updatedBy != agreement.ApproverId) { 
                TcUser supervisor = await _employeeService.GetTcUserInfo(agreement.ApproverId); 
                recipient_cc = supervisor.Email; 
            }

            subject = $"Work Arrangement Agreement – Returned / Entente d’aménagement de travail – Renvoyée ";
            recipient_to = employee.Email;

            _OnReopenEmailModel partialModel = new _OnReopenEmailModel()
            {
                Agreement = agreement,
                EmployeeFullName = employee.FullName,
                ManagerFullName = manager.FullName,
                BaseURL = BaseURL
            };

            body = await _renderer.RenderPartialToStringAsync("/Pages/Shared/EmailContent/_OnReopenEmail.cshtml", partialModel);

            SendEmail();

        }
        public async Task EmailOnAdminUpdate(Agreement agreement, string returnTo)
        {  
            subject = $"Work Arrangement Agreement – Returned / Entente d’aménagement de travail – Renvoyée";
            TcUser manager = await _employeeService.GetTcUserInfo(returnTo);
            TcUser employee = await _employeeService.GetTcUserInfo(agreement.TcUserId);

            _OnAdminUpdateModel partialModel = new _OnAdminUpdateModel()
            {
                EmployeeFullName = employee.FullName,
                ManagerFullName = manager.FullName,
                BaseURL = BaseURL,
                isReturnedToEmployee = (returnTo == agreement.TcUserId)
            };

            body = await _renderer.RenderPartialToStringAsync("/Pages/Shared/EmailContent/_OnAdminUpdate.cshtml", partialModel);
            if (returnTo != agreement.TcUserId)
            {
                recipient_to = manager.Email;
                recipient_cc = employee.Email;
            }
            else
            {
                recipient_to = employee.Email;
            }

            SendEmail();

        }
        private bool SendEmail()
        {
            if (isTesting)
            {
                subject = "Testing--" + env + subject;

                body = "=========Testing Email========<br/>Send To:&emsp;" + recipient_to + "<br/>CC:&emsp;" + recipient_cc + "<br/>============================" + body;
                recipient_to = test_recipient;
                recipient_cc = "";
            }
           return Email.Send(subject, body, recipient_to, sender, true, null, recipient_cc);
        }
    }
}

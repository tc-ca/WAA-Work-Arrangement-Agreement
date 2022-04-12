using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Repositories;
using System.Threading.Tasks;
using Data;
using System.Globalization;
using Resources;
using System;
using System.Diagnostics;
using System.DirectoryServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Web.Classes
{
    // Ensures the username, display name and email address of currently logged in user
    // are populated in the active Session.

    public class UserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public UserMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context, EmployeeService empSvc)
        {
            string impersonateName = string.Empty;
            // impersonation is disabled in production env
            if (!_env.IsProduction())
            {                
                if (context.Request.QueryString.HasValue && !string.IsNullOrEmpty(context.Request.Query["UID"]))
                {
                    string currentUser = context.Session.GetString("Username");
                    if (string.IsNullOrEmpty(currentUser) ||(!string.IsNullOrEmpty(currentUser) && context.Request.Query["UID"] != currentUser) )
                    {
                        impersonateName = context.Request.Query["UID"];
                        // set to cookie to address issues when switching between hosting cluster servers 
                        context.Response.OnStarting((e) =>
                        {
                            context.Response.Cookies.Append(
                             "UID", impersonateName,
                             new CookieOptions() { SameSite = SameSiteMode.Lax, IsEssential = true, Path = "/" });
                            return Task.FromResult(0);
                        }, null);
                    }
                } 
            }

            if (string.IsNullOrEmpty(context.Session.GetString("Username")) || !string.IsNullOrEmpty(impersonateName))
            {
                string identityName = context.User.Identity?.Name.Split('\\')[1];
                var cookieValue = context.Request.Cookies["UID"];
                string TesterEmail = null;
                // login user will be null for consultants as they don't have a valid PRI
                if (string.IsNullOrEmpty(impersonateName) && !string.IsNullOrEmpty(cookieValue))
                {
                    impersonateName = cookieValue;
                }
                // impersonateName = ""; //Plese use UserID for only testing purpose.
                if (!string.IsNullOrEmpty(impersonateName)){
                    var testerUser = await empSvc.GetTcUserInfo(identityName);
                    TesterEmail = testerUser?.Email;
                    identityName = impersonateName;
                }                
                var user = await empSvc.GetTcUserInfo(identityName);
                if (string.IsNullOrEmpty(TesterEmail)) TesterEmail = user?.Email;
                context.Session.SetString("Username", user?.UserId ?? "");
                context.Session.SetString("FirstName", user?.GivenName ?? "");
                context.Session.SetString("DisplayName", user?.GivenName + " " + user?.SurName ?? "");
                context.Session.SetString("Email", TesterEmail?? "");
                context.Session.SetString("PhoneNumber", user?.Telephone ?? "");
                context.Session.SetInt32("ShowStartAgreement", 1);
               
                bool isAdmi = await empSvc.IsSuperUser(user?.UserId);
                context.Session.SetString("IsAdmin", isAdmi? "Y" :"N");
                int myEmps =(await empSvc.GetMyEmployees(user?.UserId)).Count;
                context.Session.SetInt32("DirectReportsCount", myEmps);
            }

            await _next(context);
        }
    }
}
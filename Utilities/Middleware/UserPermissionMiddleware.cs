using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities.Middleware
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    public class UserPermissionMiddleware : AuthorizationHandler<PermissionRequirement>
    {
        public IConfiguration configuration;
        public IHttpContextAccessor httpContext;

        public UserPermissionMiddleware(IConfiguration _configuration, IHttpContextAccessor _httpContext)
        {
            configuration = _configuration;
            httpContext = _httpContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            bool isAuthorized = false;
            var Session = httpContext.HttpContext.Session;
           
            if (!string.IsNullOrEmpty(Session.GetString("Username")))
            {   
                isAuthorized = (Session.GetString("IsAdmin").ToString()=="Y");                
            }

            if (Session == null)
            {
                context.Fail();
            }
            else
            {                    
                if (!isAuthorized)
                {
                    context.Fail();
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}

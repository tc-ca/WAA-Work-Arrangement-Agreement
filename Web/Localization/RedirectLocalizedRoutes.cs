using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Localization
{
    public class RedirectLocalizedRoutes : IRule
    {
        private readonly LocalizedRouteMap routeMap;

        public RedirectLocalizedRoutes(LocalizedRouteMap routeMap)
        {
            this.routeMap = routeMap;
        }

        /// <summary>
        /// Looks for a localized path in the LocalizedRouteMap. If one is found, the URL is rewritten in the request to point to the translated path instead.
        /// </summary>
        /// <param name="rewriteContext"></param>
        public void ApplyRule(RewriteContext rewriteContext)
        {
            // Do not translate urls for handler calls!
            if (rewriteContext.HttpContext.Request.QueryString.ToString().Contains("handler="))
            {
                return;
            }

            string path = rewriteContext.HttpContext.Request.Path;

            string culture;
            if (path.StartsWith("/en/"))
            {
                culture = "en";
            }
            else if (path.StartsWith("/fr/"))
            {
                culture = "fr";
            }
            else
            {
                culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }

            //string localizedRoute;

            //if (routeMap.TryTranslate(path, culture, out localizedRoute))
            //{     
            //    HttpResponse response = rewriteContext.HttpContext.Response;
            //    response.StatusCode = StatusCodes.Status301MovedPermanently;
                
            //    var updatedLocation = $"{culture}/{localizedRoute}{rewriteContext.HttpContext.Request.QueryString}";
            //    response.Headers[HeaderNames.Location] = updatedLocation;

            //    rewriteContext.Result = RuleResult.EndResponse;
            //}
        }
    }
}

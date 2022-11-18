using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Web.Localization
{
    public class LocalizedRoute
    {
        public string DefaultPath { get; set; }
        public string Culture { get; set; }
        public string RouteName { get; set; }
        public string LocalizedPath { get; set; }
    }

    public class LocalizedRouteMap
    {
        // ==================================
        //  Configure localized routes here!
        // ==================================

        private List<LocalizedRoute> GenerateLocalizedRoutes()
        {
            var routes = new List<LocalizedRoute>();

            AddRoutes(ref routes, "Error", "error", "erreur");
            AddRoutes(ref routes, "Unauthorized", "unauthorized", "nonautorise");
            AddRoutes(ref routes, "Agreement", "agreement", "entente");
            AddRoutes(ref routes, "Agreement/Edit", "agreement/edit", "agreement/editer");
            AddRoutes(ref routes, "Agreement/Approve", "agreement/approve", "agreement/approuver");
            AddRoutes(ref routes, "Agreement/Index", "agreement/index", "agreement/index");
            //AddRoutes(ref routes, "Agreement/ViewDoc", "agreement/viewdoc", "agreement/viewdoc");
            AddRoutes(ref routes, "MyEmployees", "myemployees", "mesemployes");
            AddRoutes(ref routes, "Admin", "admin", "admin");
            AddRoutes(ref routes, "Admin/Superuser", "admin/Superuser", "admin/Superuser");
            AddRoutes(ref routes, "Admin/Agreement", "admin/Agreement", "admin/Agreement");
            AddRoutes(ref routes, "Admin/TmxMember", "admin/TmxMember", "admin/TmxMember");
            AddRoutes(ref routes, "ParameterTest", "parametertest", "testdeparametre");

            return routes;
        }



        private readonly IMemoryCache cache;

        public LocalizedRouteMap(IMemoryCache cache)
        {
            this.cache = cache;
        }


        /// <summary>
        /// Tries to get nodes from memory cache. If not found, it generates the nodes, puts them in the cache and then returns them.
        /// </summary>
        /// <returns></returns>
        private List<LocalizedRoute> GetLocalizedRoutes()
        {
            List<LocalizedRoute> localizedRoutes;
            if (!cache.TryGetValue("LocalizedRoutes", out localizedRoutes))
            {
                localizedRoutes = GenerateLocalizedRoutes();
                cache.Set("LocalizedRoutes", localizedRoutes, new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.NeverRemove
                });
            }

            return localizedRoutes;
        }


        /// <summary>
        /// Attempts to find a localized route for the specified path and culture. Returns true if found and the translated value (excluding the culture part) is outputted via translatedRoutePath. 
        /// </summary>
        /// <param name="routePath"></param>
        /// <param name="culture"></param>
        /// <param name="translatedRoutePath"></param>
        /// <returns></returns>
        public bool TryTranslate(string routePath, string culture, out string translatedRoutePath)
        {
            var cleanPath = CleanPath(routePath, culture);

            // NOTE: Can't use GetRoute here because you need to check ONLY default path, not localized path. If it is already localized, no translation should take place. This is to avoid an infinite redirection loop.
            var route = GetLocalizedRoutes().Where(x => x.DefaultPath == cleanPath && x.Culture == culture).SingleOrDefault();

            if (route != null)
            {
                translatedRoutePath = route.LocalizedPath;
                return true;
            }            

            translatedRoutePath = null;
            return false;
        }




        /// <summary>
        /// Gets the name of a localized route for the specified path in the specified or current culture.
        /// </summary>
        /// <param name="routePath"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetRouteName(string routePath, string culture = null)
        {
            var route = GetRoute(routePath, culture);

            return route.RouteName;
        }




        /// <summary>
        /// Returns true if the current request path maps to the same page as any of the pathsToCheck. The path values can be default or localized routes.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pathsToCheck"></param>
        /// <returns></returns>
        public bool IsCurrentPage(HttpContext context, params string[] pathsToCheck)
        {
            var currentRoute = GetRoute(context.Request.Path);

            List<LocalizedRoute> routesToCheck = new List<LocalizedRoute>();
            foreach (var path in pathsToCheck)
            {
                routesToCheck.Add(GetRoute(path));
            }

            return routesToCheck.Contains(currentRoute);
        }




        /// <summary>
        /// Returns the route name for opposite culture based on the current request path. Used for the SwitchLanguage partial.
        /// </summary>
        /// <param name="currentRequestPath"></param>
        /// <returns></returns>
        public string GetOtherLanguageRouteName(string currentRequestPath)
        {
            string culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string otherCulture = culture == "en" ? "fr" : "en";

            var currentRoute = GetRoute(currentRequestPath.ToLower(), culture);
            var otherLanguageRoute = GetRoute(currentRoute.DefaultPath, otherCulture);

            return otherLanguageRoute.RouteName;
        }




        /// <summary>
        /// Parses the path to clean out culture values and Index page names.
        /// </summary>
        /// <param name="routePath"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private string CleanPath(string routePath, string culture)
        {
            // Remove the starting "/".
            if (routePath.StartsWith("/"))
            {
                routePath = routePath.Substring(1);
            }

            // Remove culture value from the path.
            if (routePath.StartsWith($"{culture}/"))
            {
                routePath = routePath.Substring(3);
            }

            // Remove /Index if on an index page and it is explicitly included in the path.
            if (routePath.EndsWith("/Index"))
            {
                routePath = routePath.Substring(0, routePath.Length - "/Index".Length);
            }

            return routePath;
        }




        /// <summary>
        /// Finds a localized route by matching the path specified to either a default or localized path. Current culture is used if none is provided.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private LocalizedRoute GetRoute(string path, string culture = null)
        {
            if (String.IsNullOrEmpty(culture))
            {
                culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }

            var cleanPath = CleanPath(path, culture).ToLower();

            LocalizedRoute route = GetLocalizedRoutes()
                .Where(x => x.Culture == culture && (x.DefaultPath.ToLower() == cleanPath || x.LocalizedPath.ToLower() == cleanPath))
                .SingleOrDefault();

            return route;
        }




        /// <summary>
        /// Shorthand for creating a LocalizedRoute definitions. Creates an entry for each culture.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="defaultRoute"></param>
        /// <param name="translatedRouteEN"></param>
        /// <param name="translatedRouteFR"></param>
        private void AddRoutes(ref List<LocalizedRoute> routes, string defaultRoute, string translatedRouteEN, string translatedRouteFR)
        {
            routes.Add(new LocalizedRoute
            {
                DefaultPath = defaultRoute,
                Culture = "en",
                LocalizedPath = translatedRouteEN.ToLower(),
                RouteName = CleanPath(defaultRoute, "en") + "-en"
            });

            routes.Add(new LocalizedRoute
            {
                DefaultPath = defaultRoute,
                Culture = "fr",
                LocalizedPath = translatedRouteFR.ToLower(),
                RouteName = CleanPath(defaultRoute, "fr") + "-fr"
            });
        }

    }

}

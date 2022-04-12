using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Utilities.Middleware;

namespace Utilities
{
    public static class UtilitiesExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// 
        public static IApplicationBuilder UseExceptionEmailerMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionEmailerMiddleware>();
        }
    }
}

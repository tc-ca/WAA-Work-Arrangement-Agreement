using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Web
{
    public class RazorPartialToStringRenderer
    {
        private IRazorViewEngine _viewEngine;
        private ITempDataProvider _tempDataProvider;
        private IServiceProvider _serviceProvider;


        public RazorPartialToStringRenderer(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }


        /// <summary>
        /// Renders the specified partial as a string. partialName must include the absolute path and extension for any location 
        /// other than the default /Pages/Shared folder (i.e. "/Pages/Shared/EmailContent/_OnReopenEmail.cshtml").
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="partialName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var partial = FindView(actionContext, partialName);

            using (var output = new StringWriter())
            {
                var viewDataDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model };
                var tempDataDictionary = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
                var viewContext = new ViewContext(actionContext, partial, viewDataDictionary, tempDataDictionary, output, new HtmlHelperOptions());

                await partial.RenderAsync(viewContext);
                return output.ToString();
            }
        }


        private IView FindView(ActionContext actionContext, string partialName)
        {
            var getPartialResult = _viewEngine.GetView(null, partialName, false);
            if (getPartialResult.Success)
            {
                return getPartialResult.View;
            }

            var findPartialResult = _viewEngine.FindView(actionContext, partialName, false);
            if (findPartialResult.Success)
            {
                return findPartialResult.View;
            }

            var searchedLocations = getPartialResult.SearchedLocations.Concat(findPartialResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations));
            throw new InvalidOperationException(errorMessage);
        }
    }
}
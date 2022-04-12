using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Web.Localization
{
    public class LocalizedRouteModelConvention : IPageRouteModelConvention
    {
        private readonly LocalizedRouteMap routeMap;

        public LocalizedRouteModelConvention(LocalizedRouteMap routeMap)
        {
            this.routeMap = routeMap;
        }

        public void Apply(PageRouteModel model)
        {
            var templates = new List<string>();

            foreach (var selector in model.Selectors)
            {
                selector.AttributeRouteModel.Order = 2;
            }

            var selectorCount = model.Selectors.Count;
            for (var i = 0; i < selectorCount; i++)
            {
                var selector = model.Selectors[i];
                var template = AttributeRouteModel.CombineTemplates("{culture?}", selector.AttributeRouteModel.Template);

                if (templates.Contains(template))
                {
                    continue;
                }

                templates.Add(template);

                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Order = 0,
                        Template = template,
                    }
                });                
            }            

            var cultures = new List<string> { "en", "fr" };

            foreach (var culture in cultures)
            {
                string translatedRoutePath;

                if (routeMap.TryTranslate(model.ViewEnginePath, culture, out translatedRoutePath))
                {
                    var template = AttributeRouteModel.CombineTemplates("{culture="+culture+"}", translatedRoutePath);
                    if (templates.Contains(template))
                    {
                        continue;
                    }

                    templates.Add(template);

                    var newSelector = new SelectorModel { 
                        AttributeRouteModel = new AttributeRouteModel 
                        { 
                            Order = 1, 
                            Template = template, 
                            Name = routeMap.GetRouteName(model.ViewEnginePath, culture) 
                        } 
                    };

                    model.Selectors.Add(newSelector);
                }
            }

            Debug.WriteLine($"Page routes for {model.ViewEnginePath}");
            foreach (var selector in model.Selectors.OrderBy(x => x.AttributeRouteModel.Order))
            {
                Debug.WriteLine($" >> {selector.AttributeRouteModel.Order} >> {selector.AttributeRouteModel.Name} >> {selector.AttributeRouteModel.Template}");
            }
        }
    }
}

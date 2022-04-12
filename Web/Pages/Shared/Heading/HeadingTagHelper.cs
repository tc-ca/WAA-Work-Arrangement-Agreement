using System.Threading;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Utilities.TagHelpers
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using System.Threading.Tasks;


    public class HeadingModel
    {
        public string Primarytext { get; set; }
        public string Secondarytext { get; set; }
        public int HeadingLevel { get; set; }
        public string Icon { get; set; }
    }

    [HtmlTargetElement("Heading", Attributes = "primarytext, secondarytext, headinglevel, icon")]
    public class TemplateRendererTagHelper : TagHelper
    {
        public string Primarytext { get; set; }
        public string Secondarytext { get; set; }
        public int Headinglevel { get; set; }
        public string Icon { get; set; }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private IHtmlHelper _htmlHelper;

        public TemplateRendererTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            (_htmlHelper as IViewContextAware).Contextualize(ViewContext);

            var model = new HeadingModel { Primarytext = Primarytext, Secondarytext = Secondarytext, HeadingLevel = Headinglevel, Icon = Icon };
            output.TagName = null;

            output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("Heading", model));
        }
    }
}


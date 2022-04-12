using System.Threading;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Utilities.TagHelpers
{
    [HtmlTargetElement("lang", Attributes = "en, fr")]
    public class LangTagHelper : TagHelper
    {
        public string En { get; set; }
        public string Fr { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            string stringOut;

            if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "en")
            {
                stringOut = En;
            }
            else
            {
                stringOut = Fr;
            }

            output.Content.SetHtmlContent(stringOut);
        }
    }
}

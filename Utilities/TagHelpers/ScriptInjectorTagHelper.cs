using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace Utilities.TagHelpers
{

    // Enables "on-content-loaded" attribute for script tags.
    // Places the script at the end of the document and runs it once the document is loaded.
    // Ideal for scripts in partial views where @Section can't be used.

    [HtmlTargetElement("script", Attributes = "on-content-loaded")]
    public class ScriptTagHelper : TagHelper
    {
        public bool OnContentLoaded { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!OnContentLoaded)
            {
                base.Process(context, output);
            }
            else
            {
                var content = output.GetChildContentAsync().Result;
                var javascript = content.GetContent();

                var sb = new StringBuilder();
                sb.Append("document.addEventListener('DOMContentLoaded',");
                sb.Append("function() {");
                sb.Append(javascript);
                sb.Append("});");

                output.Content.SetHtmlContent(sb.ToString());
            }
        }

    }

    public class IfTagHelper : TagHelper
    {
        public override int Order => -1000;

        [HtmlAttributeName("include-if")]
        public bool Include { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Always strip the outer tag name as we never want <if> to render
            output.TagName = null;

            if (Include)
            {
                return;
            }

            output.SuppressOutput();
        }
    }
}
using System.Threading;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Utilities.TagHelpers
{

    [HtmlTargetElement("address", Attributes = "street, city, province, postal")]
    public class AddressTagHelper : TagHelper
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Postal { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;
            string stringOut;

            if (Street.Length == 0)
            {
                stringOut = City + ", " + Province;
            }
            else
            {
                stringOut = Street + ", " + City + ", " + Province + ", " + Postal;
            }

            output.Content.SetHtmlContent(stringOut);
        }
    }
}

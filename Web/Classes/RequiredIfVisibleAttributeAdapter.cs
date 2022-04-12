using Data;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Globalization;

namespace Web
{
    public class RequiredIfVisibleAttributeAdapter : AttributeAdapterBase<RequiredIfVisibleAttribute>
    {
        public RequiredIfVisibleAttributeAdapter(RequiredIfVisibleAttribute attribute,
            IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {

        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-requiredIfVisible", GetErrorMessage(context));
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext) =>
            Attribute.GetErrorMessage();
    }
}

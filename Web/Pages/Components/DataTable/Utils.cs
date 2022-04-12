using Castle.Core.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Web.Pages.Components.DataTable
{
    public static class Utils
    {
        // ===== LIST HELPERS =======================================================

        public static Val[] BoolDropdownValues()
        {
            return new Val[]
            {
                new Val { Label = DataTableResources.True, Value = "true" },
                new Val { Label = DataTableResources.False, Value = "false" }
            };
        }

        public static Dictionary<string, string> ValuesToDictionary(Val[] values)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var v in values)
            {
                dictionary.Add(v.Value.ToString(), v.Label);
            }
            return dictionary;
        }




        // ===== ATTRIBUTE HELPERS ==================================================


        /// <summary>
        /// If the property has a Display attribute, use GetName() to get the localized value. If not, just use the property name.
        /// If the property has a Required attribute, add a red asterisk icon.
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static string GetColumnHeader(this PropertyInfo propInfo)
        {
            string title;

            var displayAtt = propInfo.GetAttribute<DisplayAttribute>();
            title = displayAtt?.GetName() ?? propInfo.Name;

            var requiredAtt = propInfo.GetAttribute<RequiredAttribute>();
            if (requiredAtt != null)
            {
                title += "<i class=\"tiny top aligned red asterisk icon\"></i>";
            }
            return title;
        }

        





        // ===== DATA VALIDATION =====================================================

        /// <summary>
        /// Generate validator parameters based on the property's validation attributes.
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static string[] BuildValidatorParams(this PropertyInfo propInfo)
        {
            var validation = new List<string>();

            var requiredAtt = propInfo.GetAttribute<RequiredAttribute>();
            if (requiredAtt != null)
            {
                validation.Add("required");
            }

            var maxLengthAtt = propInfo.GetAttribute<MaxLengthAttribute>();
            if (maxLengthAtt != null)
            {
                validation.Add($"maxLength:{maxLengthAtt.Length}");
            }

            return validation.ToArray();
        }


        /// <summary>
        /// Scans the property's attributes and applies restrictions on the input element as necessary. (i.e. MaxLength)
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        public static EditorParams BuildEditorParams(this PropertyInfo propInfo)
        {
            var elementAttributes = new ElementAttributes();
            bool applySettings = false;

            // MaxLength
            var maxLengthAtt = propInfo.GetAttribute<MaxLengthAttribute>();
            if (maxLengthAtt != null)
            {
                elementAttributes.MaxLength = maxLengthAtt.Length.ToString();
                applySettings = true;
            }

            return applySettings
                ? new EditorParams() { ElementAttributes = elementAttributes }
                : null;
        }
    }
}

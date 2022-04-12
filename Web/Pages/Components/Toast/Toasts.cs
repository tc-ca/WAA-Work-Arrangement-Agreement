using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Web.Pages.Components.Toast
{
    public enum ToastStyles
    {
        Error, Info, Success, Warning
    };
    public class ToastModel
    {
        public string Style { get; set; } = "";
        public string Header { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public static class Toasts
    {
        public static void CreateToast(this PageModel page, ToastStyles style, string header, string message)
        {
            List<ToastModel> toastList;

            if (page.TempData["__toasts"] == null)
            {
                toastList = new List<ToastModel>();
            }
            else
            {
                toastList = page.TempData["__toasts"] as List<ToastModel>;
            }

            toastList.Add(new ToastModel
            {
                Style = StyleToString(style),
                Header = header,
                Message = message
            });

            page.TempData["__toasts"] = JsonConvert.SerializeObject(toastList);
        }

        private static string StyleToString(ToastStyles style)
        {
            switch (style)
            {
                case ToastStyles.Info:
                    return "info";
                case ToastStyles.Warning:
                    return "warning";
                case ToastStyles.Success:
                    return "success";
                case ToastStyles.Error:
                    return "error";
                default:
                    return "";
            }
        }
    }
}
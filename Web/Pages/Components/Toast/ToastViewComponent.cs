using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Web.Pages.Components.Toast
{
    public class ToastViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var toastListData = TempData["__toasts"];

            List<ToastModel> toasts;

            if (toastListData == null)
            {
                toasts = new List<ToastModel>();
            }
            else
            {
                toasts = JsonConvert.DeserializeObject<List<ToastModel>>(TempData["__toasts"].ToString());
            }

            return View(toasts);
        }
    }
}

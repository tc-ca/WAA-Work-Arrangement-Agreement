using Microsoft.AspNetCore.Mvc;
using System;

namespace Web.Pages.Components.DataTable
{

    public class DataTableControlModel
    {
        public string Header { get; set; }
        public Type Entity { get; set; }
        public string SaveApi { get; set; }
        public string DeleteApi { get; set; }
        public string ColumnInfo { get; set; }
        public string Data { get; set; }
        public int RecordCount { get; set; }
    }

    public class DataTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DataTableControlModel model)
        {
            return View(model);
        }

        //public IViewComponentResult Invoke(Type entity, string data, string columnInfo, string saveApi)
        //{
        //    var dcm = new DataTableControlModel() { Entity = entity, SaveApi = saveApi, Data = data, ColumnInfo = columnInfo };
        //    return View(dcm);
        //}
    }
}
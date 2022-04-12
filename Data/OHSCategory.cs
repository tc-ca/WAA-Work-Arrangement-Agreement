using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("TH232_WAA_OHS_CATEGORY")]
    public class OHSCategory
    {
        [Key, Column("OHS_CATEGORY_CD")]
        public string CategoryId { get; set; }

        [Column("OHS_CATEGORY_NAME_ENM")]
        public string English { get; set; }

        [Column("OHS_CATEGORY_NAME_FNM")]
        public string French { get; set; }

        [Column("DATE_DELETED_DTE")]
        public DateTime? DeleteDate { get; set; }

        public ICollection<OHSChecklist> OHSChecklist { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("TH233_WAA_OHS_CHECKLIST")]
    public class OHSChecklist 
    { 
        [Key, Column("OHS_CHECKLIST_ID")]
        public int CheckListId { get; set; }

        [Column("OHS_CATEGORY_CD"), ForeignKey("OHSCategory")]
        public string CategoryId { get; set; }

        [Column("OHS_CHECKLIST_ENM")]
        public string English { get; set; }

        [Column("OHS_CHECKLIST_FNM")]
        public string French { get; set; }

        [Column("DATE_DELETED_DTE")]
        public DateTime? DeleteDate { get; set; }

        public OHSCategory OHSCategory { get; set; }
    }
}

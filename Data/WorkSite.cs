using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("AH131_WAA_WORK_SITE")]
    public class WorkSite
    {
        [Key, Column("WORK_SITE_ID")]
        public int WorksiteId { get; set; }
        [Column("REGION_CD")]
        public string RegionCode { get; set; }
        [Column("PROVINCE_GEO_CD")]
        public string ProvinceCode { get; set; }
        [Column("WORK_SITE_NAME_ENM")]
        public string English { get; set; }

        [Column("WORK_SITE_NAME_FNM")]
        public string French { get; set; }

        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }

        [Column("DATE_LAST_UPDATE_DTE")]
        public DateTime? LastUpdateDate { get; set; }

        [Column("DATE_DELETED_DTE")]
        public DateTime? DeleteDate { get; set; }
    }
}

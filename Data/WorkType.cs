using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("TH230_WAA_WORK_TYPE")]
    public class WorkType
    {
        [Key, Column("WAA_WORK_TYPE_CD")]
        public string WorkTypeCd { get; set; }

        [Column("WAA_WORK_TYPE_ENM")]
        public string English { get; set; }

        [Column("WAA_WORK_TYPE_FNM")]
        public string French { get; set; }

        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }

        [Column("DATE_LAST_UPDATE_DTE")]
        public DateTime? LastUpdateDate { get; set; }

        [Column("DATE_DELETED_DTE")]
        public DateTime? DeleteDate { get; set; }
    }
}

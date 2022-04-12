using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("TH231_WAA_STATUS")]
    public class Status
    {
        [Key, Column("WAA_STATUS_CD")]
        public string StatusCd { get; set; }

        [Column("WAA_STATUS_ENM")]
        public string English { get; set; }

        [Column("WAA_STATUS_FNM")]
        public string French { get; set; }

        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }

        [Column("DATE_LAST_UPDATE_DTE")]
        public DateTime? LastUpdateDate { get; set; }

        [Column("DATE_DELETED_DTE")]
        public DateTime? DeleteDate { get; set; }
    }
}

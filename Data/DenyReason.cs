using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("TH234_WAA_DENY_REASON")]
    public class DenyReason
    {
        [Key, Column("WAA_DENY_REASON_CD")]
        public string DenyReasonId { get; set; }

        [Column("WAA_DENY_REASON_ENM")]
        public string English { get; set; }

        [Column("WAA_DENY_REASON_FNM")]
        public string French { get; set; }
    }
}

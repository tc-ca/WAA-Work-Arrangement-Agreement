using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("AH137_WAA_TMX_MEMBER")]
    public class TMXMember
    {    
        [Key, Column("TC_USER_ID"), ForeignKey("TcUser")]
        public string Tc_user_id { get; set; }
        [Column("DATE_START_DTE")]
        public DateTime EffectiveStartDate { get; set; }
        [Column("DATE_END_DTE")]
        public DateTime? EffectiveEndDate { get; set; }
        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }
        [Column("DATE_LAST_UPDATE_DTE")]
        public DateTime LastUpdateDate { get; set; }
        public TcUser TcUser { get; set; }
    }
}

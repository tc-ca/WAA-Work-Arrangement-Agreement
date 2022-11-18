using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("AH136_WAA_FTR_RECOMMENDER")]
    public class FTRRecommender
    {    
        [Key, ForeignKey("agreement"), Column("AGREEMENT_ID")]
        public int AgreementId { get; set; }
        [Column("RECOMM_LVL1_USER_ID")]
        public string RecommerLevel1 { get; set; }

        [Column("RECOMM_LVL2_USER_ID")]
        public string RecommerLevel2 { get; set; }
        [Column("RECOMM_LVL3_USER_ID")]
        public string RecommerLevel3 { get; set; }

        [Column("RECOMM_LVL4_USER_ID")]
        public string RecommerLevel4 { get; set; }

        public Agreement agreement { get; set; }
    }
}

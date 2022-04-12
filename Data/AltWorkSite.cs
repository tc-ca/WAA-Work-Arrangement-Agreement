using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("AH134_WAA_ALT_GOC_WORK_SITE")]
    public class AltWorkSite
    {
        [Key, Column("ALT_GOC_WORK_SITE_ID")]
        public int AltWorksiteId { get; set; }        
        [ForeignKey("agreement"), Column("AGREEMENT_ID")]
        public int AgreementId { get; set; }
        [Column("ALT_GOC_ADR_STREET_TXT")]
        public string AltWorkSiteAddrStreet { get; set; }

        [Column("ALT_GOC_ADR_CITY_TXT")]
        public string AltWorkSiteAddrCity { get; set; }
        [Column("ALT_GOC_ADR_PROVINCE_TXT")]
        public string AltWorkSiteAddrProvince { get; set; }

        [Column("ALT_GOC_ADR_POSTAL_CODE_TXT")]
        public string AltWorkSiteAddrPostal { get; set; }

        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }

        [Column("DATE_LAST_UPDATE_DTE")]
        public DateTime? LastUpdateDate { get; set; }

        [Column("DATE_DELETED_DTE")]
        public DateTime? DeleteDate { get; set; }

        public Agreement agreement { get; set; }
    }
}

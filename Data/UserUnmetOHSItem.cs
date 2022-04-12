using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("AH133_WAA_UNMET_OHS_CONDITION")]
    public class UserUnmetOHSItem
    {
        [Key, ForeignKey("agreement"), Column("AGREEMENT_ID")]
        public int AgreementId { get; set; }

        [Key, ForeignKey("OHSChecklist"),Column("OHS_CHECKLIST_ID")]
        public int UnMetOHSItemId { get; set; }

        public Agreement agreement { get; set; }
       
        public OHSChecklist OHSChecklist { get; set; }
        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }

    }
}
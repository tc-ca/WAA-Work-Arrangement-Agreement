using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("BRANCH")]
    public class TcBranch
    {
        [Key, Column("BRA_BRANCH_IND")]
        public string BranchId { get; set; }

        [Column("BRA_BRANCH_NAME_E")]
        public string English { get; set; }

        [Column("BRA_BRANCH_NAME_F")]
        public string French { get; set; }
    }
}

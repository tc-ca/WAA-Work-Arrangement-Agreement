using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("MV007_TCDIR_ORGANIZATION")]
    public class TcOrganization
    {
        [Key, Column("TCDIR_ORG_ID")]
        public int OrgId { get; set; }

        [Column("TCDIR_PARENT_ORG_ID")]
        public int OrgParentId { get; set; }

        [Column("TCDIR_ORG_ENM")]
        public string English { get; set; }

        [Column("TCDIR_ORG_FNM")]
        public string French { get; set; }
    }
}

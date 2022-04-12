using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("REGIONS")]
    public class TcRegion
    {
        [Key, Column("REG_REGION_IND")]
        public string Id { get; set; }

        [Column("REG_REGION_DESC_E")]
        public string English { get; set; }

        [Column("REG_REGION_DESC_F")]
        public string French { get; set; }

    }
}

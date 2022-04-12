using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("AH135_WAA_SUPER_USER")]
    public class SuperUser
    {
        [Key, Column("TC_USER_ID"), ForeignKey("TcUser")]
        public string UserId { get; set; }
        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }

        public TcUser TcUser { get; set; }
    }
}

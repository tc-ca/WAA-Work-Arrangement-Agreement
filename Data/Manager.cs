using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("VW_WAA_MANAGER")]
    public class Manager
    {
        [Key, Column("USER_ID")]
        public string UserId { get; set; }

        [Column("USER_NAME")]
        public string ManagerName { get; set; }

        [Column("USER_FAMILY_NAME")]
        public string ManagerSurname { get; set; }
        [Column("USER_GIVEN_NAME")]
        public string ManagerGivenName { get; set; }
    }
}

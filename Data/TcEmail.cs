using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("MV002_TC_EMAIL_ADDRESS")]
    public class TcEmail
    {
        [Key, Column("EMPLOYEE_PIN")]
        public string Pri { get; set; }

        [Column("TC_USER_ID")]
        public string TcUserId { get; set; }

        [Column("EMPLOYEE_NAME")]
        public string Name { get; set; }

        [Column("TC_EMAIL_TXT")]
        public string Email { get; set; }
    }
}

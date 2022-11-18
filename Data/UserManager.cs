using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("AH132_WAA_MANAGER")]
    public class UserManager
    {
        [Key, Column("TC_USER_ID")]
        public string UserId { get; set; }

        [Column("MGR_USER_ID")]
        public string ManagerId { get; set; }

        [Column("MGR_SURNAME_NM")]
        public string ManagerSurname { get; set; }
        [Column("MGR_GIVEN_NAME_NM")]
        public string ManagerGivenName { get; set; }
        [Column("RECOMMEDER_ID")]
        public string RecommenderId { get; set; }
        [Column("RECOMMEDER_SURNAME_NM")]
        public string RecommenderSurname { get; set; }
        [Column("RECOMMEDER_GIVEN_NAME_NM")]
        public string RecommenderGivenName { get; set; }
        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }
        [NotMapped]
        public string RecommenderUserId { get { return RecommenderId != null? RecommenderId : ManagerId ; } }
        [NotMapped]
        public string RecommenderFullName { get { return RecommenderId != null ? $"{RecommenderGivenName} {RecommenderSurname}" : $"{ManagerGivenName} {ManagerSurname}"; } }
    }
}

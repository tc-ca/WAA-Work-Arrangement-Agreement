using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("MV008_TCDIR_USERS")]
    public class TcUser
    {
        [Key, Column("TC_USER_ID")]
        public string UserId { get; set; }
        [Column("EMP_PIN")]
        public string EmployeePin { get; set; }

        [Column("EMP_GIVEN_NAME")]
        public string GivenName { get; set; }

        [Column("EMP_SURNAME")]
        public string SurName { get; set; }

        [Column("EMP_INITIALS")]
        public string Initial { get; set; }

        [Column("POS_NUMBER")]
        public string PositionNumber { get; set; }
        
        [Column("POS_ENM")]
        public string PositionEng { get; set; }
       
        [Column("POS_FNM")]
        public string PositionFra { get; set; }
        
        [Column("MGR_POS_NUMBER")]
        public string ManagerPositionNumber { get; set; }
        
        //[Column("MGR_USER_ID")]
        //public string ManagerUserId { get; set; }
        
        [Column("ORG_ID"), ForeignKey("Directorate")]
        public int OrganizationId { get; set; }

        [Column("TELEPHONE")]
        public string Telephone { get; set; }

        [Column("OFFICE_BUILDING_EN")]
        public string OfficeBuildingEng { get; set; }

        [Column("OFFICE_BUILDING_EN")]
        public string OfficeBuildingFra { get; set; }

        [Column("OFFICE_ADDR_EN")]
        public string OfficeAddrEng { get; set; }

        [Column("OFFICE_ADDR_FR")]
        public string OfficeAddrFra { get; set; }       

        [Column("OFFICE_LOCATION_EN")]
        public string OfficeLocationEng { get; set; }
        [Column("OFFICE_LOCATION_FR")]
        public string OfficeLocationFra { get; set; }
        [Column("TC_EMAIL_TXT")]
        public string Email { get; set; }
        [Column("GROUP_AND_LEVEL")]
        public string GroupAndLevel { get; set; }
        [Column("BRANCH_ID"), ForeignKey("Branch")]
        public string BranchId { get; set; } 
        [Column("GEO_CODE")]
        public string GeoCode { get; set; }
        public TcOrganization Directorate { get; set; }
        public TcBranch Branch { get; set; }
        [NotMapped]
        public UserManager Manager { get; set; }

        [NotMapped]
        public EmergencyContact EmergencyContact { get; set; }
        [NotMapped]
        public string FullName { get { return $"{GivenName} {SurName}"; } }
    }
    public class EmergencyContact
    {
        public string ContactName { get; set; }
        public string Telephone { get; set; }
    }
}

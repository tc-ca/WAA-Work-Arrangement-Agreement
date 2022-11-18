using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    [Table("AH130_WAA_AGREEMENT")]
    public class Agreement
    {
        [Key, Column("AGREEMENT_ID")]
        public int AgreementId { get; set; }

        [Column("TC_USER_ID"),ForeignKey("TcUser")]
        public string TcUserId { get; set; }

        [Column("STATUS_CD"), ForeignKey("Status")]
        public string StatusCode { get; set; }
        
        [Column("DATE_SUBMITTED_DTE")]
        public DateTime? SubmittedDate { get; set; }

        [Column("DECISION_DATE_DTE")]
        public DateTime? ApprovedRejectedDate { get; set; }

        [Column("DECISION_BY_USER_ID"), ForeignKey("ApproveRejectedBy")]
        public string ApprovedRejectedById { get; set; }

        [Column("RECOMMENDER_ID"),ForeignKey("Recommender")]
        public string RecommenderId { get; set; }

        [Column("APPROVER_ID"), ForeignKey("Approver")]
        public string ApproverId { get; set; }

        [Column("AGREEMENT_START_DATE_DTE")]
        public DateTime StartDate { get; set; }

        [Column("AGREEMENT_END_DATE_DTE")]
        public DateTime EndDate { get; set; }

        [Column("OBLIGATION_CONFIRMED_IND")]
        public bool ObligationInd { get; set; }

        [Column("CONDITION_CONFIRMED_IND")]
        public bool ConditionInd { get; set; }
        [Column("MUTUAL_AGREEMENT_CONFIRMED_IND")]
        public bool MutualAgreementInd { get; set; }
        [Required]
        [Column("WORK_TYPE_CD"),ForeignKey("WorkType")]
        public string WorkTypeId { get; set; }

        [Column("EMERGENCY_CONTACT_NAME_NM")]
        public string EmergencyContactName { get; set; }
        
        [Column("EMERGENCY_CONTACT_PHONE_NUM")]
        public long? EmergencyContactPhone { get; set; }
        
        [Column("COMMENT_TXT")]
        public string Comments { get; set; }

        [Column("ARCHIVED_IND")]
        public bool ArchivedInd { get; set; }
        [Column("DATE_CREATED_DTE")]
        public DateTime? CreateDate { get; set; }
        [Column("USER_LAST_UPDATE_ID")]
        public string LastUpdateByUserId { get; set; }

        [Column("DATE_LAST_UPDATE_DTE")]
        public DateTime LastUpdateDate { get; set; }

        [Column("DATE_DELETED_DTE")]
        public DateTime? DeleteDate { get; set; }

        [Required]
        [Column("ACCOMMODATION_IND")]
        public int? IsAccommodateDuty { get; set; }
        [Column("VIRTUAL_IND")]
        public int? IsVirtual { get; set; }
        [RequiredIfVisible]
        [Column("TELEWORK_ADR_STREET_TXT")]
        public string TeleworkAddrStreet { get; set; }
        [RequiredIfVisible]
        [Column("TELEWORK_ADR_CITY_TXT")]
        public string TeleworkAddrCity { get; set; }
        [RequiredIfVisible]
        [Column("TELEWORK_ADR_PROVINCE_TXT")]
        public string TeleworkAddrProvince { get; set; }
        [RequiredIfVisible]
        [Column("TELEWORK_ADR_POSTAL_CODE_TXT")]
        public string TeleworkAddrPostal { get; set; }
        [Column("WORK_SITE_ID"),ForeignKey("TcWorksite")]
        [Required]
        public int TcWorksiteId { get; set; } 
        [Column("DENY_REASON_CD"), ForeignKey("DenyReason")]
        public string DenyReasonCd { get; set; }

        [Column("VARIABLE_SCHEDULE_TXT")]
        public string VariableScheduleDetails { get; set; }
        [RequiredIfVisible]
        [Column("HYBRID_OPTION_CD"), ForeignKey("HybridOption")]
        public int? HybridOptionId { get; set; }
        //[Column("VERSION_CD")]
        //public int? Version { get; set; }

        [Column("RENEW_NOTIF_SENT_IND")]
        public int? RenewNotified { get; set; }

        public WorkType WorkType { get; set; }
        public TcUser TcUser { get; set; }
        public TcUser Recommender { get; set; }
        public TcUser Approver { get; set; }
        public TcUser ApproveRejectedBy { get; set; }
        public WorkSite TcWorksite { get; set; }
        public DenyReason DenyReason { get; set; }

        public Status Status { get; set; }
        public HybridOption HybridOption { get; set; }
        public ICollection<UserUnmetOHSItem> UnmetOHSItems { get; set; }

        public ICollection<AltWorkSite> AltWorkSites { get; set; }

        // handle cases employees/managers left TC
        [NotMapped]
        public string TcUserName => TcUser != null ? TcUser.FullName : TcUserId;
        [NotMapped]
        public string ApproverName => Approver != null ? Approver.FullName : ApproverId; 
        [NotMapped]
        public string RecommenderName => Recommender != null ? Recommender.FullName : RecommenderId;
        [NotMapped]
        public string ApproveRejectedByName => ApproveRejectedBy != null ? ApproveRejectedBy.FullName : ApprovedRejectedById;
        public Agreement()
        {
            StatusCode = "0"; 
        }
    }
}

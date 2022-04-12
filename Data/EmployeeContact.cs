using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    [Table("ADDRESSES")]
    public class EmployeeContact
    {
        [Key,Column("ADR_PIN")]
        public string Pri { get; set; }

        [Column("ADR_ADDRESS_TYPE")]
        public string AddressType { get; set; } //5= emergency contact
        [Column("ADR_LINE1")]
        public string FullName { get; set; }
        [Column("ADR_ADDRESS_START_DATE")]
        public DateTime EffectiveDate { get; set; }
        [Column("ADR_PHONE_NMBR")]
        public string PhoneNumber { get; set; }
        [Column("ADR_ALTERNATE_PHONE_EXT_NUM")]
        public string PhoneNumberAlt { get; set; }
        [Column("ADR_EMAIL_ADDRESS_TXT")]
        public string EamilAddress { get; set; }
    }
}

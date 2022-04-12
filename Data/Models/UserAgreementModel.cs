
using System.Collections.Generic;

namespace Data
{
    public class UserAgreementModel
    {
        public TcUser User { get; set; }
        public Agreement Agreement { get; set; }
        public TcRegion Region { get; set; }
        public List<OHSCategory> OhsCheckboxList { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class AgreementInfo
    {
        public int id { get; set; }
        public string employee { get; set; }
        public string tcUserId { get; set; }
        public string statusCode { get; set; }
        public DateTime? decisionDate { get; set; }
        public string status { get; set; }
        public string approver { get; set; }
        public string approverId { get; set; }
        public string recommender { get; set; }
        public string recommenderId { get; set; }
        public int canReturn { get; set; }
    }
}

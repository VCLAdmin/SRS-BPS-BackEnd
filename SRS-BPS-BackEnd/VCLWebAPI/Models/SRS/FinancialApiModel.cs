using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCLWebAPI.Models.SRS
{
    public class FinancialApiModel
    {
        public int FinancialId { get; set; }
        public System.Guid FinancialExternalId { get; set; }

        public int DealerId { get; set; }
        public string DealerName { get; set; }

        public double LineOfCredit { get; set; }
        public double OrdersToDate { get; set; }
        public double PaidToDate { get; set; }
        public double CurrentBalance { get; set; }

        public string CreatedByName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }

    }
}
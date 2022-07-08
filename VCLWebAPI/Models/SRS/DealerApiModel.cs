using System;
using System.Collections.Generic;

namespace VCLWebAPI.Models.SRS
{
    public class DealerApiModel
    {
        public int DealerId { get; set; }
        public Nullable<System.Guid> DealerGuid { get; set; }
        public string Name { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string PrimaryContactPhone { get; set; }
        public double  CreditLine { get; set; }
        
        public int AWSFabricatorId { get; set; }
        public int ADSFabricatorId { get; set; }
        public int ASSFabricatorId { get; set; }
        public FabricatorApiModel AWSFabricator { get; set; }
        public FabricatorApiModel ADSFabricator { get; set; }
        public FabricatorApiModel ASSFabricator { get; set; }

        public Nullable<double> DefaultSalesTax { get; set; }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }

        public double CreditUsed { get; set; }
    }

    
}
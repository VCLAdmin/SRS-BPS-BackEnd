using System;
using System.Collections.Generic;

namespace VCLWebAPI.Models.SRS
{
    public class FabricatorApiModel
    {
        public int FabricatorId { get; set; }
        public System.Guid FabricatorGuid { get; set; }
        public string Name { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public string PostalCode { get; set; }
        public sbyte SupportsAWS { get; set; }
        public sbyte SupportsADS { get; set; }
        public sbyte SupportsASS { get; set; }
    }
}
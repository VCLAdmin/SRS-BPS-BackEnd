//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VCLWebAPI.Models.Edmx
{
    using System;
    using System.Collections.Generic;
    
    public partial class StateTax
    {
        public int StateTaxId { get; set; }
        public System.Guid StateTaxExternalId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string TaxRegionName { get; set; }
        public Nullable<double> StateRate { get; set; }
        public Nullable<double> EstimatedCombinedRate { get; set; }
        public Nullable<double> EstimatedCountyRate { get; set; }
        public Nullable<double> EstimatedCityRate { get; set; }
        public Nullable<double> EstimatedSpecialRate { get; set; }
        public Nullable<double> RiskLevel { get; set; }
    }
}

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
    
    public partial class ExternalClient
    {
        public long ClientId { get; set; }
        public System.Guid ClientExternalId { get; set; }
        public string ClientName { get; set; }
        public string ClientSecret { get; set; }
        public string ClientUri { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}

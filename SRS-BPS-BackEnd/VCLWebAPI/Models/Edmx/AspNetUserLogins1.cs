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
    
    public partial class AspNetUserLogins1
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string UserId { get; set; }
    
        public virtual AspNetUsers1 AspNetUsers { get; set; }
    }
}

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
    
    public partial class Order_Status
    {
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        public System.DateTime StatusModifiedOn { get; set; }
        public Nullable<int> StatusModifiedBy { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
        public virtual SLU_Status SLU_Status { get; set; }
    }
}

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
    
    public partial class ProductType1
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductType1()
        {
            this.BpsProblem = new HashSet<BpsProblem1>();
        }
    
        public int ProductTypeId { get; set; }
        public string ProductCode { get; set; }
        public string PrettyName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BpsProblem1> BpsProblem { get; set; }
    }
}

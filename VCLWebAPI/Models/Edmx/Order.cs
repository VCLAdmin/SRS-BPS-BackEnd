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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.Order1 = new HashSet<Order>();
            this.Order_Status = new HashSet<Order_Status>();
            this.OrderDetails = new HashSet<OrderDetails>();
        }
    
        public int OrderId { get; set; }
        public System.Guid OrderExternalId { get; set; }
        public int ProjectId { get; set; }
        public int DealerId { get; set; }
        public Nullable<int> ParentOrderId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string Notes { get; set; }
        public int ShippingAddressId { get; set; }
        public double ShippingCost { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }
        public double Discount { get; set; }
        public Nullable<int> DiscountPercentage { get; set; }
        public Nullable<sbyte> ShippingMethod { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual BpsProject BpsProject { get; set; }
        public virtual Dealer Dealer { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order1 { get; set; }
        public virtual Order Order2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Status> Order_Status { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}

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
    
    public partial class BpsBoundaryCondition
    {
        public int BoundaryConditionId { get; set; }
        public System.Guid BoundaryConditionGuid { get; set; }
        public int ProblemId { get; set; }
        public byte[] BoundaryConditionNode { get; set; }
    
        public virtual BpsProblem BpsProblem { get; set; }
    }
}

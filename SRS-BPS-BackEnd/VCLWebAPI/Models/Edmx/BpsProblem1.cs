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
    
    public partial class BpsProblem1
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BpsProblem1()
        {
            this.BpsBoundaryCondition = new HashSet<BpsBoundaryCondition1>();
        }
    
        public int ProblemId { get; set; }
        public System.Guid ProblemGuid { get; set; }
        public int ProjectId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string ProblemName { get; set; }
        public string SystemModel { get; set; }
        public byte[] SystemImage { get; set; }
        public string SystemName { get; set; }
        public string SightlineArticleNumber { get; set; }
        public string IntermediateArticleNumber { get; set; }
        public string GlassConfigurations { get; set; }
        public string WallType { get; set; }
        public Nullable<double> WallHeight { get; set; }
        public Nullable<double> WallWidth { get; set; }
        public Nullable<double> RoomArea { get; set; }
        public Nullable<double> WindLoad { get; set; }
        public string EngineeringStandard { get; set; }
        public Nullable<double> BuildingLength { get; set; }
        public Nullable<double> BuildingWidth { get; set; }
        public string BuildingRiskCategory { get; set; }
        public Nullable<double> WindSpeed { get; set; }
        public string ExposureCategory { get; set; }
        public Nullable<double> WindowWidth { get; set; }
        public Nullable<double> WindowHeight { get; set; }
        public Nullable<double> WindowElevation { get; set; }
        public Nullable<double> RelativeHumidity { get; set; }
        public Nullable<sbyte> PhysicsTypeAcoustic { get; set; }
        public Nullable<sbyte> PhysicsTypeStructure { get; set; }
        public Nullable<sbyte> PhysicsTypeThermal { get; set; }
        public Nullable<int> ProductTypeId { get; set; }
        public string OperabilityConfigurations { get; set; }
        public Nullable<int> WindowZone { get; set; }
        public Nullable<double> BuildingHeight { get; set; }
        public Nullable<int> TerrainCategory { get; set; }
        public string VentFrameArticleNumber { get; set; }
        public string CustomArticles { get; set; }
        public Nullable<int> WindZone { get; set; }
        public string GlazingGasketCombination { get; set; }
        public string Alloys { get; set; }
        public string InsulatingBar { get; set; }
        public Nullable<int> PermissibleDeflection { get; set; }
        public Nullable<int> PermissibleVerticalDeflection { get; set; }
        public string UnifiedObjectModel { get; set; }
        public string AcousticReportUrl { get; set; }
        public string StructuralReportUrl { get; set; }
        public string ThermalReportUrl { get; set; }
        public string AcousticResults { get; set; }
        public string StructuralResults { get; set; }
        public string ThermalResults { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BpsBoundaryCondition1> BpsBoundaryCondition { get; set; }
        public virtual ProductType1 ProductType { get; set; }
        public virtual BpsProject1 BpsProject { get; set; }
    }
}

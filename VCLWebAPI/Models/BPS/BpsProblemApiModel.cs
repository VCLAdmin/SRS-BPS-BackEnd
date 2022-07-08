using System;
using System.Collections.Generic;

namespace VCLWebAPI.Models.BPS
{
    public class BpsProblemApiModel
    {
        public List<BpsBoundaryConditionApiModel> BoundaryConditions { get; set; }
        public string GlassConfigurations { get; set; }
        public Guid ProblemGuid { get; set; }
        public Guid ProjectGuid { get; set; }
        public string ProblemName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectLocation { get; set; }
        public string SystemModel { get; set; }
        public string SystemName { get; set; }
        public string SightlineArticleNumber { get; set; }
        public string IntermediateArticleNumber { get; set; }
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
        public Nullable<int> WindowZone { get; set; }
        public Nullable<double> RelativeHumidity { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public byte[] SystemImage { get; set; }
        public Nullable<sbyte> PhysicsTypeAcoustic { get; set; }
        public Nullable<sbyte> PhysicsTypeStructure { get; set; }
        public Nullable<sbyte> PhysicsTypeThermal { get; set; }
        public string ProductCode { get; set; }
        public int UserId { get; set; }
        public string OperabilityConfigurations { get; set; }
        public Nullable<double> BuildingHeight { get; set; }
        public Nullable<int> TerrainCategory { get; set; }
        public string VentFrameArticleNumber { get; set; }
        public string CustomArticles { get; set; }
        public string Alloys { get; set; }
        public Nullable<int> WindZone { get; set; }
        public string GlazingGasketCombination { get; set; }
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

        public BpsProblemApiModel()
        {
            BoundaryConditions = new List<BpsBoundaryConditionApiModel>();
            GlassConfigurations = String.Empty;
            ProblemGuid = Guid.Empty;
            ProjectGuid = Guid.Empty;
            ProblemName = String.Empty;
            ProjectName = String.Empty;
            ProjectLocation = String.Empty;
            SystemModel = String.Empty;
            SystemName = String.Empty;
            SightlineArticleNumber = String.Empty;
            IntermediateArticleNumber = String.Empty;
            WallType = String.Empty;
            WallHeight = null;
            WallWidth = null;
            RoomArea = null;
            WindLoad = null;
            EngineeringStandard = String.Empty;
            BuildingLength = null;
            BuildingWidth = null;
            BuildingRiskCategory = String.Empty;
            WindSpeed = null;
            ExposureCategory = String.Empty;
            WindowWidth = null;
            WindowHeight = null;
            WindowElevation = null;
            WindowZone = null;
            RelativeHumidity = null;
            SystemImage = null;
            PhysicsTypeAcoustic = null;
            PhysicsTypeStructure = null;
            PhysicsTypeThermal = null;
            ProductCode = String.Empty;
            UserId = 0;
            OperabilityConfigurations = String.Empty;
            BuildingHeight = null;
            TerrainCategory = null;
            VentFrameArticleNumber = String.Empty;
            CustomArticles = String.Empty;
            Alloys = String.Empty;
            WindZone = null;
        }
    }
}
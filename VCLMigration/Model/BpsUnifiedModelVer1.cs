using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCLMigration.Model
{
    public class BpsUnifiedModel_V1
    {
        public UserSetting UserSetting;
        public ProblemSetting ProblemSetting;
        public ModelInput ModelInput;
        public AnalysisResult AnalysisResult;
        public string UnifiedModelVersion;
    }

    public class ProblemSetting
    {
        // for bps only
        public Guid UserGuid;
        public Guid ProjectGuid;
        public Guid ProblemGuid;
        public bool EnableAcoustic;
        public bool EnableStructural;
        public bool EnableThermal;
        // for physics core
        public string ProductType;
        public string FacadeType;
        public string ProjectName;
        public string Location;
        public string ConfigurationName;
        public string UserNotes;
    }

    public class UserSetting
    {
        public string Language;
        public string UserName;
    }

    public class ModelInput
    {
        public FrameSystem FrameSystem;
        public Geometry Geometry;
        public Acoustic Acoustic;
        public Structural Structural;
        public Thermal Thermal;
    }

    public class FrameSystem
    {
        public string SystemType;
        public string UvalueType;
        public string InsulationType;   ////Options: Polythermid Coated Before; Polythermid Anodized Before; Polyamide Coated Before; Polyamide Coated After; Polyamide Anodized Before; Polyamide Anodized After;
        public string InsulatingBarDataNote;
        public string InsulationMaterial;
        public string Alloys;
        public int xNumberOfPanels;
        public int yNumberOfPanels;

        //Remove this for next release. Due to some issue in UI we are using below property
        public string InsulationZone;

        public double VerticalJointWidth;   // for UDC only
        public double HorizontalJointWidth;   // for UDC only
    }

    public class Geometry
    {
        public List<Point> Points;
        public List<Member> Members;
        public List<Glass> GlassList;
        public List<GlazingSystem> GlazingSystems;
        public List<PanelSystem> PanelSystems;
        public List<SlabAnchor> SlabAnchors;
        public List<Reinforcement> Reinforcements;
        public List<SpliceJoint> SpliceJoints;

        public List<Section> Sections;
        public List<FacadeSection> FacadeSections;
        //for bps internal user only
        public List<CustomGlass> CustomGlass;
    }
    public class CustomGlass
    {
        public int customGlassID;
        public string selectedType;
        public string name;
        public string element_xx_1;
        public string element_type_1;
        public string element_size_1;
        public string element_interlayer_1;

        public string element_ins_type_1;
        public string element_ins_size_1;

        public string element_xx_2;
        public string element_type_2;
        public string element_size_2;
        public string element_interlayer_2;

        public string element_ins_type_2;
        public string element_ins_size_2;

        public string element_xx_3;
        public string element_type_3;
        public string element_size_3;
        public string element_interlayer_3;

        public string uValue;
        public string glassrw;
    }

    public class Point
    {
        public int PointID;
        public double X;
        public double Y;
    }

    public class Member
    {
        public int MemberID;
        public int PointA;
        public int PointB;
        public int SectionID;
        public int MemberType;                  // same as SectionType. 1: Outer Frame, 2: Mullion, 3: transom,  4: Facade Major Mullion, 5: Facade Transom, 6 Facade Minor Mullion, 7: Facade Mullion Reinforcement; 
                                                // 21:UDC Top Frame; 22: UDC Vertical;  23: UDC Bottom Frame; 24: UDC Vertical Glazing Bar; 25: UDC Horizontal Glazing Bar;
                                                // for physics core internal use
        public double Length_cm;
        public double TributaryArea;            // mm2
        public double TributaryAreaFactor;      // mm2
        public double Cp;
    }

    public class SlabAnchor
    {
        public int SlabAnchorID;
        public int MemberID;
        public string AnchorType;           // "Fixed" or "Sliding"
        public double Y;
        // for PhysicsCore internal
        public double X;
    }

    public class Reinforcement
    {
        public int ReinforcementID;
        public int MemberID;
        public int SectionID;               // reinforcement section ID
    }

    public class SpliceJoint
    {
        public int SpliceJointID;
        public int MemberID;
        public string JointType;           // "Hinged" or "Ridgid"
        public double Y;
        // for PhysicsCore internal
        public double X;
    }

    public class Glass
    {
        public int GlassID;
        public List<int> BoundingMembers;       // follow this: [left, top, right, bottom]
        public int GlazingSystemID;             // -1 if the type is panel
        public int OperabilitySystemID;
        public string VentArticleName;          // -1 if we don't have any operabiltity for that particular glass
        public double VentInsideW;              // read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public double VentOutsideW;             // read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public double VentDistBetweenIsoBars;   // read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public int PanelSystemID;               // Facade. -1 if the type is glass
        public int JunctionType;                // Facade. Facade Only.
        public string InsertedWindowType;       // Facade Only. Window System Types
        //public string InsertWindowSystem;               // Facade Only
        public string InsertOuterFrameArticleName;         // Facade Only. 
        public double InsertOuterFrameInsideW;             // Facade Only. read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public double InsertOuterFrameOutsideW;            // Facade Only. read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public double InsertOuterFrameDistBetweenIsoBars;  // Facade Only. read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public string InsertUvalueType;                    // Facade Thermal: for inserted window 
        public string InsertInsulationType;                // Facade Thermal: for inserted window, PA or PT
        public string VentOpeningDirection;             // Options: "Inward", "Outward"
        public string VentOperableType;                 // Options: "Tilt-Turn", "Side-Hung",...
        public double BlockDistance;
        // for solver internal use
        public double centerX;
        public double centerY;

        //  For SRS     
        public string RebateGasketArticleName;
        public string CenterGasketInsulationArticleName;
        public string InsideHandleArticleName;
        public string HandleColor;

        //this is for internal use only in Angular UI
        public double InsertOuterFrameDepth;               // Angular User only
        public string InsertWindowSystem;
        public string InsertWindowSystemType;
    }

    public class Plate
    {
        public string Material;
        public Nullable<double> H;
        public string InterMaterial;
        public Nullable<double> InterH;
    }

    public class Cavity
    {
        public string CavityType;   // Acoustic Only
        public double Lz;           // Acoustic Only
    }

    public class GlazingSystem
    {
        public int GlazingSystemID;
        public double Rw;
        public double UValue;
        public int SpacerType;
        public string Description;
        public List<Plate> Plates;
        public List<Cavity> Cavities;
        public string Category;
        public double PSIValue;
    }

    public class PanelSystem            // For Facade Only
    {
        public int PanelSystemID;
        public int PanelID;
        public double Rw;
        public double UValue;
        public int PanelType;             // Use default value 1. Panel Type 1, 2, 3, 4 based on EN ISO 12631. 
        public double Psi;
        public string Description;
        public double Thickness;
        public List<Plate> Plates;
        public List<Cavity> Cavities;
    }

    public class FacadeSection : ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int SectionID;           // = 1,2,3,4 SectionType = SectionID + 3
        public int SectionType;         // same as MemberType. 4: major mullion, 5: transom, 6: minor mullion, 7: reinforcement
                                        // 21:UDC Top Frame; 22: UDC Vertical;  23: UDC Bottom Frame; 24: UDC Vertical Glazing Bar; 25: UDC Horizontal Glazing Bar;
        public string ArticleName;      // For Schuco standard article, use article ID.
        public double OutsideW;         // for display in pdf only
        public double BTDepth;
        public double Width;
        public double Zo;          // depth from central axis to the top
        public double Zu;          // depth from central axis to the bottom
        public double Zl;          // depth from central axis to the mullion left
        public double Zr;          // depth from central axis to the mullion right
        public double A;           //Area
        public string Material;        // Aluminum or Steel
        public double beta;           // MPa; depending on Alloy
        public double Weight;      //section weight kg/m
        public double Iyy;         //Moment of inertia for bending about the y-axis, out-of-plane
        public double Izz;           //Moment of inertia for bending about the z-axis, in-plane
        public double Wyy;          // min(Iyy / Zo, Izz/ Zu)
        public double Wzz;          // min(Izz / Zyl, Izz/ Zyr)
        public double Asy;         //Shear area in y-direction
        public double Asz;         //Shear area in z-direction
        public double J;           //Torsional constant 
        public double E;            //Young's modulus
        public double G;            //Torsional shear modulus
        public double EA;                   // Derived EA
        public double GAsy;                 // Derived GA
        public double GAsz;                 // Derived GA
        public double EIy;                  // Derived EIy
        public double EIz;                  // Derived EIz
        public double GJ;                   // Derived GJ
        // sectional properties for display only
        public double Ys = 0;           // Shear center y coordinate
        public double Zs = 0;           // Shear center z coordinate
        public double Ry = 0;           // Radius of Gyration about the y-axis
        public double Rz = 0;           // Radius of Gyration about the z-axis
        public double Wyp = 0;          // Section Modulus about the positive semi-axis of the y-axis 
        public double Wyn = 0;          // Section Modulus about the negative semi-axis of the y-axis 
        public double Wzp = 0;          // Section Modulus about the positive semi-axis of the z-axis 
        public double Wzn = 0;          // Section Modulus about the negative semi-axis of the z-axis 
        public double Cw = 0;
        public double Beta_torsion = 0;
        public double Zy = 0;           // Plastic Property 
        public double Zz = 0;           // Plastic Property 
        public double Depth;

        // Reinforcement properties, for physics core internal only
        public double ReinforcementEIy;
        public double ReinforcementEIz;
        public double ReinforcementWeight;
    }

    public class Section
    {
        public int SectionID;           // = 1,2,3 , SectionID and SectionType is same
        public int SectionType;        // same as MemberType. 1: Outer Frame, 2: Mullion, 3: transom
        public string ArticleName;     // article name
        public bool isCustomProfile;
        public double InsideW;
        public double OutsideW;
        public double LeftRebate;
        public double RightRebate;
        public double DistBetweenIsoBars;
        public double d;                    // mm.  article parameters for strucutural analysis. from d to alpha. 
        public double Weight;               // N/m
        public double Ao;                   // mm2
        public double Au;                   // mm2
        public double Io;                   // mm4
        public double Iu;                   // mm4
        public double Ioyy;                 // mm4
        public double Iuyy;                 // mm4
        public double Zoo;                  // mm
        public double Zuo;                  // mm
        public double Zou;                  // mm
        public double Zuu;                  // mm
        public double Zol;                  // mm
        public double Zul;                  // mm
        public double Zor;                  // mm
        public double Zur;                  // mm
        public double RSn20;                // N/m  
        public double RSp80;
        public double RTn20;
        public double RTp80;
        public double Cn20;                 // N/mm2  
        public double Cp20;
        public double Cp80;
        public double beta;                   // MPa; depending on Alloy
        public double gammaM;                 // Optional, Partial factor for material property
        public double A2;                     // Optional, for future use
        public double E;                      // Optional, for future use
        public double alpha;                  // Optional, for future use
    }

    public class Acoustic
    {
        public int WallType;
        public double Height;
        public double Width;
        public double RoomArea;
    }

    public class Structural
    {
        public int DispIndexType;                   // 1 - User Define, 2 - DIN EN 14351-1-2016 Class B, 3 - DIN EN 14351-1-2016 Class C
                                                    // 4 - DIN EN 13830:2003, 5 - DIN EN 13830:2015/2020, 6 - US Standard
        public int DispHorizontalIndex;             // user defined displacement index, when DispIndexType =3.  otherwise, backend will calculate based on design code;
        public int DispVerticalIndex;               // user defined displacement index, when DispIndexType =3.  otherwise, backend will calculate based on design code;
        public int WindLoadInputType;               // 1 - user defined, 2 - DIN code
        public DinWindLoadInput dinWindLoadInput;   // input for wind load calculation, ignored when WindLoadInputType =1;
        public double WindLoad;                     // wind pressure, user defined or calculated when WindLoadInputType =2;
        public double Cpp;                          // user positive Cp 
        public double Cpn;                          // user negative Cp
        public double HorizontalLiveLoad;           // kN/m
        public double HorizontalLiveLoadHeight;     // mm                    
        public LoadFactor LoadFactor;
        public SeasonFactor SeasonFactor;
        public TempChange TemperatureChange;
        // for physicscore internal use
        public double Cp;                           // user Cp
    }


    public class DinWindLoadInput
    {
        public int WindZone;
        public int TerrainCategory;
        public double L0;                   // 
        public double B0;
        public double h;
        public double ElvW;
        public int WindowZone;              // 1 edge zone; 2 middle zone
        public bool IncludeCpi;             // whether include Cpi when calculate Cp
    }

    public class LoadFactor
    {
        public double DeadLoadFactor;
        public double WindLoadFactor;
        public double HorizontalLiveLoadFactor;
        public double TemperatureLoadFactor;
    }

    public class SeasonFactor
    {
        public double SummerFactor;
        public double WinterFactor;
    }

    public class TempChange
    {
        public double Summer;
        public double Winter;
    }


    public class Thermal
    {
        public double RelativeHumidity;
        public string InsulationZone; // Facade Only, SI, HI, SI GREEN
    }

    public class LossDistributionPoint
    {
        public double Frequency { get; set; }
        public double Tau { get; set; }
        public double STL { get; set; }
    }

    public class Classification
    {
        public int STC { get; set; }
        public int OITC { get; set; }
        public int Rw { get; set; }
        public int C { get; set; }
        public int Ctr { get; set; }
        public int[] NC { get; set; }
        public int[] Deficiencies { get; set; }
    }

    public class AnalysisResult
    {
        public AcousticResult AcousticResult;
        public StructuralResult StructuralResult;
        public FacadeStructuralResult FacadeStructuralResult;
        public UDCStructuralResult UDCStructuralResult;
        public ThermalResult ThermalResult;
    }

    public class AcousticResult
    {
        public AcousticOutput AcousticUIOutput { get; set; }
        public string reportFileUrl { get; set; }
    }

    public class AcousticOutput
    {
        public Classification classification { get; set; }
        public List<LossDistributionPoint[]> LossDistributions { get; set; }
        public double TotalRw { get; set; }
    }

    public class StructuralResult
    {
        public List<StructuralMemberResult> MemberResults { get; set; }
        public string reportFileUrl { get; set; }
        public string summaryFileUrl { get; set; }
        public string errorMessage { get; set; }
    }

    public class FacadeStructuralResult
    {
        public List<FacadeStructuralMemberResult> MemberResults { get; set; }
        public string reportFileUrl { get; set; }
        public string summaryFileUrl { get; set; }
        public string errorMessage { get; set; }
    }

    public class UDCStructuralResult
    {
        public List<UDCStructuralMemberResult> MemberResults { get; set; }
        public string reportFileUrl { get; set; }
        public string summaryFileUrl { get; set; }
    }

    public class StructuralMemberResult
    {
        public int memberID { get; set; }
        public double deflectionRatio { get; set; }
        public double verticalDeflectionRatio { get; set; }
        public double stressRatio { get; set; }
        public double shearRatio { get; set; }
    }

    public class FacadeStructuralMemberResult
    {
        public int memberID { get; set; }
        public double outofplaneBendingCapacityRatio { get; set; }
        public double outofplaneReinfBendingCapacityRatio { get; set; }
        public double inplaneBendingCapacityRatio { get; set; }
        public double outofplaneDeflectionCapacityRatio { get; set; }
        public double inplaneDeflectionCapacityRatio { get; set; }
    }

    public class UDCStructuralMemberResult
    {
        public int memberID { get; set; }
        public double outofplaneBendingCapacityRatio { get; set; }
        public double inplaneBendingCapacityRatio { get; set; }
        public double outofplaneDeflectionCapacityRatio { get; set; }
        public double inplaneDeflectionCapacityRatio { get; set; }
    }

    public class ThermalResult
    {
        public ThermalOutput ThermalUIResult { get; set; }
        public string reportFileUrl { get; set; }
    }

    public class ThermalOutput
    {
        public class ThermalFrame
        {
            public List<FrameSegment> FrameSegs { get; set; }
            public double Area { get; set; }
            public double Uf { get; set; }
            public string UfNote { get; set; }
            public int ThermalFrameID { get; set; }
        }

        public class FrameSegment
        {
            public int FrameSegID { get; set; }
            public Point PointA { get; set; }
            public Point PointB { get; set; }
            public string ArticleCombo { get; set; }
        }

        public class ThermalFacadeMember
        {
            public List<FrameSegment> FrameSegs { get; set; }
            public string ArticleID { get; set; }
            public double Area { get; set; }
            public double Uf { get; set; }
            public double HeatLoss { get; set; }
            public int FacadeFrameID { get; set; }
            public double Width { get; set; }
        }

        public class ThermalGlass
        {
            public int GlassID { get; set; }
            public double Ug { get; set; }
            public double Area { get; set; }
        }

        public class ThermalUIGlass
        {
            public List<int> GlassID { get; set; }
            public double Ug { get; set; }
            public double Area { get; set; }
        }

        public class ThermalPanel
        {
            public int GlassID { get; set; }
            public double Up { get; set; }
            public double Area { get; set; }
        }

        public class ThermalUIPanel
        {
            public List<int> GlassID { get; set; }
            public double Up { get; set; }
            public double Area { get; set; }
        }

        public class ThermalGlassEdge
        {
            public int GlassID { get; set; }
            public double Psi { get; set; }
            public double Length { get; set; }
        }

        public class ThermalUIGlassEdge
        {
            public List<int> GlassID { get; set; }
            public double Psi { get; set; }
            public double Length { get; set; }
        }

        public class ThermalUIFacadeGlassEdge
        {
            public List<int> GlassID { get; set; }
            public double PsiH { get; set; }
            public double PsiV { get; set; }
            public double LengthH { get; set; }
            public double LengthV { get; set; }
        }

        public class ThermalPanelEdge
        {
            public int GlassID { get; set; }
            public string PanelDiscript { get; set; }
            public double Psi { get; set; }
            public double Length { get; set; }
            public double HeatLoss { get; set; }
        }

        public class ThermalUIPanelEdge
        {
            public List<int> GlassID { get; set; }
            public double Psi { get; set; }
            public double Length { get; set; }
        }

        public class ThermalUIInsertUnitGlassEdge
        {
            public int GlassID { get; set; }
            public double Psi { get; set; }
            public double Length { get; set; }
        }

        public class ThermalUIInsertUnitFrameEdge
        {
            public List<int> GlassID { get; set; }
            public double Psi { get; set; }
            public double Length { get; set; }
        }

        public class ThermalInsertUnitFrame
        {
            public int GlassID { get; set; }
            public string ArticleIDCombo { get; set; }
            public double Uf { get; set; }
            public double Area { get; set; }
        }

        public class GlassGeometricInfo
        {
            public int GlassID { get; set; }
            public double[] PointCoordinates { get; set; }
            public double[] CornerCoordinates { get; set; }
            public double[] VentCoordinates { get; set; }
            public double[] InsertOuterFrameCoordinates { get; set; }
            public string VentOpeningDirection { get; set; }
            public string VentOperableType { get; set; }
        }

        public List<ThermalFrame> ThermalFrames { get; set; } // Window
        public List<ThermalFacadeMember> ThermalFacadeMembers { get; set; } // Facade
        public List<ThermalUIFacadeGlassEdge> ThermalUIFacadeGlassEdges { get; set; } //Facade, insert unit not included

        public List<ThermalUIGlass> ThermalUIGlasses { get; set; } // Used by Window & Facade. If facade, insert unit not included
        public List<ThermalUIGlassEdge> ThermalUIGlassEdges { get; set; } // Window
        public List<ThermalUIPanel> ThermalUIPanels { get; set; } //Facade, insert unit not included
        public List<ThermalUIPanelEdge> ThermalUIPanelEdges { get; set; } //Facade, insert unit not included

        public List<ThermalUIInsertUnitFrameEdge> ThermalUIInsertUnitFrameEdges { get; set; }

        public List<ThermalGlass> ThermalUIInsertUnitGlasses { get; set; }
        public List<ThermalPanel> ThermalUIInsertUnitPanels { get; set; }
        public List<ThermalGlassEdge> ThermalUIInsertUnitGlassEdges { get; set; }
        public List<ThermalPanelEdge> ThermalUIInsertUnitPanelEdges { get; set; }
        public List<ThermalInsertUnitFrame> ThermalUIInsertUnitFrames { get; set; }

        public List<GlassGeometricInfo> GlassGeometricInfos { get; set; }

        public double TotalArea { get; set; }
        public double TotalUw { get; set; }
    }

    public class DXFInput
    {
        public string fileName;
        public string content;
    }

    public class ProjectInfo
    {
        public Guid ProjectGuid;
        public string ProjectName;
        public string Location;
    }
}

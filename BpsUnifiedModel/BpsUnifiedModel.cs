using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BpsUnifiedModelLib
{
    public class BpsUnifiedModel
    {
        // for physics core
        public UserSetting UserSetting;
        public ProblemSetting ProblemSetting;
        public SRSProblemSetting SRSProblemSetting;
        public string UnifiedModelVersion;              //added 2021.08.31
        public ModelInput ModelInput;
        public AnalysisResult AnalysisResult;

        //for insert unit only - ANGULAR
        public CollapsedPanelStatus CollapsedPanels;
    }

    public class CollapsedPanelStatus
    {
        public bool Panel_Configure;
        public bool Panel_Operability;
        public bool Panel_Framing;
        public bool Panel_Glass;
        public bool Panel_Acoustic;
        public bool Panel_Structural;
        public bool Panel_Thermal;
        public bool Panel_Load;
        public bool Panel_SlidingUnit;
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
        // for SRS
        public string Client;
        public string ProjectNumber;
        public string LastModifiedDate;
        public List<string> DrawingNames;
        // for BPS and SRS
        public string SlidingDoorType;      //Added 2022.03.21, moved 2022.03.23, can be "Classic", "Panorama", or "Bi-fold"
    }

    public class SRSProblemSetting
    {
        public bool isOrderPlaced;          // UnifiedInputVersion2.0. For SRS.
        public string OrderNumber;          // UnifiedInputVersion2.0. For SRS.
        public double SubTotal;             // UnifiedInputVersion2.0. For SRS.
        public double Quantity;             // UnifiedInputVersion2.0. For SRS.
        public string Client;               // UnifiedInputVersion2.0. For SRS.
        public string ProjectNumber;        // UnifiedInputVersion2.0. For SRS.
        public string LastModifiedDate;     // UnifiedInputVersion2.0. For SRS.
        public List<string> DrawingNames;         // UnifiedInputVersion2.0. For SRS.
        public bool QuickCheckPassed;
    }

    public class UserSetting
    {
        public string Language;
        public string UserName;
        public string ApplicationType;      //added 2021.08.31
    }

    public class ModelInput
    {
        public FrameSystem FrameSystem;
        public Geometry Geometry;
        public Acoustic Acoustic;
        public Structural Structural;
        public Thermal Thermal;
        public SRSExtendedData SRSExtendedData;
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

        // for facade only
        public double MajorMullionTopRecess;            //UnifiedInputVersion2.0. For SRS.
        public double MajorMullionBottomRecess;            //UnifiedInputVersion2.0. For SRS.

        // for UDC only
        public double VerticalJointWidth;
        public double HorizontalJointWidth;

        // for SRS
        public string AluminumFinish;
        public string AluminumColor;

        //Remove this for next release. Due to some issue in UI we are using below property
        public string InsulationZone;
    }

    public class Geometry
    {
        public List<Point> Points;
        public List<Member> Members;
        public List<Infill> Infills;
        public List<GlazingSystem> GlazingSystems;
        public List<PanelSystem> PanelSystems;
        public List<OperabilitySystem> OperabilitySystems;
        public List<DoorSystem> DoorSystems;
        public List<SlidingDoorSystem> SlidingDoorSystems;  //Added 2022.02.01

        public List<SlabAnchor> SlabAnchors;
        public List<SpliceJoint> SpliceJoints;
        public List<Reinforcement> Reinforcements;

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
        public int MemberID;                    //ID number assigned in order of member creation (first member is ID 1, second is ID 2, etc.)
        public int PointA;
        public int PointB;
        public int SectionID;                   //ID number of corresponding Section
        public int MemberType;                  // same as SectionType. 1: Outer Frame, 2: Mullion, 3: transom,  4: Facade Major Mullion, 5: Facade Transom, 6 Facade Minor Mullion, 7: Facade Mullion Reinforcement; 
                                                // 21:UDC Top Frame; 22: UDC Vertical;  23: UDC Bottom Frame; 24: UDC Vertical Glazing Bar; 25: UDC Horizontal Glazing Bar; 31: Door Threshold; 33: Door Sidelight Sill;
                                                // 41: Sliding Door Outer Frame; 42: Interlock Vent Frame (will match Type 43 in SRS 3.0); 43: Sliding Door Vent Frame; 45: Sliding Door Bottom Outer Frame;
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
    }

    public class Infill
    {
        public int InfillID;
        public List<int> BoundingMembers;       // follow this: [left, top, right, bottom]
        public int GlazingSystemID;             // -1 if the type is panel;
        public int PanelSystemID;               // Facade. -1 if the type is glass;
        public int OperabilitySystemID;         // UnifiedInputVersion2.0;  -1 if no vent or door;
        public double BlockDistance;

        //for SRS
        public double VentWeight;
        public int LockingPointOption;
        public int HandlePosition;

        //this is for internal use only in Angular UI
        public double InsertOuterFrameDepth;               // Angular User only
        public string InsertWindowSystem;                  // Angular User only
        public string InsertWindowSystemType;              // Angular User only

        public string GlazingBeadProfileArticleName;        // UnifiedInputVersion2.0. For SRS.
        public string GlazingGasketArticleName;             // UnifiedInputVersion2.0. For SRS.
        public string GlazingRebateGasketArticleName;       // UnifiedInputVersion2.0. For SRS.
        public string GlazingRebateInsulationArticleName;   // UnifiedInputVersion2.0. For SRS.
    }

    public class OperabilitySystem
    {
        public int OperabilitySystemID;
        public int DoorSystemID;                            // UnifiedInputVersion2.0, -1 if it's not a door
        public int SlidingDoorSystemID;                     // Added 2022.02.01
        public string VentArticleName;                      // -1 if it's not a vent
        public double VentInsideW;                          // read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public double VentOutsideW;                         // read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public double VentDistBetweenIsoBars;               // read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public int JunctionType;                            // Facade. Facade Only.
        public string InsertedWindowType;                   // Facade Only. Window System Types
        public string InsertOuterFrameArticleName;          // Facade Only. 
        public double InsertOuterFrameInsideW;              // Facade Only. read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public double InsertOuterFrameOutsideW;             // Facade Only. read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public double InsertOuterFrameDistBetweenIsoBars;   // Facade Only. read from database Article table; -1 if we don't have any operabiltity for that particular glass
        public string InsertUvalueType;                     // Facade Thermal: for inserted window 
        public string InsertInsulationType;                 // Facade Thermal: for inserted window, PA or PT
        public string InsertInsulationTypeName;             // Added 2022.06.30 Angular use only
        public string VentOpeningDirection;                 // Options: "Inward", "Outward"
        public string VentOperableType;                     // NOTE : Please update all "Tilt-Turn" to proper name "Turn-Tilt"
                                                            // NOTE : Left/Right indicates position when viewed from interior. So "Side-Hung-Left" has hinges on left side from interior. "Double-Door-Active-Right" has active vent on right side when viewed from interior
                                                            // Options: "Turn-Tilt-Left"; "Turn-Tilt-Right"; "Side-Hung-Left"; "Side-Hung-Right"; "Bottom-Hung-Left"; "Bottom-Hung-Right"; "Bottom-Hung-Top"; "Top-Hung-Left"; "Top-Hung-Right";
                                                            // Options (continued): "Parallel-Left"; "Parallel-Right"; "Single-Door-Left"; "Single-Door-Right"; "Double-Door-Active-Left"; "Double-Door-Active-Right";
                                                            // Options (continued): SLIDING DOOR OPTIONS FOR BPS 4.0 / SRS 3.0: For sliding doors, left/right indicate sliding vent on inner track location when viewed from interior.
                                                            // Options (continued): DOUBLE TRACK: "SlidingDoor-Type-2A-Left"; "SlidingDoor-Type-2A-Right"; "SlidingDoor-Type-2A1.i-Left"; "SlidingDoor-Type-2A1.i-Right"; "SlidingDoor-Type-2D1.i"; 
                                                            // Options (continued): TRIPLE TEACK: "SlidingDoor-Type-3E-Left"; "SlidingDoor-Type-3E-Right"; "SlidingDoor-Type-3E1-Left"; "SlidingDoor-Type-3E1-Right";"SlidingDoor-Type-3F";

        //  For SRS     
        public string RebateGasketArticleName;
        public string CenterGasketInsulationArticleName;
        public string InsideHandleArticleName;
        public string InsideHandleArticleDescription;       // Added 2022.06.30 Angular use only
        public string InsideHandleColor;                    //renamed from "HandleColor" 2021.11.02                        

        //this is for internal use only in Angular UI
        public double InsertOuterFrameDepth;               // Angular User only
        public string InsertWindowSystem;

        public int PickerIndex;                            // Angular Use only
    }

    public class DoorSystem
    {
        public int DoorSystemID;
        public string DoorSystemType;
        public string DoorSillArticleName;
        public string DoorLeafArticleName;
        public string DoorPassiveJambArticleName;       //renamed from "DoubleVentSecondaryJambArticleName" on 2021.08.31
        public string DoorThresholdArticleName;
        public double DoorSillInsideW;                  //added 2021.08.31
        public double DoorSillOutsideW;                 //added 2021.08.31
        public double DoorLeafInsideW;                  //added 2021.08.31
        public double DoorLeafOutsideW;                 //added 2021.08.31
        public double DoorPassiveJambInsideW;           //added 2021.08.31
        public double DoorPassiveJambOutsideW;           //added 2021.08.31
        public string DoorSidelightSillArticleName;      //renamed from "DoorSideLiteSillArticleName" on 2021.08.31
        public string OutsideHandleArticleName;
        public string OutsideHandleColor;               //added 2021.11.02
        public string OutsideHandleArticleDescription;  // Added 2022.06.30 Angular Use only
        public string InsideHandleArticleName;
        public string InsideHandleColor;                //added 2021.11.02
        public string InsideHandleArticleDescription;   // Added 2022.06.30 Angular Use only
        //public string HingeCount;                     //omitted 2021.11.18
        public int HingeCondition;                      //added 2021.11.18 to replace HingeCount. 0 = error (not valid door configuration), 1 = two hinges (one at top, one at bottom), 2 = three hinges (one at top, one at bottom, one at center), 3 = three hinges (one at top, one at bottom, and one near top), 4 = four hinges (one at top, one at bottom, one at center, and one near top).
        public string HingeArticleName;                 //added 2021.11.02
        public string HingeArticleDescription;          // Added 2022.06.30 Angular Use only
        public string HingeColor;                       //added 2021.11.02
        //public string DoorOpeningDirection;                 // Options: "Inward", "Outward" (omitted 2021.10.19)
        //public string DoorOperableType;                     // Options: "Single", "Double"  (omitted 2021.10.19)
    }

    public class SlidingDoorSystem  //Added 2022.02.01
    {
        public int SlidingDoorSystemID;             //Added 2022.02.01
        public string InsideHandleArticleName;      //Added 2022.02.01
        public string InsideHandleColor;            //Added 2022.02.01
        public string OutsideHandleArticleName;     //Added 2022.02.01
        public string OutsideHandleColor;           //Added 2022.02.01
        public string SlidingDoorSystemType;        //Added 2022.02.01, for BPS 4.0/SRS 3.0 will be either ASE 60 or ASE 80.HI
        public string SlidingOperabilityType;       //Added 2022.02.01, can be "Lift-and-slide" or "Sliding", but for BPS 4.0/SRS 3.0 it will only be "Lift-and-slide"
        public int SlidingVentSectionID;            //Added 2022.02.01
        public int SlidingVentInterlockSectionID;   //Added 2022.02.01
        public bool InterlockReinforcement;         //Added 2022.02.01, user sets as true or false to toggle external reinforcement on interlocks
        public string SteelTubeArticleName;         //Added 2022.02.22, if InterlockReinforcement is set to 'true,' this string will list article number for said reinforcement. Otherwise, string is null.
        public string StructuralProfileArticleName; //Added 2022.02.22, this article is not assigned by user, but is rather required with double spilt ASE vent profiles (the kind used in SRS 3.0 release). 
        public string DoubleVentArticleName;        //Added 2022.02.22, this article is not assigned by user, but is required for Type 2D/1.i in SRS 3.0 release, as well as Type 3F in BPS 4.0 release (i.e. any Type that has a double vent interlock at middle)
        public string MovingVent;                   //Added 2022.03.14, string to choose whether moving vent is on inside/outside/inside and outside
        public List<VentFrame> VentFrames;          //Added 2022.02.01, list vents based on outside view, left to right
    }

    public class VentFrame  //Added 2022.02.01
    {
        public int VentFrameID;     //Added 2022.02.01
        public double Width;        //Added 2022.02.01
        public string Type;         //Added 2022.02.01, can be either "Fixed" or "Sliding" depending on Sliding Door configuration
        public int Track;           //Added 2022.02.01, Tracks counted 1 up to 3 depending on configuration, with 1 most outward and 3 most inward track
    }

    public class Plate
    {
        public string Material;
        public double H;
        public string InterMaterial;
        public double InterH;
        public string UDF1;         // Added 2022.06.30 
    }
    public class Cavity
    {
        public string CavityType;   // Acoustic Only
        public double Lz;           // Acoustic Only
    }

    public class GlazingSystem
    {
        public string Manufacturer;
        public string BrandName;
        public int GlazingSystemID;
        public string Color;            // UnifiedInputVersion2.0
        public double Rw;
        public double RwC;              // UnifiedInputVersion2.0
        public double RwCtr;            // UnifiedInputVersion2.0
        public double STC;              // UnifiedInputVersion2.0
        public double OITC;             // UnifiedInputVersion2.0
        public double UValue;
        public double SHGC;             // UnifiedInputVersion2.0
        public double VT;               // UnifiedInputVersion2.0
        public int SpacerType;
        public string Description;
        public List<Plate> Plates;
        public List<Cavity> Cavities;
        public string Category;
        public double PSIValue;
        public double Thickness;        //added 2021.11.02, just should list total IGU thickness in mm (sum glass and spacer heights)
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

    public class FacadeSection
    {
        public int SectionID;           // = 1,2,3,4 SectionType = SectionID + 3
        public int SectionType;         // same as MemberType. 4: major mullion, 5: transom, 6: minor mullion, 7: reinforcement
                                        // 21:UDC Top Frame; 22: UDC Vertical Frame;  23: UDC Bottom Frame; 24: UDC Vertical Glazing Bar; 25: UDC Horizontal Glazing Bar;
        public string ArticleName;      // For Schuco standard article, use article ID.
        public bool isCustomProfile;   // is custom profile
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

        //this is for internal use only in Angular UI
        public double Depth;
    }

    public class Section
    {
        public int SectionID;           // = 1,2,3 , SectionID and SectionType is same
        public int SectionType;        // same as MemberType. 1: Outer Frame, 2: Mullion, 3: transom,  4: Facade Major Mullion, 5: Facade Transom, 6 Facade Minor Mullion, 7: Facade Mullion Reinforcement; 
                                       // 21:UDC Top Frame; 22: UDC Vertical;  23: UDC Bottom Frame; 24: UDC Vertical Glazing Bar; 25: UDC Horizontal Glazing Bar; 31: Door Threshold; 33: Door Sidelight Sill;
                                       // 41: Sliding Door Outer Frame; 42: Interlock Vent Frame (will match Type 43 in SRS 3.0); 43: Sliding Door Vent Frame; 45: Sliding Door Bottom Outer Frame;
        public string ArticleName;     // article name
        public bool isCustomProfile;   // is custom profile
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
        public double Zou;                  // mm
        public double Zol;                  // mm
        public double Zor;                  // mm
        public double Zuo;                  // mm
        public double Zuu;                  // mm
        public double Zul;                  // mm
        public double Zur;                  // mm

        public double RSn20;                // N/m  
        public double RSp80;
        public double RTn20;
        public double RTp80;
        public double Cn20;                 // N/mm2  
        public double Cp20;
        public double Cp80;
        public double beta;                   // MPa; depending on Alloy
        public double A2;                     // Optional, for future use
        public double E;                      // Optional, for future use
        public double alpha;                  // Optional, for future use

        public double Woyp = 0;          // Section Modulus about the positive semi-axis of the y-axis 
        public double Woyn = 0;          // Section Modulus about the negative semi-axis of the y-axis 
        public double Wozp = 0;          // Section Modulus about the positive semi-axis of the z-axis 
        public double Wozn = 0;          // Section Modulus about the negative semi-axis of the z-axis 
        public double Wuyp = 0;          // Section Modulus about the positive semi-axis of the y-axis 
        public double Wuyn = 0;          // Section Modulus about the negative semi-axis of the y-axis 
        public double Wuzp = 0;          // Section Modulus about the positive semi-axis of the z-axis 
        public double Wuzn = 0;          // Section Modulus about the negative semi-axis of the z-axis 

        public double gammaM; //Optional, Partial factor for material property (added 2021.10.20)

        //this is for internal use only in Angular UI
        public double Depth;

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
        public int DispIndexType;                   // 1 - User Define, 2 - DIN EN 14351-1-2016 CL.B L/200, 3 - DIN EN 14351-1-2016 CL.C L/300
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

        // for internal use
        public bool ShowBoundaryCondition;

        public bool ShowWindPressure;
        public string PositiveWindPressure;
        public string NegativeWindPressure;

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
        public double pCpi;                 // user positive Cpi, added 2021.09.03
        public double nCpi;                 // user negative Cpi, added 2021.09.03

        // for internal use
        public bool RequestDescription;
        // for internal use angular
        public string PostCodeValue;
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

    public class SRSExtendedData // UnifiedInputVersion2.0. For SRS.
    {
        public List<Hardware> Hardwares { get; set; }       // UnifiedInputVersion2.0
        public Machining MachiningInfo { get; set; }    // UnifiedInputVersion2.0
    }

    public class Hardware // UnifiedInputVersion2.0
    {
        public int HardwareID;
        public string HardwareAlloy;
        public double HardwareFy;
        public double HardwareFu;
        public string HardwareFinishes;
    }

    public class Machining // UnifiedInputVersion2.0
    {
        public double GlueHoleOffsetsfromLeftTopCorner;
        public double NailHoleOffsetsfromLeftTopCorner;
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
        public double windLoadCapacity { get; set; }        //added 2021.10.20
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
        public double windLoadCapacity { get; set; }        //added 2021.10.20
    }

    public class FacadeStructuralMemberResult
    {
        public int memberID { get; set; }
        public double outofplaneBendingCapacityRatio { get; set; }
        public double outofplaneReinfBendingCapacityRatio { get; set; }
        public double inplaneBendingCapacityRatio { get; set; }
        public double outofplaneDeflectionCapacityRatio { get; set; }
        public double inplaneDeflectionCapacityRatio { get; set; }
        public double combinedStressRatio { get; set; }         //added 2021.10.20
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

        public class ThermalSDFrameSegment // SlidingDoor Systems for Thermal Solver, added 2022.03.02
        {
            public List<string> ArticleCombo { get; set; }
            public double Area { get; set; }
            public double Uf { get; set; }
            public string Key { get; set; }
            public double HeatLoss { get; set; }
            public double Width { get; set; }
            public Point PointA { get; set; }
            public Point PointB { get; set; }
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

        public class ThermalSlidingDoorFrame // SlidingDoor Systems for Thermal Solver, added 2022.03.02
        {
            public List<ThermalSDFrameSegment> SDFrameSegments { get; set; }
            public int ThermalFrameID { get; set; }
            public double Area { get; set; }
            public double Uf { get; set; }
            public string Key { get; set; }
            public int SlidingDoorFrameID { get; set; }
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
        public List<ThermalSlidingDoorFrame> ThermalSlidingDoorFrames { get; set; } // SlidingDoor Systems for Thermal Solver, added 2022.03.02
        public List<ThermalUIFacadeGlassEdge> ThermalUIFacadeGlassEdges { get; set; } //Facade, insert unit not included

        public List<ThermalGlass> ThermalGlasses { get; set; } // Used for SlidingDoor Systems for Thermal Solver, added 2022.03.02
        public List<ThermalGlassEdge> ThermalGlassEdges { get; set; } // Used for SlidingDoor Systems for Thermal Solver, added 2022.03.02
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
        public string Line1;
        public string Line2;
        public string Country;
        public string State;
        public string County;
        public string City;
        public string PostalCode;
        public string AdditionalDetails;
        public string AddressType;
        public Nullable<bool> Active;
        public Nullable<int> ProjectId;
        public Nullable<decimal> Latitude;
        public Nullable<decimal> Longitude;
    }
}

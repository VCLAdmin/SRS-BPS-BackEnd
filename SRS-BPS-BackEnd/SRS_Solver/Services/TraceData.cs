using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading.Tasks;


namespace SRS_Solver.Services
{
    public class TraceData
    {
        public TraceData()     //Project Data (is there a reason this is not its own class?
        {
            this.Project = new ProjectInfo();
            this.Glazing = new GlazingInfo();
            this.Framing = new FramingInfo();
            this.Vents = new List<Vent>();
            this.CutList = new CuttingInfo();
            this.SectionList = new SectionInfo();
            this.Specs = new Specifications();
        }
        public ProjectInfo Project { get; set; }
        public GlazingInfo Glazing { get; set; }
        public FramingInfo Framing { get; set; }
        public CuttingInfo CutList { get; set; }
        public SectionInfo SectionList { get; set; }
        public List<Vent> Vents { get; set; }
        public Specifications Specs { get; set; }

        //------------------------------

        /// <summary>
        /// Cover Page and Title Block Info
        /// </summary>
        public class ProjectInfo
        {

            public double Scale { get; set; }           //Page scale should be uniform to fit largest drawing on a page, 1:1, 1:2, etc.
            public string Client { get; set; }          //Set in SRS
            public string Name { get; set; }            //Set in SRS, Project Name
            public string Location { get; set; }        //Set in SRS, Street Address/Zip Code/Country, etc.
            public string Number { get; set; }          //Set in SRS? What is difference from order number?
            public string OrderNumber { get; set; }     //See above
            public string Alloy { get; set; }           //Alu Alloy, set in SRS
            public string Configuration { get; set; }   //Title Page Name, not sure how this differs from Name

            public string Finish { get; set; }           //Alu finish (e.g. anodized)

            public string Color { get; set; }            //Alu color

            public string VentOperableType { get; set; } //Vent Operable type taken from OperabilitySystems

        }

        /// <summary>
        /// Glass information
        /// </summary>
        public class GlazingInfo
        {

            public GlazingInfo()
            {
                Panes = new List<Part>();       //Set in SRS Glazing config
                Spacers = new List<Part>();     //Set in SRS Glazing config
                Beads = new List<Part>();       //Glazing beads determined by glass thickness
                TakeOff = new List<Part>();     //Estimation of material needed for project
            }

            //Following are as above or set in SRS config/properties of the glazing

            public List<Part> Panes { get; set; }
            public List<Part> Spacers { get; set; }
            public List<Part> Beads { get; set; }
            public List<Part> TakeOff { get; set; }

            public string Manufacturer { get; set; }
            public string Makeup { get; set; }
            public string SpacerBrand { get; set; }
            public string BrandName { get; set; }
            public string Decription { get; set; }
            public double U_value { get; set; }
            public double SHGC { get; set; }
            public double STC { get; set; }
            public double OITC { get; set; }
            public bool DoubleGlazing { get; set; } //set true for double glazing, false for triple glazing (assume SRS sizes for choosing glazing beads)


        }

        /// <summary>
        /// Extrusion information
        /// </summary>
        public class FramingInfo
        {

            public FramingInfo()
            {
                Extrusions = new List<Part>();
            }

            public List<Part> Extrusions { get; set; }


        }

        /// <summary>
        /// Cut list for BOM
        /// </summary>
        public class CuttingInfo
        {

            public CuttingInfo()
            {
                Cuts = new List<Cut>();
            }

            public List<Cut> Cuts { get; set; }


        }

        /// <summary>
        /// Information for elevation
        /// </summary>
        public class SectionInfo
        {
            public SectionInfo()
            {
                Sections = new List<Section>();
            }
            public List<Section> Sections { get; set; }

        }

        /// <summary>
        /// Physics spec set in SRS, info from unified model
        /// </summary>
        public class Specifications
        {
            public double WindPressure { get; set; }
            public double LateralLiveLoad { get; set; }
            public int AllowableDeflectionIndex { get; set; }
            public double U_Value { get; set; }

            public double STC { get; set; }
            public double OITC { get; set; }
            public double RW { get; set; }

            public string Alloy { get; set; }
            public double Fy { get; set; }
            public double Fu { get; set; }

            public string HardwareAlloy { get; set; }
            public double HardwareFy { get; set; }
            public double HardwareFu { get; set; }
            public string HardwareFinishes { get; set; }

            public string InsulationType { get; set; }


        }

        //------------------------------


        /// <summary>
        /// Set 2D points in X/Y coordinate space
        /// </summary>
        public class Point       //Is this not already a function in Rhino common?
        {
            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }
            public double X { get; set; }
            public double Y { get; set; }
        }

        /// <summary>
        /// Name assigned to a given extrusion for the project
        /// </summary>
        public class Part
        {
            public Part()
            {
                CornerPoints = new List<Point>();
                CornerNodes = new List<Point>();
                SideMemberTypes = new List<int>();
                SideMembers = new List<int>();
                CornerPointsInside = new List<Point>();
                HoleLocations = new List<double>();
                HoleTypes = new List<string>();
            }

            //Below are properties of a part, read from DXF file database or Unified model

            public string ID { get; set; }
            public int Number { get; set; }
            public string Profile { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public string Finish { get; set; }
            public int Code { get; set; }
            public int Quantity { get; set; }
            public double InsideWidth { get; set; }
            public double OutsideWidth { get; set; }
            public double Depth { get; set; }
            public double Width { get; set; }
            public double Length { get; set; }
            public double Height { get; set; }
            public double Thickness { get; set; }
            public Point TextLocation { get; set; }
            public double TextAngle { get; set; }
            public List<Point> CornerPoints { get; set; }
            public List<Point> CornerPointsInside { get; set; }
            public List<Point> CornerNodes { get; set; }
            public List<int> SideMembers { get; set; }
            public List<int> SideMemberTypes { get; set; }
            public List<double> HoleLocations { get; set; }       //Locking bar holes
            public List<string> HoleTypes { get; set; }           //Locking bar holes
            public double StructuralLength { get; set; }          //use to set length for structural box, if different from length
            public List<Point> StructuralCornerPoints { get; set; } // use to draw structural boxes
            public bool DrawOnProposal { get; set; }        //boolean to determine which ones get drawn on Proposal
            public string EndCut { get; set; }          //90/90, 90/45, 45/90, 45/45, or Linear. First number is for left end of extrusion, second is for right end. For left jamb, first number is bottom. For right jamb, second number is bottom.
        
        }

        /// <summary>
        /// Add cut material to BOM, etc.
        /// </summary>
        public class Cut
        {
            public Cut()
            {

            }

            public string ID { get; set; }
            public string Profile { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public string Finish { get; set; }
            public int Code { get; set; }
            public int Quantity { get; set; }
            public double Length { get; set; }
        }

        /// <summary>
        /// Add table to FDS
        /// </summary>
        public class Table
        {

            public Table(string name, double X, double Y, double width, double height)
            {
                TextHeight = .086;
                XLocation = X;
                YLocation = Y;
                Width = width;
                Height = height;
                Name = name;
            }
            public Table()
            {
                TextHeight = .086;
            }
            public string Name { get; set; }
            public double XLocation { get; set; }
            public double YLocation { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double TextHeight { get; set; }

        }

        /// <summary>
        /// Add cross section to FDS
        /// </summary>
        public class Section
        {
            public int ID { get; set; }
            public string Page { get; set; }
            public double InsertionPointX { get; set; }
            public double InsertionPointY { get; set; }
            public string PageName { get; set; }
            public string DetailName { get; set; }
            public string Direction { get; set; }
            public int DirectionIndex { get; set; }
            public string MemberType { get; set; }
            public int SideOnePanel { get; set; }
            public int SideTwoPanel { get; set; }
            public string DetailType { get; set; }
            public List<Part> parts { get; set; }

            public Section()
            {
                parts = new List<Part>();
            }

        }

        /// <summary>
        /// Add a vent to FDS. Info for BOM, etc.
        /// </summary>
        public class Vent
        {
            public Vent()
            {
                Glass = new Part();
                Extrusions = new List<Part>();
                GlazingBeads = new List<Part>();
                LockingBars = new List<Part>();
                Glass = new Part();
                DoorGlass = new List<Part>();
                CornerPoints = new List<Point>();
                HandleLocation = new Point(0, 0);

            }
            public int Number { get; set; }
            public string ID { get; set; }                  
            
            //ID Naming logic: OrderNumber + "-XX" + "-YY" + "-ZZ"  (example: 1234-5678-WF-RJ-01)
                                                            
            //XX: For Outer frames, use WF, DF, or SF (Window/Door/Sliding)
            //XX: For Vents, use V + VentNumber (V1, V2, etc.)
            //YY: LJ = Left Jamb, RJ = Right Jamb, WS/DS/SS = Window/Door/Sliding Sill, WH/DH/SH = Window/Door/Sliding Head,
            //YY: WM/DM = Window/Door Mullion, WT/DT = Window/Door Transom, FB = Fixed Glazing Bead, VB = Vent Glazing Bead, SL = Side Light Sill
            //ZZ: Number for extrusion (D2 formatting). 

            public string Type { get; set; }
            public string Finish { get; set; }
            public string Profile { get; set; }
            public double ProfileInsideWidth { get; set; }
            public double ProfileOutsideWidth { get; set; }
            public double ProfileDepth { get; set; }
            public bool UseAdapter { get; set; }                    //For adapting inward to outward opening vent
            public string AdapterProfile { get; set; }
            public double AdapterInsideWidth { get; set; }
            public double AdapterOutsideWidth { get; set; }
            public string OpeningDirection { get; set; }
            public string ProfileClass { get; set; }
            public string VentOperableType { get; set; }        //Set based on Unified model
            public List<Point> CornerPoints { get; set; }
            public double Height { get; set; }
            public double Width { get; set; }
            public Point TextLocation { get; set; }
            public Point HandleLocation { get; set; }
            public string HandleSide { get; set; }
            public string HandleBlockName { get; set; }
            public string HandleColor { get; set; }
            public Part Glass { get; set; }
            public List<Part> Extrusions { get; set; }
            public List<Part> GlazingBeads { get; set; }
            public List<Part> LockingBars { get; set; }
            public double VentWeight { get; set; }                          // UnifiedInputVersion2.0. For SRS.
            public int LockingPointOption { get; set; }

            public int HandlePosition { get; set; }                      // UnifiedInputVersion2.0. For SRS.

            //ADS-specific fields
            public int DoorSystemID { get; set; }
            public string DoorSystemType { get; set; }
            public string DoorSillArticleName {get; set; }
            public string DoorLeafArticleName { get; set; }
            public string DoorPassiveJambArticleName { get; set; }       
            public string DoorThresholdArticleName { get; set; }
            public double DoorSillInsideW { get; set; }
            public double DoorSillOutsideW { get; set; }
            public double DoorLeafInsideW { get; set; }
            public double DoorLeafOutsideW { get; set; }
            public double DoorPassiveJambInsideW { get; set; }
            public double DoorPassiveJambOutsideW { get; set; }
            public string DoorSidelightSillArticleName { get; set; }
            public string OutsideHandleArticleName { get; set; }
            public string OutsideHandleColor { get; set; }
            public string InsideHandleArticleName { get; set; }
            public string InsideHandleColor { get; set; }
            public int HingeCondition { get; set; }
            public string HingeArticleName { get; set; }
            public string HingeColor { get; set; }
            public List<Part> DoorGlass { get; set; }       //using this to make two different pieces of glass for a double door. Assign to passive vent's glass a number of 33 to avoid conflict with existing glass.

            //ASE Fields


            public int SlidingDoorSystemID { get; set; }
            public int Track { get; set; }
            public bool InterlockReinforcement { get; set; }
            public string SlidingType { get; set; }

        }


    }
}

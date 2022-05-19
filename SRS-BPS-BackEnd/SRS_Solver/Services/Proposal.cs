using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BpsUnifiedModelLib;

namespace SRS_Solver.Services
{
    class Proposal
    {

        #region Create Project

        public static BpsUnifiedModel LoadModelFromJsonString(string jsonString)
        {
            BpsUnifiedModel model = JsonConvert.DeserializeObject<BpsUnifiedModel>(jsonString);
            return model;
        }

        public static BpsUnifiedModel LoadNewModelFromFile(string Filename)
        {
            string ModelFilepath = Filename;
            string strInput = "";
            using (StreamReader inputFile = new StreamReader(ModelFilepath))
            {
                strInput = inputFile.ReadToEnd();
            }
            BpsUnifiedModel model = JsonConvert.DeserializeObject<BpsUnifiedModel>(strInput);

            return model;
        }

        public static MemoryStream GetReport(string reportURL)
        {
            MemoryStream dataStream = new MemoryStream();
            if (File.Exists(reportURL))
            {
                var dataBytes = File.ReadAllBytes(reportURL);
                dataStream = new MemoryStream(dataBytes);
            }
            return dataStream;
        }

        public static TraceData CreateTraceData(BpsUnifiedModel Model)
        {
            //Universal functions

            TraceData td = new TraceData();
            CreateProject(Model, td);
            CreateSpecifications(Model, td);
            CreateGlassProperty(Model, td);
            CreateGlass(Model, td);


            //ADS vs ASE vs AWS functions

            if (Model.ModelInput.Geometry.DoorSystems != null)   //ADS
            {
                CreateFrame(Model, td, false);
                CreateGlazingBeads(Model, td);
                CreateDoorVents(Model, td);
            }
            else if (Model.ModelInput.Geometry.SlidingDoorSystems != null)  //ASE
            {
                CreateFrame(Model, td, true);
                CreateSlidingExtraFrameExtrusions(td);
                CreateSlidingDoorVents(Model, td);
                CreateSlidingExtraVentExtrusions(td);
            }
            else    //AWS
            {
                CreateFrame(Model, td, true);
                CreateGlazingBeads(Model, td);
                CreateVents(Model, td);
            }

            return td;
        }

        public static List<TraceData.Point> ControlPoints(BpsUnifiedModel Model, double Scale)
        {

            List<TraceData.Point> points = new List<TraceData.Point>();
            for (int i = 0; i < Model.ModelInput.Geometry.Points.Count; i++)
            {
                double x = Model.ModelInput.Geometry.Points.ElementAt(i).X * Scale;
                double y = Model.ModelInput.Geometry.Points.ElementAt(i).Y * Scale;
                TraceData.Point point = new TraceData.Point(x, y);
                points.Add(point);

            }
            return points;
        }

        public static void CreateProject(BpsUnifiedModel Model, TraceData TD)
        {
            TD.Project.Number = Model.SRSProblemSetting.ProjectNumber != null ?
                Model.SRSProblemSetting.ProjectNumber : "1905-01";
            TD.Project.OrderNumber = Model.SRSProblemSetting.OrderNumber != null ?
                Model.SRSProblemSetting.OrderNumber : "2645-04";
            TD.Project.Client = Model.UserSetting.UserName != null ?
                    Model.UserSetting.UserName : "N/A";
            TD.Project.Configuration = Model.ProblemSetting.ConfigurationName != null ?
                Model.ProblemSetting.ConfigurationName : "N/A";
            TD.Project.Name = Model.ProblemSetting.ProjectName != null ?
                Model.ProblemSetting.ProjectName : "N/A";
            TD.Project.Location = Model.ProblemSetting.Location != null ?
                Model.ProblemSetting.Location : "N/A";
            TD.Project.Alloy = Model.ModelInput.FrameSystem.Alloys != null ?
                Model.ModelInput.FrameSystem.Alloys : "N/A";
            TD.Project.Finish = Model.ModelInput.FrameSystem.AluminumFinish != null ?
                Model.ModelInput.FrameSystem.AluminumFinish : "N/A";
            TD.Project.Color = Model.ModelInput.FrameSystem.AluminumColor != null ?
                Model.ModelInput.FrameSystem.AluminumColor : "N/A";


            TD.Project.Scale = 1.0 / 25.4;                                          //Why is this needed if next line sets to 1?
            TD.Project.Scale = 1.0;                                                 //default to 1
            //TD.Project.Client = Model.UserSetting.UserName;                         //Currently username, will want to update to client
            //TD.Project.Configuration = Model.ProblemSetting.ConfigurationName;      
            //TD.Project.Name = Model.ProblemSetting.ProjectName;                     
            //TD.Project.Location = Model.ProblemSetting.Location;                    
            //TD.Project.Number = n;                                                  
            //TD.Project.OrderNumber = o;                                             
            //TD.Project.Alloy = Model.ModelInput.FrameSystem.Alloys;                 
            //TD.Project.Finish = Model.ModelInput.FrameSystem.AluminumFinish;
            //TD.Project.Color = Model.ModelInput.FrameSystem.AluminumColor;

            if (Model.ModelInput.Geometry.SlidingDoorSystems != null)
            {
                TD.Project.VentOperableType = Model.ModelInput.Geometry.OperabilitySystems[0].VentOperableType;
            }
        }

       
        public static void CreateSpecifications(BpsUnifiedModel Model, TraceData TD)
        {
            

            if (Model.AnalysisResult != null)
            {
                if (Model.AnalysisResult.ThermalResult != null)
                {
                    TD.Specs.U_Value = Model.AnalysisResult.ThermalResult.ThermalUIResult.TotalUw;
                   
                }
                else
                {
                    TD.Specs.U_Value = 0;
                }

                if (Model.AnalysisResult.AcousticResult != null)
                {
                    TD.Specs.STC = Model.AnalysisResult.AcousticResult.AcousticUIOutput.classification.STC;
                    TD.Specs.OITC = Model.AnalysisResult.AcousticResult.AcousticUIOutput.classification.OITC;
                    TD.Specs.RW = Model.AnalysisResult.AcousticResult.AcousticUIOutput.TotalRw;
                }
                else
                {
                    
                    TD.Specs.STC = 0;
                    TD.Specs.OITC = 0;
                    TD.Specs.RW = 0;
                }

                if (Model.ModelInput.Structural != null)
                {
                    TD.Specs.WindPressure = Model.ModelInput.Structural.WindLoad;
                    TD.Specs.LateralLiveLoad = Model.ModelInput.Structural.HorizontalLiveLoad;
                    TD.Specs.AllowableDeflectionIndex = Model.ModelInput.Structural.DispHorizontalIndex;
                }
                else
                {
                    TD.Specs.WindPressure = 0;
                    TD.Specs.LateralLiveLoad = 0;
                    TD.Specs.AllowableDeflectionIndex = 0;
                }
            }
        }


        public static void CreateGlassProperty(BpsUnifiedModel Model, TraceData TD)
        {
            
            var gs = Model.ModelInput.Geometry.GlazingSystems.Single(x => x.GlazingSystemID == 1);

            TD.Glazing.Manufacturer = gs.Manufacturer != null?
                gs.Manufacturer : "N/A";                                
            TD.Glazing.BrandName = gs.BrandName != null?
                gs.BrandName : "N/A";                                   
            TD.Glazing.Makeup = gs.Description != null?
                gs.Description : "N/A";                             
            TD.Glazing.SpacerBrand = gs.SpacerType.ToString() != null?
                gs.SpacerType.ToString() : "N/A";              
            TD.Glazing.U_value = gs.UValue != 0?
                gs.UValue : 0;                                 
            TD.Glazing.SHGC = gs.SHGC != 0?
                gs.SHGC : 0;                                      
            TD.Glazing.STC = gs.STC != 0?
                gs.STC : 0;                                        
            TD.Glazing.OITC = gs.OITC != 0?
                gs.OITC : 0;

            if (TD.Glazing.Makeup == "1/4+1/2 ARGON+1/4+1/2 ARGON+1/4 (1.75 3/4 in)")
            {
                TD.Glazing.DoubleGlazing = false;
            }
            else
            {
                TD.Glazing.DoubleGlazing = true;
            }
        }

        #endregion


        #region Utility


        public static string TraceFraction(double value, int fracBase = 32)
        {
            int[] result = { 0, 0, 0 };


            result[0] = (int)Math.Truncate(value);
            double num = (value - (double)result[0]);
            num *= fracBase;
            double denom = fracBase;
            if (num > 0)
            {
                while (num % 2 == 0 && denom % 2 == 0)
                {
                    num /= 2;
                    denom /= 2;
                }
                if (num == 1 && denom == 1)
                {
                    result[0] += 1;
                    num = 0;
                    denom = 0;
                }
                result[1] = (int)Math.Truncate(num);
                result[2] = (int)Math.Truncate(denom);
            }

            if (result[1] % 2 == 0 && result[2] % 2 == 0)
            {
                result[1] /= 2;
                result[2] /= 2;

            }
            string text = result[0].ToString() + " " + result[1].ToString() + "/" + result[2].ToString();
            if (result[1] == 0)
            {
                text = result[0].ToString();
            }

            return text;
        }

        /// <summary>
        /// OLD Set offset for mullion/transoms in AWS (doesn't yet account for structural)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Member"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        public static double EndOffset(BpsUnifiedModel Model, int Member, string EndPoint)
        {
            // create end offset for all members except frames
            //Review this logic and expand to deal with 3-way vs 4-way intersections for mullion/transoms
            var members = Model.ModelInput.Geometry.Members;
            var points = Model.ModelInput.Geometry.Points;
            var sections = Model.ModelInput.Geometry.Sections;
            int memberType = members.ElementAt(Member - 1).MemberType;
            double endOffset = 0;
            int memberEnd;
            if (EndPoint == "PointA")
            {
                memberEnd = members.ElementAt(Member - 1).PointA;
            }
            else
            {
                memberEnd = members.ElementAt(Member - 1).PointB;
            }

            double X = points.ElementAt(memberEnd - 1).X;
            double Y = points.ElementAt(memberEnd - 1).Y;

            for (int i = 0; i < members.Count - 1; i++)
            {
                if (i != Member - 1)
                {
                    int inode = members.ElementAt(i).PointA;
                    int jnode = members.ElementAt(i).PointB;
                    double XA = points.ElementAt(inode - 1).X;
                    double YA = points.ElementAt(inode - 1).Y;
                    double XB = points.ElementAt(jnode - 1).X;
                    double YB = points.ElementAt(jnode - 1).Y;
                    int endType = members.ElementAt(i).MemberType;
                    int endSection = members.ElementAt(i).SectionID;
                    if (memberType == 2 && (Math.Abs(Y - YA) < .0001 || Math.Abs(Y - YB) < .0001) && (Math.Abs(XA - XB) > 0.0001))
                    {
                        if (endType == 1)
                        {
                            endOffset = sections.ElementAt(endSection - 1).OutsideW;
                        }
                        else
                        {
                            endOffset = sections.ElementAt(endSection - 1).OutsideW / 2;
                        }
                    }
                    else if (memberType == 3 && (Math.Abs(X - XA) < .0001 || Math.Abs(X - XB) < .0001) && (Math.Abs(YA - YB) > 0.0001))
                    {
                        if (endType == 1)
                        {
                            endOffset = sections.ElementAt(endSection - 1).OutsideW;
                        }
                        else
                        {
                            endOffset = sections.ElementAt(endSection - 1).OutsideW / 2;
                        }
                    }
                }
            }

            return endOffset;
        }

        /// <summary>
        /// set offset for mullion/transoms in ADS (structural not considered here)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="Member"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        public static double EndOffset(BpsUnifiedModel Model, int MemberID, int PointID, bool isWindow)
        {
            // create end offset for all members except frames
            var members = Model.ModelInput.Geometry.Members;
            var points = Model.ModelInput.Geometry.Points;
            var sections = Model.ModelInput.Geometry.Sections;
            int memberType = members.Single(x => x.MemberID == MemberID).MemberType;
            double endOffset = 0;
            int memberEnd = PointID;

            //Find X and Y for PointA or PointB of Member

            double X = points.Single(x => x.PointID == PointID).X;
            double Y = points.Single(x => x.PointID == PointID).Y;

            //AWS Mullion
            if(memberType == 2 && isWindow == true)
            {
                if (Y == points.ElementAt(1).Y || Y == points.ElementAt(0).Y)               //offset at head and at sill
                {
                    endOffset = sections.Single(x => x.SectionID == 1).OutsideW;        
                }
                else if (Y != points.ElementAt(0).Y && Y != points.ElementAt(1).Y)           //offset at transom connection
                {
                    endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2;
                }

            }

            //ADS Mullion
            else if (memberType == 2 && isWindow == false)
            {
                if (Y == points.ElementAt(1).Y)                                     //offset at head but not at sill for door
                {
                    endOffset = sections.Single(x => x.SectionID == 1).OutsideW;        
                }

                if (Y != points.ElementAt(0).Y && Y != points.ElementAt(1).Y)       //offset at transom connection
                {
                    endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2;
                }
            }

            //Transom or Sidelight Sill
            else if (memberType == 3 || memberType == 33)
            {
                {
                    if (X == points.ElementAt(0).X || X == points.ElementAt(2).X)
                    {
                        endOffset = sections.Single(x => x.SectionID == 1).OutsideW;        //edge
                    }

                    if (X != points.ElementAt(0).X && X != points.ElementAt(2).X)
                    {
                        endOffset = sections.Single(x => x.SectionID == 2).OutsideW / 2;
                    }
                }
            }



            return endOffset;
        }

        public static double StructuralOffset(BpsUnifiedModel Model, int MemberID, int PointID, bool isWindow)
        {
            // create end offset for all members except frames
            var members = Model.ModelInput.Geometry.Members;
            var points = Model.ModelInput.Geometry.Points;
            var sections = Model.ModelInput.Geometry.Sections;
            int memberType = members.Single(x => x.MemberID == MemberID).MemberType;
            int memberSection = members.Single(x => x.MemberID == MemberID).SectionID;
            string memberArticle = sections.Single(x => x.SectionID == memberSection).ArticleName;
            double endOffset = 0;
            int memberEnd = PointID;

            //Find X and Y for PointA or PointB of Member

            double X = points.Single(x => x.PointID == PointID).X;
            double Y = points.Single(x => x.PointID == PointID).Y;

            double mullionDepth = 75;
            double transomDepth = 75;
            string mullionArticle = "";
            string transomArticle = "";

            //Use to determine if 3-way or 4-way intersection
            int pointCount = 0;
            for (var i = 0; i < points.Count; i++)
            {
                if (points.ElementAt(PointID - 1).X == points.ElementAt(i).X && points.ElementAt(PointID - 1).Y == points.ElementAt(i).Y)
                {
                    pointCount++;
                }
            }

            //Set Mullion article and depth (if any are in configuration)
            if (sections.Any(x => x.SectionType == 2) == true)
            {
                mullionArticle = sections.Single(x => x.SectionType == 2).ArticleName;
                if(mullionArticle == "368650")
                {
                    mullionDepth = 100;
                }
                else if (mullionArticle == "368660")
                {
                    mullionDepth = 125;
                }
            }

            //Set Transom article and depth (if any are in configuration)
            if (sections.Any(x => x.SectionType == 3) == true)
            {
                transomArticle = sections.Single(x => x.SectionType == 3).ArticleName;
                if (transomArticle == "368650")
                {
                    transomDepth = 100;
                }
                else if (transomArticle == "368660")
                {
                    transomDepth = 125;
                }
            }

            if(memberArticle == "382280" && memberType == 2) //Regular mullion
            {
                if(Y == points.ElementAt(0).Y && isWindow == false) //ADS bottom offset is zero
                {
                    endOffset = 0;
                }
                else 
                {
                    endOffset = 25;
                }
            }
            else if (memberArticle == "382280" && memberType == 3) //Regular transom
            {
                endOffset = 25;
            }
            else if (memberType == 2 && isWindow == true) //AWS structural mullion
            {
                if (Y == points.ElementAt(1).Y || Y == points.ElementAt(0).Y)               //offset at head and at sill
                {
                    endOffset = sections.Single(x => x.SectionID == 1).OutsideW;
                }
                else if (Y != points.ElementAt(1).Y && Y != points.ElementAt(0).Y)
                {
                    if (pointCount > 1) //4-way intersection
                    {
                        if (mullionDepth > transomDepth) //4-way, transom has smaller box
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2;
                        }
                        else if (mullionDepth <= transomDepth) //4-way, transom is equal or larger
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2 - sections.Single(x => x.SectionID == 3).InsideW / 2;
                        }
                    }

                    else //3-way intersection
                {
                        if (transomDepth == 75) //3-way, transom is regular
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW;
                        }
                        else if (mullionDepth > transomDepth) //3-way, transom has smaller box
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2 + sections.Single(x => x.SectionID == 3).InsideW / 2;
                        }
                        else if (mullionDepth <= transomDepth) //3-way, transom is equal or larger
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2 - sections.Single(x => x.SectionID == 3).InsideW / 2;
                        }
                    } 
                }
            }
            else if (memberType == 2 && isWindow == false) //ADS structural mullion
            {
                if (Y == points.ElementAt(1).Y)               //offset at head and at sill
                {
                    endOffset = sections.Single(x => x.SectionID == 1).OutsideW;
                }
                else if (Y == points.ElementAt(0).Y)
                {
                    endOffset = 0;
                    return endOffset;
                }
                else if (Y != points.ElementAt(1).Y && Y != points.ElementAt(0).Y)
                {
                    if (pointCount > 1) //4-way intersection
                    {
                        if (mullionDepth > transomDepth) //4-way, transom has smaller box
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2;
                        }
                        else if (mullionDepth <= transomDepth) //4-way, transom is equal or larger
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2 - sections.Single(x => x.SectionID == 3).InsideW / 2;
                        }
                    }

                    else //3-way intersection
                    {
                        if (transomDepth == 75) //3-way, transom is regular
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW;
                        }
                        else if (mullionDepth > transomDepth) //3-way, transom has smaller box
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2 + sections.Single(x => x.SectionID == 3).InsideW / 2;
                        }
                        else if (mullionDepth <= transomDepth) //3-way, transom is equal or larger
                        {
                            endOffset = sections.Single(x => x.SectionID == 3).OutsideW / 2 - sections.Single(x => x.SectionID == 3).InsideW / 2;
                        }
                    }
                }
            }
            else if (memberType == 3) //AWS or ADS structural transom
            {
                if (X == points.ElementAt(0).X || X == points.ElementAt(2).X)               //offset at head and at sill
                {
                    endOffset = sections.Single(x => x.SectionID == 1).OutsideW;
                }
                else if (X != points.ElementAt(0).X && X != points.ElementAt(2).X)
                { 
                    if (pointCount > 1) //4-way intersection
                    {
                        if (transomDepth > mullionDepth) //4-way, mullion has smaller box
                        {
                            endOffset = sections.Single(x => x.SectionID == 2).OutsideW / 2;
                        }
                        else if (transomDepth <= mullionDepth) //4-way, mullion box is equal or larger
                        {
                            endOffset = sections.Single(x => x.SectionID == 2).OutsideW / 2 - sections.Single(x => x.SectionID == 2).InsideW / 2;
                        }
                    }
                    else //3-way intersection
                    {
                        if (mullionDepth == 75) //3-way, mullion is regular
                        {
                            endOffset = sections.Single(x => x.SectionID == 2).OutsideW;
                        }
                        else if (transomDepth > mullionDepth) //3-way, mullion has smaller box
                        {
                            endOffset = sections.Single(x => x.SectionID == 2).OutsideW / 2 + sections.Single(x => x.SectionID == 2).InsideW / 2;
                        }
                        else if (transomDepth <= mullionDepth) //3-way, mullion box is equal or larger
                        {
                            endOffset = sections.Single(x => x.SectionID == 2).OutsideW / 2 - sections.Single(x => x.SectionID == 2).InsideW / 2;
                        }
                    } 
                }
            }

            return endOffset;
        }
        /// <summary>
        /// Set glass offset for AWS or ADS (ASE doesn't have fixed glass so no use for this currently)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="MemberType"></param>
        /// <param name="SectionIndex"></param>
        /// <returns></returns>
        public static double GlassOffset(BpsUnifiedModel Model, int MemberType, int SectionIndex)
        {
            double offset;
            offset = Model.ModelInput.Geometry.Sections.Single(x => x.SectionID == SectionIndex).InsideW;
            
            if (MemberType == 1 || MemberType == 33)
            {
                offset = (offset + 8);      //outer frame or Door sidelight sill
            }
            else
            {
                offset = offset / 2 + 8;     //intermediates
            }
            

            return offset;
        }
        #endregion


        #region Create Frame

        /// <summary>
        /// Create glass for AWS, ADS, and ASE (ASE currently just uses one big piece for all vents)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="TD"></param>
        public static void CreateGlass(BpsUnifiedModel Model, TraceData TD)
        {
            var geo = Model.ModelInput.Geometry;
            var glasslist = Model.ModelInput.Geometry.Infills;
            var members = Model.ModelInput.Geometry.Members;

            double[] offset = new double[4];
            double[] X = new double[4];
            double[] Y = new double[4];
            int[] sideMemberTypes = new int[4];
            int[] sideMembers = new int[4];
            double scale = TD.Project.Scale;

            foreach (var glass in glasslist)
            {
                for (int i = 0; i < 4; i++)
                {
                    int memberIndex = glass.BoundingMembers.ElementAt(i);
                    int sectionIndex = members.ElementAt(memberIndex - 1).SectionID;
                    int memberType = members.ElementAt(memberIndex - 1).MemberType;
                    sideMembers[i] = memberIndex;
                    sideMemberTypes[i] = memberType;
                    int inode = members.ElementAt(memberIndex - 1).PointA;
                    offset[i] = GlassOffset(Model, memberType, sectionIndex) * scale;
                    X[i] = geo.Points.ElementAt(inode - 1).X * scale;
                    Y[i] = geo.Points.ElementAt(inode - 1).Y * scale;

                }

                var pane = new TraceData.Part();
                TraceData.Point[] pts = new TraceData.Point[4];

                pts[0] = new TraceData.Point(X[0] + offset[0], Y[3] + offset[3]);
                pts[1] = new TraceData.Point(X[0] + offset[0], Y[1] - offset[1]);
                pts[2] = new TraceData.Point(X[2] - offset[2], Y[1] - offset[1]);
                pts[3] = new TraceData.Point(X[2] - offset[2], Y[3] + offset[3]);

                TraceData.Point[] nds = new TraceData.Point[4];

                nds[0] = new TraceData.Point(X[0], Y[3]);
                nds[1] = new TraceData.Point(X[0], Y[1]);
                nds[2] = new TraceData.Point(X[2], Y[1]);
                nds[3] = new TraceData.Point(X[2], Y[3]);


                for (int i = 0; i < 4; i++)
                {
                    pane.CornerPoints.Add(pts[i]);
                    pane.CornerNodes.Add(nds[i]);
                    pane.SideMembers.Add(sideMembers[i]);
                    pane.SideMemberTypes.Add(sideMemberTypes[i]);
                }

                pane.Width = Math.Round(Math.Abs(pts[3].X - pts[0].X), 3);
                pane.Height = Math.Round(Math.Abs(pts[1].Y - pts[0].Y), 3);

                pane.TextLocation = new TraceData.Point((pts[0].X + pts[2].X) / 2, (pts[0].Y + pts[2].Y) / 2);

                pane.Type = "Fixed";
                if (glass.OperabilitySystemID != -1) pane.Type = "Vent";

                pane.Quantity = 1;
                pane.Profile = glass.InfillID.ToString();
                TD.Glazing.Panes.Add(pane);
            }
        }

        /// <summary>
        /// Generate Outer Frame for AWS, ADS, and ASE
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="TD"></param>
        /// <param name="isWindow"></param>
        public static void CreateFrame(BpsUnifiedModel Model, TraceData TD, bool isWindow)
        {
            double scale = TD.Project.Scale;

            // Create  Nodal Points

            List<TraceData.Point> points = ControlPoints(Model, TD.Project.Scale);

            int index = 0;

            foreach (var m in Model.ModelInput.Geometry.Members)                                    
            {
                
                int PointAID = m.PointA;
                int PointBID = m.PointB;
                var PointA = points.ElementAt(PointAID - 1);
                var PointB = points.ElementAt(PointBID - 1);

                // create extrusion
                var extrusion = new TraceData.Part();
                extrusion.Profile = Model.ModelInput.Geometry.Sections.Single(x => x.SectionID == m.SectionID).ArticleName;
                extrusion.Depth = Math.Abs(Model.ModelInput.Geometry.Sections.Single(x => x.SectionID == m.SectionID).Depth);
                extrusion.Finish = TD.Project.Color;

                //Set ID for AWS, ADS, or ASE

                if (isWindow == true && Model.ModelInput.Geometry.SlidingDoorSystems == null)   //AWS
                {
                    extrusion.ID = TD.Project.OrderNumber + "-WF";
                }
                else if (isWindow == true && Model.ModelInput.Geometry.SlidingDoorSystems != null)  //ASE
                {
                    extrusion.ID = TD.Project.OrderNumber + "-SF";
                }
                else if (isWindow == false) //ADS
                {
                    extrusion.ID = TD.Project.OrderNumber + "-DF";
                }

                //Set extrusion widths

                extrusion.InsideWidth = Model.ModelInput.Geometry.Sections.Single(x => x.SectionID == m.SectionID).InsideW;
                extrusion.OutsideWidth = Model.ModelInput.Geometry.Sections.Single(x => x.SectionID == m.SectionID).OutsideW;
                var outsideWidth = extrusion.OutsideWidth * scale;

                //Set offsets (not considering structural)
                double OffsetA = EndOffset(Model, index + 1, PointAID, isWindow) * scale;
                double OffsetB = EndOffset(Model, index + 1, PointBID, isWindow) * scale;

                //Create extrusion corner points 

                TraceData.Point[] p = Frame(index + 1, m.MemberType, PointA, PointB, outsideWidth, OffsetA, OffsetB, isWindow);

                //Add extrusion corner points
                for (int j = 0; j < 4; j++) { extrusion.CornerPoints.Add(p[j]); }


                //Initialize length
                double length = 0;

                //This extra offset gets actual lengths for intermediates (not currently drawn on Proposal but needed in BOM)
                
                double StructuralA = StructuralOffset(Model, index + 1, PointAID, isWindow) * scale;
                double StructuralB = StructuralOffset(Model, index + 1, PointBID, isWindow) * scale;

                double structuralOffset = StructuralA + StructuralB;

                extrusion.DrawOnProposal = true;    //Draw on Proposal

                //Set extrusion values

                if (m.MemberID == 1 && (m.MemberType == 1 || m.MemberType == 41))
                {
                   
                    length = p[1].Y - p[0].Y;
                    extrusion.TextAngle = 90;
                    extrusion.Type = "Left Jamb";
                    extrusion.ID = extrusion.ID + "-LJ-01";

                    if(isWindow == true)
                    {
                        extrusion.EndCut = "45/45";
                    }
                    else
                    {
                        extrusion.EndCut = "90/45";
                    }

                }
                else if (m.MemberID == 2 && (m.MemberType == 1 || m.MemberType == 41))
                {

                    length = p[1].Y - p[0].Y;
                    extrusion.TextAngle = 90;
                    extrusion.Type = "Right Jamb";
                    extrusion.ID = extrusion.ID + "-RJ-02";

                    if (isWindow == true)
                    {
                        extrusion.EndCut = "45/45";
                    }
                    else
                    {
                        extrusion.EndCut = "45/90";
                    }
                }
                else if (m.MemberID == 3 && (m.MemberType == 1 || m.MemberType == 45) && isWindow == true) //AWS Sill
                {
                    
                    length = p[1].X - p[0].X;
                    extrusion.TextAngle = 0;
                    extrusion.Type = "Sill";
                    extrusion.ID = extrusion.ID + "-BS-03";
                    extrusion.EndCut = "45/45";
                }
                else if (m.MemberID == 4 && (m.MemberType == 1 || m.MemberType == 41) && isWindow == true) //AWS Head
                {
                   
                    length = p[1].X - p[0].X;
                    extrusion.TextAngle = 0;
                    extrusion.Type = "Head";
                    extrusion.ID = extrusion.ID + "-TH-04";
                    extrusion.EndCut = "45/45";
                }
                else if (m.MemberID != 1 && m.MemberID != 2 && m.MemberType == 1 && isWindow == false) //ADS Head
                {

                    length = p[1].X - p[0].X;
                    extrusion.TextAngle = 0;
                    extrusion.Type = "Head";
                    extrusion.ID = extrusion.ID + "-TH-03";
                    extrusion.EndCut = "45/45";
                }
                else if (m.MemberType == 2 && isWindow == true) //AWS Mullion
                {
                   
                    extrusion.Type = "Mullion";
                    length = p[1].Y - p[0].Y + structuralOffset;
                    extrusion.TextAngle = 90;
                    extrusion.ID = extrusion.ID + "-IM-" + (index + 1).ToString("D2");
                    extrusion.EndCut = "90/90";
                }
                else if (m.MemberType == 2 && isWindow == false) //ADS Mullion
                {

                    extrusion.Type = "Mullion";
                    length = p[1].Y - p[0].Y + structuralOffset;
                    extrusion.TextAngle = 90;
                    extrusion.ID = extrusion.ID + "-IM-" + (index + 1).ToString("D2");
                    extrusion.EndCut = "90/90";
                }
                else if (m.MemberType == 3)
                {
                    
                    extrusion.Type = "Transom";
                    length = p[1].X - p[0].X + structuralOffset;
                    extrusion.TextAngle = 0;
                    extrusion.ID = extrusion.ID + "-IT-" + (index + 1).ToString("D2");
                    extrusion.EndCut = "90/90";
                }
                else if (m.MemberType == 31)
                {
                   
                    length = p[1].X - p[0].X;
                    extrusion.TextAngle = 0;
                    extrusion.Type = "Door Threshold";
                    extrusion.DrawOnProposal = false;   //don't draw

                }
                else if (m.MemberType == 33)
                {
                   
                    length = p[1].X - p[0].X + 50;
                    extrusion.TextAngle = 0;
                    extrusion.Type = "Door Sidelight Sill";
                    extrusion.ID = extrusion.ID + "-SL-" + (index + 1).ToString("D2");
                    extrusion.EndCut = "90/90";
                }

                length = Math.Round(Math.Abs(length), 3);
                extrusion.Length = length;
                
                extrusion.TextLocation = new TraceData.Point((extrusion.CornerPoints[0].X + extrusion.CornerPoints[2].X) / 2,
                                                            (extrusion.CornerPoints[0].Y + extrusion.CornerPoints[2].Y) / 2);

                TD.Framing.Extrusions.Add(extrusion);
                index++;
            }
        }

        /// <summary>
        /// Generate extra extrusions for ASE 60 in BOM
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="TD"></param>
        public static void CreateSlidingExtraFrameExtrusions(TraceData TD)
        {
            //These are required for outer frame of ASE. Hard coding with ASE 60 articles for now, in future could expand to include ASE 80 articles too.
            
            //Drip Bar

            var e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-TH-05";
            e.Profile = "359700";
            e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Head").Length - 64;
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Drip Bar";
            e.Finish = TD.Project.Color;

            TD.Framing.Extrusions.Add(e);
            TD.Framing.Extrusions.Add(e);

            //Guide Profile

            e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-TH-06";
            e.Profile = "278380";
            e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Head").Length - 96;
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Guide Profile";
            e.Finish = "Black";

            //2 for double, 3 for triple track

            if (TD.Project.VentOperableType.Contains("2"))
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3"))
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }

            //Insulating Profile (Head)

            e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-TH-07";
            e.Profile = "278382";
            e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Head").Length - 64;
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Insulating Profile";
            e.Finish = "Black";

            //1 for double, 2 for triple

            if (TD.Project.VentOperableType.Contains("2"))
            {
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3"))
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }

            //Insulating Profile (Jambs)

            e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-SJ-08";
            e.Profile = "278381";
            e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Right Jamb").Length - 53;
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Insulating Profile";
            e.Finish = "Black";

            //x1 per jamb for double, x2 per jamb for triple

            if (TD.Project.VentOperableType.Contains("2"))
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3"))
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }

            //Insulating Profile (Sill)

            e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-BS-09";
            e.Profile = "278381";
            e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 42;
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Insulating Profile";
            e.Finish = "Black";

            //x1 for double, x2 for triple

            if (TD.Project.VentOperableType.Contains("2"))
            {
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3"))
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }

            //Track

            e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-BS-10";
            e.Profile = "201316";
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Track";
            e.Finish = "INOX";

            //Set lengths and add correct quantity of tracks based on Type
            if (TD.Project.VentOperableType.Contains("2A-")) //2A has two tracks, same length
            {
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 100;
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("2A1")) //2A/1 has one track
            {
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 100;
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("2D1")) //2D/1.i has one track
            {
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 104;
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3E-")) //3E has three tracks, two same length and one different
            {
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 100;
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-SF-BS-11";
                e.Profile = "201316";
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Track";
                e.Finish = "INOX";
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 104;
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3E1")) //3E/1 has two tracks, each of different length
            {
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 100;
                TD.Framing.Extrusions.Add(e);

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-SF-BS-11";
                e.Profile = "201316";
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Track";
                e.Finish = "INOX";
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 104;
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3F")) //3F has two tracks of same length
            {
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 104;
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }

            //Groove Cover Profile

            e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-SJ-11";
            e.Profile = "278362";
            e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Right Jamb").Length - 91;
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Groove Cover Profile";
            e.Finish = "Black";

            //0 - 2 based on Type. 2A has 2, 2A/1 has 1, 2D/1.i has 0, 3E has 2, 3E/1 has 1, 3F has 0

            if (TD.Project.VentOperableType.Contains("2A1") || TD.Project.VentOperableType.Contains("3E1"))
            {
                TD.Framing.Extrusions.Add(e);
            }
            else if ((TD.Project.VentOperableType.Contains("2A-"))
                || (TD.Project.VentOperableType.Contains("3E-"))
                )
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("2D1") || TD.Project.VentOperableType.Contains("3F"))
            {
                //Zero
            }

            //Cover Profile (Jambs)

            e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-SJ-12";
            e.Profile = "460640";
            e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Right Jamb").Length - 69;
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Cover Profile";
            e.Finish = TD.Project.Color;

            if (TD.Project.VentOperableType.Contains("2A1")) //2A/1.i contains one that is different length!
            {
                TD.Framing.Extrusions.Add(e);

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-SF-SJ-13";
                e.Profile = "460640";
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Right Jamb").Length - 87;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Cover Profile";
                e.Finish = TD.Project.Color;
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("2"))
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3E1")) //3E/1 contains one that is different length!
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-SF-SJ-13";
                e.Profile = "460640";
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Right Jamb").Length - 87;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Cover Profile";
                e.Finish = TD.Project.Color;

                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3"))
            {
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
                TD.Framing.Extrusions.Add(e);
            }

            //Threshold Profile (Rule takes overall width assuming that vent actual widths are equal. If vents are not equal width, rule breaks)

            e = new TraceData.Part();

            e.ID = TD.Project.OrderNumber + "-SF-BS-13";
            e.Profile = "460620";
            e.DrawOnProposal = false;
            e.EndCut = "90/90";
            e.Quantity = 1;
            e.Type = "Threshold Profile";
            e.Finish = TD.Project.Color;

            //Find length based on type

            if (TD.Project.VentOperableType.Contains("2A1"))
            {
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length / 2 - 84;
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("2D1")) 
            {
                e.Length = TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length / 2 - 139;
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3E1"))
            {
                e.Length = (TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 162) / 3 * 2;
                TD.Framing.Extrusions.Add(e);
            }
            else if (TD.Project.VentOperableType.Contains("3F"))
            {
                e.Length = (TD.Framing.Extrusions.Single(x => x.Type == "Sill").Length - 262) / 3 * 2;
                TD.Framing.Extrusions.Add(e);
            }

            
        }

       

        /// <summary>
        /// OLD create members for AWS
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="TD"></param>
        public static void CreateMembers(BpsUnifiedModel Model, TraceData TD)
        {
            double scale = TD.Project.Scale;
            List<TraceData.Point> points = ControlPoints(Model, TD.Project.Scale);

            // Mullion and Transoms
            for (int i = 4; i < Model.ModelInput.Geometry.Members.Count; i++)
            {
                int sectionID = Model.ModelInput.Geometry.Members.ElementAt(i).SectionID - 1;
                int inode = Model.ModelInput.Geometry.Members.ElementAt(i).PointA;
                int jnode = Model.ModelInput.Geometry.Members.ElementAt(i).PointB;
                int memberType = Model.ModelInput.Geometry.Members.ElementAt(i).MemberType;

                double width = Model.ModelInput.Geometry.Sections.ElementAt(sectionID).OutsideW * scale;
                double widthA = EndOffset(Model, i + 1, "PointA") * scale;
                double widthB = EndOffset(Model, i + 1, "PointB") * scale;

                // creare extrusion
                var extrusion = new TraceData.Part();
                extrusion.Profile = Model.ModelInput.Geometry.Sections.ElementAt(sectionID).ArticleName;
                extrusion.ID = TD.Project.OrderNumber + "-WF";

                // add corner points
                TraceData.Point[] p = Member(memberType, width, widthA, widthB,
                                                          points.ElementAt(inode - 1),
                                                          points.ElementAt(jnode - 1), scale);



                for (int j = 0; j < 4; j++) { extrusion.CornerPoints.Add(p[j]); }
                double length;

                if (memberType == 2)
                {
                    extrusion.Type = "Mullion";
                    length = p[1].Y - p[0].Y;
                    extrusion.TextAngle = 90;
                    extrusion.ID = extrusion.ID + "-WM";
                }
                else
                {
                    extrusion.Type = "Transom";
                    length = p[1].X - p[0].X;
                    extrusion.TextAngle = 0;
                    extrusion.ID = extrusion.ID + "-WT";
                }

                length = Math.Round(Math.Abs(length + 50 * scale), 3);
                extrusion.Length = length;
                extrusion.InsideWidth = Model.ModelInput.Geometry.Sections.ElementAt(sectionID).InsideW;
                extrusion.OutsideWidth = Model.ModelInput.Geometry.Sections.ElementAt(sectionID).OutsideW;
                
                extrusion.Depth = 75;

                //Change Depth for structural mullions! (quick fix, should make better logic for future)
                if (extrusion.Profile == "368650")
                {
                    extrusion.Depth = 100;
                }
                else if (extrusion.Profile == "368660")
                {
                    extrusion.Depth = 125;
                }

                extrusion.ID = extrusion.ID + "-" + i.ToString("D2");

                extrusion.TextLocation = new TraceData.Point((extrusion.CornerPoints[0].X + extrusion.CornerPoints[2].X) / 2,
                                                            (extrusion.CornerPoints[0].Y + extrusion.CornerPoints[2].Y) / 2);
                TD.Framing.Extrusions.Add(extrusion);

            }
        }

     
        /// <summary>
        /// Create fixed glazing beads for AWS and ADS (ASE has no fixed panes for now, will if we add Type 1s)
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="TD"></param>
        public static void CreateGlazingBeads(BpsUnifiedModel Model, TraceData TD)
        {
            double width, offset, length = 0;
            double scale = TD.Project.Scale;
            width = 22 * scale;
            offset = 8 * scale;
            int index = 1;

            foreach (var g in TD.Glazing.Panes)
            {

                if (g.Type != "Vent")
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var bead = new TraceData.Part();
                        TraceData.Point[] p = Bead(i, g.CornerPoints.ElementAt(i), g.CornerPoints.ElementAt((i + 1) % 4), width, offset);
                        for (int j = 0; j < 4; j++) { bead.CornerPoints.Add(p[j]); }

                        if (TD.Glazing.DoubleGlazing == true)
                        {
                            bead.Profile = "184070"; //bead for 25 mm glass in fixed
                        }
                        else
                        {
                            bead.Profile = "184030"; //bead for 44 mm glass in fixed
                        }

                        bead.ID = TD.Project.OrderNumber;

                        if(Model.ModelInput.Geometry.DoorSystems != null)
                        {
                            bead.ID = bead.ID + "-DF";
                        }
                        else
                        {
                            bead.ID = bead.ID + "-WF";
                        }

                        if (i == 0)
                        {
                            length = p[1].Y - p[0].Y;
                            bead.TextAngle = 90;
                            bead.TextLocation = new TraceData.Point(p[2].X + 40 * scale, (p[0].Y + p[1].Y) / 2);
                            bead.ID = bead.ID + "-FB" + "-" + index.ToString("D2");
                        }
                        else if (i == 1)
                        {
                            length = p[3].X - p[0].X;
                            bead.TextAngle = 0;
                            bead.TextLocation = new TraceData.Point((p[0].X + p[3].X) / 2, p[0].Y - 40 * scale);
                            bead.ID = bead.ID + "-FB" + "-" + (index + 1).ToString("D2");
                        }
                        else if (i == 2)
                        {
                            length = p[1].Y - p[0].Y;
                            bead.TextAngle = 90;
                            bead.TextLocation = new TraceData.Point(p[2].X - 40 * scale, (p[2].Y + p[3].Y) / 2);
                            bead.ID = bead.ID + "-FB" + "-" + index.ToString("D2");
                        }
                        else if (i == 3)
                        {
                            length = p[3].X - p[0].X;
                            bead.TextAngle = 0;
                            bead.TextLocation = new TraceData.Point((p[0].X + p[3].X) / 2, p[1].Y + 40 * scale);
                            bead.ID = bead.ID + "-FB" + "-" + (index + 1).ToString("D2");


                        }

                        bead.Length = Math.Round(Math.Abs(length), 3);
                        bead.Type = "Fixed Bead";
                        bead.DrawOnProposal = true;

                        TD.Glazing.Beads.Add(bead);

                        
                    }

                    index = index + 2;
                }
            }
        }

        /// <summary>
        /// Create control points for AWS/ADS outer framing (doesn't consider structural intermediates)
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="MemberType"></param>
        /// <param name="PointA"></param>
        /// <param name="PointB"></param>
        /// <param name="Width"></param>
        /// <param name="WidthA"></param>
        /// <param name="WidthB"></param>
        /// <param name="isWindow"></param>
        /// <returns></returns>
        public static TraceData.Point[] Frame(int Member, int MemberType, TraceData.Point PointA, TraceData.Point PointB, double Width, double WidthA, double WidthB, bool isWindow)
        {
            TraceData.Point[] P = new TraceData.Point[4];

            P[0] = new TraceData.Point(PointA.X, PointA.Y);
            P[1] = new TraceData.Point(PointB.X, PointB.Y);
            P[2] = new TraceData.Point(0, 0);
            P[3] = new TraceData.Point(0, 0);

            if(isWindow == true)    //AWS
            {
                if (Member == 1 && (MemberType == 1 || MemberType == 41))
                {
                    //AWS Jamb 1
                    P[2].X = P[1].X + Width;
                    P[2].Y = P[1].Y - Width;
                    P[3].X = P[0].X + Width;
                    P[3].Y = P[0].Y + Width;

                }
                else if (Member == 2 && (MemberType == 1 || MemberType == 41))
                {
                    //AWS Jamb 2
                    P[2].X = P[1].X - Width;
                    P[2].Y = P[1].Y - Width;
                    P[3].X = P[0].X - Width;
                    P[3].Y = P[0].Y + Width;

                }
                else if (Member == 3 && (MemberType == 1 || MemberType == 45))
                {
                    //AWS Sill

                    P[2].X = P[1].X - Width;
                    P[2].Y = P[1].Y + Width;
                    P[3].X = P[0].X + Width;
                    P[3].Y = P[0].Y + Width;
                }
                else if (Member == 4 && (MemberType == 1 || MemberType == 41))
                {
                    //AWS Head
                    P[2].X = P[1].X - Width;
                    P[2].Y = P[1].Y - Width;
                    P[3].X = P[0].X + Width;
                    P[3].Y = P[0].Y - Width;
                }
                else if (MemberType == 2)
                {
                    //AWS Mullion
                    P[0].X = P[0].X - Width / 2;
                    P[1].X = P[1].X - Width / 2;
                    P[2].X = P[1].X + Width;
                    P[3].X = P[0].X + Width;

                    if (P[0].Y < P[1].Y)
                    {
                        P[0].Y = P[0].Y + WidthA;
                        P[1].Y = P[1].Y - WidthB;
                    }
                    else
                    {
                        P[0].Y = P[0].Y - WidthB;
                        P[1].Y = P[1].Y + WidthA;
                    }

                    P[2].Y = P[1].Y;
                    P[3].Y = P[0].Y;

                }

            }  
            else        //ADS
            {
                if (Member == 1 && MemberType == 1)
                {
                    //ADS Jamb 1
                    P[2].X = P[1].X + Width;
                    P[2].Y = P[1].Y - Width;
                    P[3].X = P[0].X + Width;
                    P[3].Y = P[0].Y;

                }
                else if (Member == 2 && MemberType == 1)
                {
                    //ADS Jamb 2
                    P[2].X = P[1].X - Width;
                    P[2].Y = P[1].Y - Width;
                    P[3].X = P[0].X - Width;
                    P[3].Y = P[0].Y;

                }
                else if (Member != 1 && Member != 2 && MemberType == 1)
                {
                    //ADS Head

                    P[2].X = P[1].X - Width;
                    P[2].Y = P[1].Y - Width;
                    P[3].X = P[0].X + Width;
                    P[3].Y = P[0].Y - Width;
                }
                else if (MemberType == 33)
                {
                    //Door Sidelight Sill
                    if (P[0].X < P[1].X)
                    {
                        P[0].X = P[0].X + WidthA;
                        P[1].X = P[1].X - WidthB;
                    }
                    else
                    {
                        P[0].X = P[0].X - WidthA;
                        P[1].X = P[1].X + WidthB;
                    }

                    P[2].X = P[1].X;
                    P[3].X = P[0].X;
                    P[2].Y = P[1].Y + Width;
                    P[3].Y = P[0].Y + Width;
                }
                else if (MemberType == 2)
                {
                    //ADS Mullion
                    P[0].X = P[0].X - Width / 2;
                    P[1].X = P[1].X - Width / 2;
                    P[2].X = P[1].X + Width;
                    P[3].X = P[0].X + Width;

                    if (P[0].Y < P[1].Y)
                    {

                        P[1].Y = P[1].Y - WidthB;
                    }
                    else
                    {
                        P[0].Y = P[0].Y - WidthB;

                    }

                    P[2].Y = P[1].Y;
                    P[3].Y = P[0].Y;

                }
            }
           
            
            if (MemberType == 3)
            {
                //Transom
                if (P[0].X < P[1].X)
                {
                    P[0].X = P[0].X + WidthA;
                    P[1].X = P[1].X - WidthB;
                }
                else
                {
                    P[0].X = P[0].X - WidthA;
                    P[1].X = P[1].X + WidthB;
                }

                P[2].X = P[1].X;
                P[3].X = P[0].X;

                P[0].Y = P[0].Y - Width / 2;
                P[1].Y = P[1].Y - Width / 2;
                P[2].Y = P[1].Y + Width;
                P[3].Y = P[0].Y + Width;
            }
            


            return P;
        }

        /// <summary>
        /// OLD create Member points for AWS
        /// </summary>
        /// <param name="MemberType"></param>
        /// <param name="Width"></param>
        /// <param name="WidthA"></param>
        /// <param name="WidthB"></param>
        /// <param name="PointA"></param>
        /// <param name="PointB"></param>
        /// <param name="Scale"></param>
        /// <returns></returns>
        public static TraceData.Point[] Member(int MemberType, double Width, double WidthA, double WidthB,
                                              TraceData.Point PointA, TraceData.Point PointB, double Scale)
        {
            TraceData.Point[] P = new TraceData.Point[4];

            P[0] = new TraceData.Point(PointA.X, PointA.Y);
            P[1] = new TraceData.Point(PointB.X, PointB.Y);
            P[2] = new TraceData.Point(0, 0);
            P[3] = new TraceData.Point(0, 0);

            switch (MemberType)
            {
                case 2: //Mullion
                    P[0].X = P[0].X - Width / 2;
                    P[1].X = P[1].X - Width / 2;
                    P[2].X = P[1].X + Width;
                    P[3].X = P[0].X + Width;

                    if (P[0].Y < P[1].Y)
                    {
                        P[0].Y = P[0].Y + WidthA;
                        P[1].Y = P[1].Y - WidthB;
                    }
                    else
                    {
                        P[0].Y = P[0].Y - WidthB;
                        P[1].Y = P[1].Y + WidthB;
                    }

                    P[2].Y = P[1].Y;
                    P[3].Y = P[0].Y;

                    break;

                case 3:  //Transom
                    if (P[0].X < P[1].X)
                    {
                        P[0].X = P[0].X + WidthA;
                        P[1].X = P[1].X - WidthB;
                    }
                    else
                    {
                        P[0].X = P[0].X - WidthA;
                        P[1].X = P[1].X + WidthB;
                    }

                    P[2].X = P[1].X;
                    P[3].X = P[0].X;

                    P[0].Y = P[0].Y - Width / 2;
                    P[1].Y = P[1].Y - Width / 2;
                    P[2].Y = P[1].Y + Width;
                    P[3].Y = P[0].Y + Width;
                    break;


                default:
                    return null;
            }

            return P;
        }

        public static TraceData.Point[] Bead(int MemberType, TraceData.Point PointA, TraceData.Point PointB, double Width, double GlassOffset)
        {

            TraceData.Point[] P = new TraceData.Point[4];

            P[0] = new TraceData.Point(0, 0);
            P[1] = new TraceData.Point(0, 0);
            P[2] = new TraceData.Point(0, 0);
            P[3] = new TraceData.Point(0, 0);


            switch (MemberType)
            {
                case 0:
                    P[0].X = PointA.X - GlassOffset;
                    P[0].Y = PointA.Y + Width - GlassOffset;
                    P[1].Y = PointB.Y - Width + GlassOffset;
                    P[2].X = PointB.X + Width - GlassOffset;
                    break;

                case 1:
                    P[0].X = PointA.X - GlassOffset;
                    P[0].Y = PointA.Y - Width + GlassOffset;
                    P[1].Y = P[0].Y + Width;
                    P[2].X = PointB.X + GlassOffset;

                    break;

                case 2:
                    P[0].X = PointA.X + GlassOffset;
                    P[0].Y = PointA.Y - Width + GlassOffset;
                    P[1].Y = PointB.Y + Width - GlassOffset;
                    P[2].X = PointB.X - Width + GlassOffset;

                    break;

                case 3:
                    P[0].X = PointB.X - GlassOffset;
                    P[0].Y = PointB.Y - GlassOffset;
                    P[1].Y = PointB.Y + Width - GlassOffset;
                    P[2].X = PointA.X + GlassOffset;

                    break;

                

                default:
                    return null;

            }

            P[1].X = P[0].X;
            P[2].Y = P[1].Y;
            P[3].X = P[2].X;
            P[3].Y = P[0].Y;

            return P;
        }
 
        #endregion


        #region Create Vents    

        public static void CreateVents(BpsUnifiedModel Model, TraceData TD)
        {
            var geo = Model.ModelInput.Geometry;
            var glassList = Model.ModelInput.Geometry.Infills;
            var ventList = Model.ModelInput.Geometry.OperabilitySystems;
            var members = Model.ModelInput.Geometry.Members;

            double[] offset = new double[4];
            double[] X = new double[4];
            double[] Y = new double[4];
            int[] sideMemberTypes = new int[4];
            int[] sideMembers = new int[4];
            double scale = TD.Project.Scale;
            int ventCount = 0;
            foreach (var g in glassList)
            {
                if (g.OperabilitySystemID == -1) continue;
                
                var v = new TraceData.Vent();

                // General Information
                var vent = ventList.ElementAt(g.OperabilitySystemID - 1);
                var infill = glassList.ElementAt(g.InfillID - 1);

                ventCount++;
                v.Number = ventCount;
                v.ID = TD.Project.OrderNumber + "-V" + ventCount.ToString();
                v.Type = vent.VentOperableType;
                v.Profile = vent.VentArticleName;
                v.ProfileInsideWidth = vent.VentInsideW;
                v.ProfileOutsideWidth = vent.VentOutsideW;
                v.UseAdapter = false;
                v.AdapterProfile = "-1";
                v.OpeningDirection = vent.VentOpeningDirection;
                v.HandleSide = "TBD";
                v.VentWeight = infill.VentWeight;
                v.LockingPointOption = infill.LockingPointOption;
                v.HandlePosition = g.HandlePosition;
                v.HandleColor = vent.InsideHandleColor;
                v.Finish = TD.Project.Color;
                

                // Corner Points

                for (int i = 0; i < 4; i++)
                {
                    int memberIndex = g.BoundingMembers.ElementAt(i);
                    int sectionIndex = members.ElementAt(memberIndex - 1).SectionID;
                    int memberType = members.ElementAt(memberIndex - 1).MemberType;
                    sideMembers[i] = memberIndex;
                    sideMemberTypes[i] = memberType;
                    int inode = members.ElementAt(memberIndex - 1).PointA;
                    offset[i] = VentOffset(Model, memberType, sectionIndex) * scale;
                    X[i] = geo.Points.ElementAt(inode - 1).X * scale;
                    Y[i] = geo.Points.ElementAt(inode - 1).Y * scale;

                }


                TraceData.Point[] pts = new TraceData.Point[4];

                pts[0] = new TraceData.Point(X[0] + offset[0], Y[3] + offset[3]);
                pts[1] = new TraceData.Point(X[0] + offset[0], Y[1] - offset[1]);
                pts[2] = new TraceData.Point(X[2] - offset[2], Y[1] - offset[1]);
                pts[3] = new TraceData.Point(X[2] - offset[2], Y[3] + offset[3]);

                // Add Corner Points

                for (int i = 0; i < 4; i++)
                {
                    v.CornerPoints.Add(pts[i]);
                }

                // Add Extusions

                CreateVentExtrusions(ref v, scale);

                // Update glass dimension based on the modified frame size Glass

                CreateVentGlass(ref v, scale);
                TD.Glazing.Panes[g.InfillID - 1].CornerPoints = v.Glass.CornerPoints;
                TD.Glazing.Panes[g.InfillID - 1].Width = v.Glass.Width;
                TD.Glazing.Panes[g.InfillID - 1].Height = v.Glass.Height;
                v.Glass.Number = TD.Glazing.Panes[g.InfillID - 1].Number;

                // Add Overall Sizes

                v.Width = Math.Round(Math.Abs(pts[3].X - pts[0].X), 3);
                v.Height = Math.Round(Math.Abs(pts[1].Y - pts[0].Y), 3);
                v.ProfileDepth = 75;

                // Add Window Handle

                v.HandleBlockName = "Handle01";



                if (vent.VentOperableType == "Tilt-Turn-Right" || vent.VentOperableType == "Turn-Tilt-Right")
                {
                    v.HandleLocation.X = pts[2].X - 34 / 2;
                    v.HandleLocation.Y = pts[0].Y + v.HandlePosition;
                    v.HandleSide = "Right";
                    v.Type = "TurnTilt";
                }
                else if (vent.VentOperableType == "Tilt-Turn-Left" || vent.VentOperableType == "Turn-Tilt-Left")
                {
                    v.HandleLocation.X = pts[0].X + 34 / 2; ;
                    v.HandleLocation.Y = pts[0].Y + v.HandlePosition;
                    v.HandleSide = "Left";
                    v.Type = "TurnTilt";
                }
                else if (vent.VentOperableType == "Bottom-Hung")           //BPS should add option to select handle location on Bottom Hung
                {
                    v.HandleLocation.X = pts[0].X + 34 / 2; ;
                    v.HandleLocation.Y = (pts[0].Y + pts[1].Y) / 2;
                    v.HandleSide = "Left";
                    v.Type = "BottomHung";
                }
                else if (vent.VentOperableType == "Side-Hung-Right")
                {
                    v.HandleLocation.X = pts[2].X - 34 / 2;
                    v.HandleLocation.Y = (pts[0].Y + pts[1].Y) / 2;
                    v.HandleSide = "Right";
                    v.Type = "SideHung";
                }
                else if (vent.VentOperableType == "Side-Hung-Left")
                {
                    v.HandleLocation.X = pts[0].X + 34 / 2; ;
                    v.HandleLocation.Y = (pts[0].Y + pts[1].Y) / 2;
                    v.HandleSide = "Left";
                    v.Type = "SideHung";
                }
                else if (vent.VentOperableType == null)
                {
                    v.HandleLocation.X = 0;
                    v.HandleLocation.Y = 0;
                    v.HandleSide = null;
                    v.Type = "Fixed";
                }

                // Add Glazing Beads

                CreateVentGlazingBeads(ref v, scale, TD);


                v.TextLocation = new TraceData.Point((pts[0].X + pts[2].X) / 2, (pts[0].Y + pts[2].Y) / 2 + 60);
                TD.Vents.Add(v);

            }
        }

        public static void CreateSlidingDoorVents(BpsUnifiedModel Model, TraceData TD)
        {
            var geo = Model.ModelInput.Geometry;
            var glassList = Model.ModelInput.Geometry.Infills;
            var ventList = Model.ModelInput.Geometry.OperabilitySystems;
            var slidingList = Model.ModelInput.Geometry.SlidingDoorSystems;
            var members = Model.ModelInput.Geometry.Members;
            
            double scale = TD.Project.Scale;
            var points = Model.ModelInput.Geometry.Points;      //list of points for outer frame
            var ventPoints = new List<Point>();                 //new list for vent frame points (just will add widths to make new points)


            
                //Set up initial left-hand points


                foreach (var g in glassList)
            {
                //initialize sliding data
                if (g.OperabilitySystemID == -1) continue;  //confirm operability system

                if (slidingList.Count != 1)
                {
                    continue;       //confirm single sliding system in configuration
                }

                int ventCount = slidingList[0].VentFrames.Count;
                var vent = ventList.ElementAt(g.OperabilitySystemID - 1);

                //Get correct point count based on number of vents

                int pointCount = 4;

                if(ventCount == 2)
                {
                    pointCount = 6;
                }
                else if (ventCount == 3)
                {
                    pointCount = 8;
                }
                else if (ventCount == 4)
                {
                    pointCount = 10;
                }
                else if (ventCount == 6)
                {
                    pointCount = 14;
                }

                TraceData.Point[] pts = new TraceData.Point[pointCount];

                //first two points will be left end points of model (0, 0 and then 0, height)

                pts[0] = new TraceData.Point(0, 0);
                pts[1] = new TraceData.Point(0, points.Single(x => x.PointID == 2).Y);
                int index = 0;

                //to get rest of points, need to add cumulative vent widths to X values

                for(var i = 2; i < pointCount - 1; i+= 2)
                {
                    double ventWidth = slidingList[0].VentFrames[index].Width;

                    //Need to get widths as vents are added (can't think of better way to do this)

                    if(index == 0)  //first vent
                    { }
                    else if (index == 1)    //second vent plus width of first
                    {
                        ventWidth = ventWidth + slidingList[0].VentFrames[index - 1].Width;
                    }
                    else if (index == 2)    //third vent plus widths of first/second
                    {
                        ventWidth = ventWidth + slidingList[0].VentFrames[index - 1].Width + slidingList[0].VentFrames[index - 2].Width;
                    }
                    else if (index == 3)    //fourth vent plus widths of first/second/third
                    {
                        ventWidth = ventWidth + slidingList[0].VentFrames[index - 1].Width + slidingList[0].VentFrames[index - 2].Width + slidingList[0].VentFrames[index - 3].Width;
                    }
                    else if (index == 4)    //fifth vent plus widths of first/second/third/fourth
                    {
                        ventWidth = ventWidth + slidingList[0].VentFrames[index - 1].Width + slidingList[0].VentFrames[index - 2].Width + slidingList[0].VentFrames[index - 3].Width + slidingList[0].VentFrames[index - 4].Width;
                    }
                    else if (index == 5)    //sixth vent plus widths of first/second/third/fourth/fifth
                    {
                        ventWidth = ventWidth + slidingList[0].VentFrames[index - 1].Width + slidingList[0].VentFrames[index - 2].Width + slidingList[0].VentFrames[index - 3].Width + slidingList[0].VentFrames[index - 4].Width + slidingList[0].VentFrames[index - 5].Width;
                    }

                    pts[i] = new TraceData.Point(ventWidth, 0);
                    pts[i + 1] = new TraceData.Point(ventWidth, points.Single(x => x.PointID == 2).Y);

                    index++;
                }

                //add point list to vent corner points

                int index2 = 0;
                foreach(var vf in slidingList[0].VentFrames) 
                {
                    var v = new TraceData.Vent();
                    
                    v.CornerPoints.Add(pts[0 + index2]);
                    v.CornerPoints.Add(pts[1 + index2]);
                    v.CornerPoints.Add(pts[3 + index2]);
                    v.CornerPoints.Add(pts[2 + index2]);

                    v.Profile = geo.Sections.Single(x => x.SectionID == 43).ArticleName;
                    v.Number = vf.VentFrameID;
                    v.ProfileInsideWidth = geo.Sections.Single(x => x.SectionID == 43).InsideW;
                    v.ProfileOutsideWidth = geo.Sections.Single(x => x.SectionID == 43).OutsideW;
                    v.Track = vf.Track;         //Track 1 to 3, with 3 being on inside and 1 on outside
                    v.SlidingType = vf.Type;    //Fixed or sliding
                    v.SlidingDoorSystemID = geo.SlidingDoorSystems[0].SlidingDoorSystemID;
                    v.InterlockReinforcement = geo.SlidingDoorSystems[0].InterlockReinforcement;
                    v.InsideHandleColor = geo.SlidingDoorSystems[0].InsideHandleColor;
                    v.OutsideHandleColor = geo.SlidingDoorSystems[0].OutsideHandleColor;
                    v.ID = TD.Project.OrderNumber + "-V" + vf.VentFrameID.ToString();
                    v.Finish = TD.Project.Color;

                    CreateSlidingVentExtrusions(ref v, scale, slidingList);

                    //Ignoring glass for now, as it can be one big piece behind rest in elevation
                    
                    // Add Overall Sizes

                    v.Width = Math.Round(Math.Abs(pts[3].X - pts[0].X), 3);
                    v.Height = Math.Round(Math.Abs(pts[1].Y - pts[0].Y), 3);
                    v.ProfileDepth = 75;

                    // Add Sliding Handles
                    var operableType = Model.ModelInput.Geometry.OperabilitySystems[0].VentOperableType;

                    if (v.SlidingType == "Sliding")
                    {
                       if(v.Track == 2 && slidingList[0].VentFrames.Any(x => x.Track == 3) == true) //Middle sliding unit that has no handle
                        {
                            v.HandleLocation.X = 0;
                            v.HandleLocation.Y = 0;
                            v.Type = "Passive Sliding";

                            if(v.Number == 5)
                            {
                                v.HandleSide = "Right";
                            }
                            else if (operableType.Contains("Left"))  //if vent operable type is left or right, can determine handle side (used for opening symbol)
                            {
                                v.HandleSide = "Left";
                            }
                            else if (operableType.Contains("Right"))  //if vent operable type is left or right, can determine handle side (used for opening symbol)
                            {
                                v.HandleSide = "Right";
                            }
                            else if (operableType.Contains("3F") && v.Number == 2)
                            {
                                v.HandleSide = "Left";
                            }
                        }
                        else if ((ventCount == 4 && v.Number == 2) || (ventCount == 6 && v.Number == 3))    //double vent on right from inside
                        {
                            v.HandleLocation.X = v.CornerPoints[2].X - 32.5;
                            v.HandleLocation.Y = 1007;
                            v.HandleSide = "Left";
                            v.Type = "Sliding";
                        }
                        else if ((ventCount == 4 && v.Number == 3) || (ventCount == 6 && v.Number == 4))    //double vent on left from inside
                        {
                            v.HandleLocation.X = v.CornerPoints[0].X + 32.5;
                            v.HandleLocation.Y = 1007;
                            v.HandleSide = "Right";
                            v.Type = "Sliding";
                        }
                        else if (v.Number == 1)     //First vent in series
                        {
                            v.HandleLocation.X = v.CornerPoints[0].X + 72.5;
                            v.HandleLocation.Y = 1007;
                            v.HandleSide = "Right";
                            v.Type = "Sliding";
                        }
                        else if (v.Number == slidingList[0].VentFrames.Count)       //last vent in series
                        {
                            v.HandleLocation.X = v.CornerPoints[2].X - 72.5;
                            v.HandleLocation.Y = 1007;
                            v.HandleSide = "Left";
                            v.Type = "Sliding";
                        }
                    }
                    else if (v.SlidingType == "Fixed")
                    {
                        v.HandleLocation.X = 0;
                        v.HandleLocation.Y = 0;
                        v.HandleSide = null;
                        v.Type = "Sliding Fixed";
                    }


                    // Add Glazing Beads

                    CreateSlidingVentGlazingBeads(ref v, scale, TD, slidingList);

                   
                   v.TextLocation = new TraceData.Point((pts[0].X + pts[2].X) / 2, (pts[0].Y + pts[2].Y) / 2 + 60);

                    TD.Vents.Add(v);

                    index2 = index2 + 2;
                }

                



            }
            
        }

        public static void CreateDoorVents(BpsUnifiedModel Model, TraceData TD)
        {
            var geo = Model.ModelInput.Geometry;
            var glassList = Model.ModelInput.Geometry.Infills;
            var ventList = Model.ModelInput.Geometry.OperabilitySystems;
            var doorlist = Model.ModelInput.Geometry.DoorSystems;
            var members = Model.ModelInput.Geometry.Members;

            double[] offset = new double[4];
            double[] X = new double[4];
            double[] Y = new double[4];
            int[] sideMemberTypes = new int[4];
            int[] sideMembers = new int[4];
            double[] sideMemberInsideW = new double[4];
            double[] sideMemberOutsideW = new double[4];
            double scale = TD.Project.Scale;
            int ventCount = 0;
            foreach (var g in glassList)
            {
                if (g.OperabilitySystemID == -1) continue;

                //If OperabilitySystemID != -1, then the following is executed:
                var v = new TraceData.Vent();

                // General Information
                var vent = ventList.ElementAt(g.OperabilitySystemID - 1);
                var door = doorlist.ElementAt(0);      
                var infill = glassList.ElementAt(g.InfillID - 1);

                ventCount++;
                v.Number = ventCount;       
                v.ID = TD.Project.OrderNumber + "-V" + ventCount.ToString();
                v.Type = vent.VentOperableType;
                v.OpeningDirection = vent.VentOpeningDirection;
                v.HandleSide = "TBD";
                v.DoorSystemID = door.DoorSystemID;
                v.DoorSystemType = door.DoorSystemType;
                v.DoorSillArticleName = door.DoorSillArticleName;
                v.DoorLeafArticleName = door.DoorLeafArticleName;
                v.DoorPassiveJambArticleName = door.DoorPassiveJambArticleName;
                v.DoorThresholdArticleName = door.DoorThresholdArticleName;
                v.DoorSillInsideW = door.DoorSillInsideW;
                v.DoorSillOutsideW = door.DoorSillOutsideW;
                v.DoorLeafInsideW = door.DoorLeafInsideW;
                v.DoorLeafOutsideW = door.DoorLeafInsideW;
                v.DoorPassiveJambInsideW = door.DoorPassiveJambInsideW;
                v.DoorPassiveJambOutsideW = door.DoorPassiveJambOutsideW;
                v.DoorSidelightSillArticleName = door.DoorSidelightSillArticleName;
                v.OutsideHandleArticleName = door.OutsideHandleArticleName;
                v.OutsideHandleColor = door.OutsideHandleColor;
                v.InsideHandleArticleName = door.InsideHandleArticleName;
                v.InsideHandleColor = door.InsideHandleColor;
                v.HingeCondition = door.HingeCondition;
                v.HingeArticleName = door.HingeArticleName;
                v.HingeColor = door.HingeColor;
                v.Finish = TD.Project.Color;

                // Framing Corner Points


                for (int i = 0; i < 4; i++)
                {
                    int memberIndex = g.BoundingMembers.ElementAt(i);
                    int sectionIndex = members.Single(x => x.MemberID == memberIndex).SectionID;
                    int memberType = members.ElementAt(memberIndex - 1).MemberType;
                    sideMembers[i] = memberIndex;
                    sideMemberTypes[i] = memberType;
                    sideMemberInsideW[i] = Model.ModelInput.Geometry.Sections.Single(x => x.SectionID == memberType).InsideW;
                    sideMemberOutsideW[i] = Model.ModelInput.Geometry.Sections.Single(x => x.SectionID == memberType).OutsideW;
                    int inode = members.ElementAt(memberIndex - 1).PointA;
                    offset[i] = VentOffset(Model, memberType, sectionIndex) * scale;
                    X[i] = geo.Points.ElementAt(inode - 1).X * scale;
                    Y[i] = geo.Points.ElementAt(inode - 1).Y * scale;

                }

                //Make second set of corner points for double doors

                int pointCount = 4;
                if(vent.VentOperableType == "Double-Door-Active-Right" || vent.VentOperableType == "Double-Door-Active-Left")
                {
                    pointCount = 8;
                }

                TraceData.Point[] pts = new TraceData.Point[pointCount];

                double active = (X[2] - X[0]) / 2;
                int passive = 0;        //If 0, no passive vent. If 1, passive vent on left for Double-Door-Active-Right. If 2, passive vent on right for Double-Door-Active-Left.

                //Set up corner points for single or double door situations

                if (vent.VentOperableType == "Single-Door-Right" || vent.VentOperableType == "Single-Door-Left")
                {
                    pts[0] = new TraceData.Point(X[0] + offset[0], Y[3] + offset[3]);
                    pts[1] = new TraceData.Point(X[0] + offset[0], Y[1] - offset[1]);
                    pts[2] = new TraceData.Point(X[2] - offset[2], Y[1] - offset[1]);
                    pts[3] = new TraceData.Point(X[2] - offset[2], Y[3] + offset[3]);
                }
                else if (vent.VentOperableType == "Double-Door-Active-Right")
                {
                    pts[0] = new TraceData.Point(X[0] + offset[0], Y[3] + offset[3]);
                    pts[1] = new TraceData.Point(X[0] + offset[0], Y[1] - offset[1]);
                    pts[2] = new TraceData.Point(X[2] - active + 15, Y[1] - offset[1]);
                    pts[3] = new TraceData.Point(X[2] - active + 15, Y[3] + offset[3]);
                    passive = 1;
                }
                else if (vent.VentOperableType == "Double-Door-Active-Left")
                {
                    pts[0] = new TraceData.Point(X[0] + active - 15, Y[3] + offset[3]);
                    pts[1] = new TraceData.Point(X[0] + active - 15, Y[1] - offset[1]);
                    pts[2] = new TraceData.Point(X[2] - offset[2], Y[1] - offset[1]);
                    pts[3] = new TraceData.Point(X[2] - offset[2], Y[3] + offset[3]);
                    passive = 2;
                }
                
                if(passive == 1)        //passive vent on left from inside view, goes with Double Door Active Right
                {
                    pts[4] = new TraceData.Point(X[0] + active - 10, Y[3] + offset[3]);
                    pts[5] = new TraceData.Point(X[0] + active - 10, Y[1] - offset[1]);
                    pts[6] = new TraceData.Point(X[2] - offset[2], Y[1] - offset[1]);
                    pts[7] = new TraceData.Point(X[2] - offset[2], Y[3] + offset[3]);
                }
                else if (passive == 2)      //passive vent on right from inside view, goes with Double Door Active Left
                {
                    pts[4] = new TraceData.Point(X[0] + offset[0], Y[3] + offset[3]);
                    pts[5] = new TraceData.Point(X[0] + offset[0], Y[1] - offset[1]);
                    pts[6] = new TraceData.Point(X[2] - active + 10, Y[1] - offset[1]);
                    pts[7] = new TraceData.Point(X[2] - active + 10, Y[3] + offset[3]);
                }

                // Add Corner Points

                if (passive == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        v.CornerPoints.Add(pts[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        v.CornerPoints.Add(pts[i]);
                    }
                }

                // Add Extusions

                CreateDoorVentExtrusions(ref v, scale, sideMemberTypes, sideMemberInsideW, sideMemberOutsideW, passive);

                // Update glass dimension based on the modified frame size Glass

                CreateDoorGlass(ref v, scale, passive);
                TD.Glazing.Panes[g.InfillID - 1].CornerPoints = v.DoorGlass[0].CornerPoints;
                TD.Glazing.Panes[g.InfillID - 1].Width = v.DoorGlass[0].Width;
                TD.Glazing.Panes[g.InfillID - 1].Height = v.DoorGlass[0].Height;
                v.Glass.Number = TD.Glazing.Panes[g.InfillID - 1].Number;

                if(passive != 0)
                {
                    var pane = new TraceData.Part();
                    pane.CornerPoints = v.DoorGlass[1].CornerPoints;
                    pane.Width = v.DoorGlass[1].Width;
                    pane.Height = v.DoorGlass[1].Height;
                    pane.Number = 33;
                    TD.Glazing.Panes.Add(pane);
                    
                }

                // Add Glazing Beads

                CreateDoorGlazingBeads(ref v, scale, passive, TD);


                // Add Overall Sizes

                v.Width = Math.Round(Math.Abs(pts[3].X - pts[0].X), 3);
                v.Height = Math.Round(Math.Abs(pts[1].Y - pts[0].Y), 3);
                v.ProfileDepth = 75;

                // Add Door Handle
                                                                                
                if (vent.VentOperableType == "Single-Door-Right")
                {
                    v.HandleLocation.X = pts[2].X - 32.5;
                    v.HandleLocation.Y = 1050;
                    v.HandleSide = "Left";
                    v.Type = "SingleDoor";
                }
                else if (vent.VentOperableType == "Single-Door-Left")
                {
                    v.HandleLocation.X = pts[0].X + 32.5; ;
                    v.HandleLocation.Y = 1050;
                    v.HandleSide = "Right";
                    v.Type = "SingleDoor";
                }
                else if (vent.VentOperableType == "Double-Door-Active-Right")
                {
                    v.HandleLocation.X = pts[2].X - 32.5;
                    v.HandleLocation.Y = 1050;
                    v.HandleSide = "Right";
                    v.Type = "DoubleDoor";
                }
                else if (vent.VentOperableType == "Double-Door-Active-Left")
                {
                    v.HandleLocation.X = pts[0].X + 32.5;
                    v.HandleLocation.Y = 1050;
                    v.HandleSide = "Left";
                    v.Type = "DoubleDoor";
                }
                else if (vent.VentOperableType == null)
                {
                    v.HandleLocation.X = 0;
                    v.HandleLocation.Y = 0;
                    v.HandleSide = null;
                    v.Type = "Fixed";
                }



               


                v.TextLocation = new TraceData.Point((pts[0].X + pts[2].X) / 2, (pts[0].Y + pts[2].Y) / 2 + 60);
                TD.Vents.Add(v);

            }
        }

        public static void CreateVentExtrusions(ref TraceData.Vent Vent, double Scale)
        {

            double offset = 30 * Scale;
            double width = Vent.ProfileOutsideWidth + 7 * Scale;
            double widthInside = Vent.ProfileInsideWidth;
            var e = new TraceData.Part();

            // left jamb

            e.ID = Vent.ID + "-LJ-01";
            e.Type = "Left Jamb";
            e.Profile = Vent.Profile;
            e.Quantity = 1;
            e.Finish = Vent.Finish;
            e.Length = Math.Abs((Vent.CornerPoints[1].Y) - (Vent.CornerPoints[0].Y));
            e.TextAngle = 90;
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset, Vent.CornerPoints[0].Y + offset));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset, Vent.CornerPoints[1].Y - offset));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset + width, Vent.CornerPoints[1].Y - offset - width));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset + width, Vent.CornerPoints[0].Y + offset + width));

            e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X, Vent.CornerPoints[0].Y));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X, Vent.CornerPoints[1].Y));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + widthInside, Vent.CornerPoints[1].Y - widthInside));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + widthInside, Vent.CornerPoints[0].Y + widthInside));

            e.EndCut = "45/45";
            e.DrawOnProposal = true;
            Vent.Extrusions.Add(e);


            // head

            e = new TraceData.Part();
            e.ID = Vent.ID + "-VH-02";
            e.Type = "Head";
            e.Profile = Vent.Profile;
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[2].X) - (Vent.CornerPoints[1].X));
            e.TextAngle = 0;
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset, Vent.CornerPoints[1].Y - offset));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset, Vent.CornerPoints[2].Y - offset));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset - width, Vent.CornerPoints[2].Y - offset - width));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset + width, Vent.CornerPoints[1].Y - offset - width));

            e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[3].Y) / 2);

            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X, Vent.CornerPoints[1].Y));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X, Vent.CornerPoints[2].Y));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - widthInside, Vent.CornerPoints[2].Y - widthInside));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + widthInside, Vent.CornerPoints[1].Y - widthInside));

            e.EndCut = "45/45";
            e.DrawOnProposal = true;
            Vent.Extrusions.Add(e);

            // right jamb

            e = new TraceData.Part();
            e.ID = Vent.ID + "-RJ-03";
            e.Type = "Right Jamb";
            e.Profile = Vent.Profile;
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[2].Y) - (Vent.CornerPoints[3].Y));
            e.TextAngle = 90;
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset, Vent.CornerPoints[2].Y - offset));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset, Vent.CornerPoints[3].Y + offset));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset - width, Vent.CornerPoints[3].Y + offset + width));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset - width, Vent.CornerPoints[2].Y - offset - width));

            e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X, Vent.CornerPoints[2].Y));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X, Vent.CornerPoints[3].Y));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - widthInside, Vent.CornerPoints[3].Y + widthInside));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - widthInside, Vent.CornerPoints[2].Y - widthInside));

            e.EndCut = "45/45";
            e.DrawOnProposal = true;
            Vent.Extrusions.Add(e);

            // sill

            e = new TraceData.Part();
            e.ID = Vent.ID + "-VS-04";
            e.Type = "Sill";
            e.Profile = Vent.Profile;
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[0].X) - (Vent.CornerPoints[3].X));
            e.TextAngle = 0;
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset, Vent.CornerPoints[3].Y + offset));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset, Vent.CornerPoints[0].Y + offset));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset + width, Vent.CornerPoints[0].Y + offset + width));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset - width, Vent.CornerPoints[3].Y + offset + width));

            e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[1].Y + e.CornerPoints[2].Y) / 2);

            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X, Vent.CornerPoints[3].Y));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X, Vent.CornerPoints[0].Y));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + widthInside, Vent.CornerPoints[0].Y + widthInside));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - widthInside, Vent.CornerPoints[3].Y + widthInside));

            e.EndCut = "45/45";
            e.DrawOnProposal = true;
            Vent.Extrusions.Add(e);

        }

        public static void CreateSlidingVentExtrusions(ref TraceData.Vent Vent, double Scale, List<SlidingDoorSystem>  SlidingList)
        {
            double interlockOffsetR = 31;
            double interlockOffsetL = 31;
            int ventCount = SlidingList[0].VentFrames.Count;
            double offset = 40 * Scale;
            double topOffset = 59 * Scale;
            double width = Vent.ProfileOutsideWidth * Scale;
            double widthInside = Vent.ProfileInsideWidth * Scale;
            var e = new TraceData.Part();
            double doubleVentOffsetR = 0;
            double doubleVentOffsetL = 0;
            bool InterlockReinforcement = SlidingList[0].InterlockReinforcement;

            //Find if there is a double vent to set interlock offset to zero at middle (otherwise interlocks will overlap by 51)

            if((ventCount == 4 && Vent.Number == 2) || (ventCount == 6 && Vent.Number == 3))
            {
                interlockOffsetR = 0;
                doubleVentOffsetR = 4;
            }
            else if ((ventCount == 4 && Vent.Number == 3) || (ventCount == 6 && Vent.Number == 4))
            {
                interlockOffsetL = 0;
                doubleVentOffsetL = 4;
            }
            

            if (Vent.Number == 1)   //first vent, left side has offset of outer frame (defined to only have interlock on right jamb, never double vent)
            {
                // left jamb

                e.ID = Vent.ID + "-LJ-01";
                e.Type = "Left Jamb";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset, Vent.CornerPoints[0].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset, Vent.CornerPoints[1].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset + width, Vent.CornerPoints[1].Y - offset - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset + width, Vent.CornerPoints[0].Y + offset + width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset, Vent.CornerPoints[0].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset, Vent.CornerPoints[1].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset + widthInside, Vent.CornerPoints[1].Y - offset - widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset + widthInside, Vent.CornerPoints[0].Y + offset + widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);


                // head

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VH-02";
                e.Type = "Head";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].X + interlockOffsetR) - (Vent.CornerPoints[1].X + offset));
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset, Vent.CornerPoints[1].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - width, Vent.CornerPoints[2].Y - offset - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset + width, Vent.CornerPoints[1].Y - offset - width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[3].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset, Vent.CornerPoints[1].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - widthInside, Vent.CornerPoints[2].Y - offset - widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset + widthInside, Vent.CornerPoints[1].Y - offset - widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // right jamb

                e = new TraceData.Part();
                e.ID = Vent.ID + "-RJ-03";
                e.Type = "Right Jamb";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].Y - topOffset) - (Vent.CornerPoints[3].Y + offset));
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - width, Vent.CornerPoints[3].Y + offset + width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - width, Vent.CornerPoints[2].Y - offset - width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - widthInside, Vent.CornerPoints[3].Y + offset + widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - widthInside, Vent.CornerPoints[2].Y - offset - widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // sill

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VS-04";
                e.Type = "Sill";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[0].X + offset) - (Vent.CornerPoints[3].X + interlockOffsetR));
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset, Vent.CornerPoints[0].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset + width, Vent.CornerPoints[0].Y + offset + width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - width, Vent.CornerPoints[3].Y + offset + width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[1].Y + e.CornerPoints[2].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset, Vent.CornerPoints[0].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset + widthInside, Vent.CornerPoints[0].Y + offset + widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X  + interlockOffsetR - widthInside, Vent.CornerPoints[3].Y + offset + widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // right jamb Cover Cap

                e = new TraceData.Part();
                e.ID = Vent.ID + "-CC-05";
                e.Type = "Interlock Cover Cap";
                e.Profile = "490240";
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].Y - topOffset) - (Vent.CornerPoints[3].Y + offset));
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR + 20, Vent.CornerPoints[3].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR + 20, Vent.CornerPoints[2].Y - offset));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR + 20, Vent.CornerPoints[3].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR + 20, Vent.CornerPoints[2].Y - offset));

                e.EndCut = "90/90";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                //One reinforcement for end unit, cover cap always the long length
                if (InterlockReinforcement == true)
                {
                    //Steel Tube
                    e = new TraceData.Part();
                    e.ID = Vent.ID + "-IR-01";
                    e.Type = "Steel Tube";
                    e.Profile = "201056"; //should get from unified, for now hard coded (as only one option)
                    e.Finish = "Galvanized Steel";
                    e.Quantity = 1;
                    e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                    e.TextAngle = 90;
                    e.EndCut = "90/90";
                    e.DrawOnProposal = false;

                    Vent.Extrusions.Add(e);

                    //Plastic Retaining Profile

                    e = new TraceData.Part();
                    e.ID = Vent.ID + "-IR-02";
                    e.Type = "Plastic Retention Profile";
                    e.Profile = "224129"; //should get from unified, for now hard coded (as only one option)
                    e.Finish = "Grey";
                    e.Quantity = 1;
                    e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                    e.TextAngle = 90;
                    e.EndCut = "90/90";
                    e.DrawOnProposal = false;
                   
                    Vent.Extrusions.Add(e);

                    //Cover Cap (Long) x1

                    e = new TraceData.Part();
                    e.ID = Vent.ID + "-IR-03";
                    e.Type = "Cover Profile";
                    e.Profile = "105620"; //should get from unified, for now hard coded (as only one option)
                    e.Finish = Vent.Finish;
                    e.Quantity = 1;
                    e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                    e.TextAngle = 90;
                    e.EndCut = "90/90";
                    e.DrawOnProposal = false;

                    Vent.Extrusions.Add(e);
                }

            }
            else if (Vent.Number == SlidingList[0].VentFrames.Count)        //last vent, right side has offset of outer frame
            {
                // left jamb

                e.ID = Vent.ID + "-LJ-01";
                e.Type = "Left Jamb";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + width, Vent.CornerPoints[1].Y - offset - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + width, Vent.CornerPoints[0].Y + offset + width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + widthInside, Vent.CornerPoints[1].Y - offset - widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + widthInside, Vent.CornerPoints[0].Y + offset + widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);


                // head

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VH-02";
                e.Type = "Head";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].X - offset) - (Vent.CornerPoints[1].X - interlockOffsetL));
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset, Vent.CornerPoints[2].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset - width, Vent.CornerPoints[2].Y - offset - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL+ width, Vent.CornerPoints[1].Y - offset - width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[3].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset, Vent.CornerPoints[2].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset - widthInside, Vent.CornerPoints[2].Y - offset - widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + widthInside, Vent.CornerPoints[1].Y - offset - widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // right jamb

                e = new TraceData.Part();
                e.ID = Vent.ID + "-RJ-03";
                e.Type = "Right Jamb";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].Y - topOffset) - (Vent.CornerPoints[3].Y + offset));
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset, Vent.CornerPoints[2].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset, Vent.CornerPoints[3].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset - width, Vent.CornerPoints[3].Y + offset + width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset - width, Vent.CornerPoints[2].Y - offset - width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset, Vent.CornerPoints[2].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset, Vent.CornerPoints[3].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset - widthInside, Vent.CornerPoints[3].Y + offset + widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset - widthInside, Vent.CornerPoints[2].Y - offset - widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // sill

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VS-04";
                e.Type = "Sill";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[3].X - offset) - (Vent.CornerPoints[0].X - interlockOffsetL));
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset, Vent.CornerPoints[3].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + width, Vent.CornerPoints[0].Y + offset + width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset - width, Vent.CornerPoints[3].Y + offset + width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[1].Y + e.CornerPoints[2].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset , Vent.CornerPoints[3].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + widthInside, Vent.CornerPoints[0].Y + offset + widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset - widthInside, Vent.CornerPoints[3].Y + offset + widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // left jamb Cover Cap

                e = new TraceData.Part();
                e.ID = Vent.ID + "-CC-05";
                e.Type = "Interlock Cover Cap";
                e.Profile = "490240";
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL - 20, Vent.CornerPoints[1].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL - 20, Vent.CornerPoints[0].Y + offset));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL - 20, Vent.CornerPoints[1].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL - 20, Vent.CornerPoints[0].Y + offset));

                e.EndCut = "90/90";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                //One reinforcement for end unit, cover cap always the long length
                if (InterlockReinforcement == true)
                {
                    //Steel Tube
                    e = new TraceData.Part();
                    e.ID = Vent.ID + "-IR-01";
                    e.Type = "Steel Tube";
                    e.Profile = "201056"; //should get from unified, for now hard coded (as only one option)
                    e.Finish = "Galvanized Steel";
                    e.Quantity = 1;
                    e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                    e.TextAngle = 90;
                    e.EndCut = "90/90";
                    e.DrawOnProposal = false;

                    Vent.Extrusions.Add(e);

                    //Plastic Retaining Profile

                    e = new TraceData.Part();
                    e.ID = Vent.ID + "-IR-02";
                    e.Type = "Plastic Retention Profile";
                    e.Profile = "224129"; //should get from unified, for now hard coded (as only one option)
                    e.Finish = "Grey";
                    e.Quantity = 1;
                    e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                    e.TextAngle = 90;
                    e.EndCut = "90/90";
                    e.DrawOnProposal = false;

                    Vent.Extrusions.Add(e);

                    //Cover Cap (Long) x1

                    e = new TraceData.Part();
                    e.ID = Vent.ID + "-IR-03";
                    e.Type = "Cover Profile";
                    e.Profile = "105620"; //should get from unified, for now hard coded (as only one option)
                    e.Finish = Vent.Finish;
                    e.Quantity = 1;
                    e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                    e.TextAngle = 90;
                    e.EndCut = "90/90";
                    e.DrawOnProposal = false;

                    Vent.Extrusions.Add(e);
                }
            }
            else    //in-between vents have no offset on sides
            {
                // left jamb

                e.ID = Vent.ID + "-LJ-01";
                e.Type = "Left Jamb";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + doubleVentOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + doubleVentOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + doubleVentOffsetL + width, Vent.CornerPoints[1].Y - offset - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + doubleVentOffsetL + width, Vent.CornerPoints[0].Y + offset + width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + doubleVentOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + doubleVentOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + doubleVentOffsetL + widthInside, Vent.CornerPoints[1].Y - offset - widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + doubleVentOffsetL + widthInside, Vent.CornerPoints[0].Y + offset + widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);


                // head

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VH-02";
                e.Type = "Head";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].X + interlockOffsetR) - (Vent.CornerPoints[1].X - interlockOffsetL) - (doubleVentOffsetL + doubleVentOffsetR));
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + doubleVentOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - doubleVentOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - doubleVentOffsetR - width, Vent.CornerPoints[2].Y - offset - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + doubleVentOffsetL + width, Vent.CornerPoints[1].Y - offset - width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[3].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + doubleVentOffsetL, Vent.CornerPoints[1].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - doubleVentOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - doubleVentOffsetR - widthInside, Vent.CornerPoints[2].Y - offset - widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL + doubleVentOffsetL + widthInside, Vent.CornerPoints[1].Y - offset - widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // right jamb

                e = new TraceData.Part();
                e.ID = Vent.ID + "-RJ-03";
                e.Type = "Right Jamb";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].Y - topOffset) - (Vent.CornerPoints[3].Y + offset));
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - doubleVentOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - doubleVentOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - doubleVentOffsetR - width, Vent.CornerPoints[3].Y + offset + width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - doubleVentOffsetR - width, Vent.CornerPoints[2].Y - offset - width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - doubleVentOffsetR, Vent.CornerPoints[2].Y - offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - doubleVentOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - doubleVentOffsetR - widthInside, Vent.CornerPoints[3].Y + offset + widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR - doubleVentOffsetR - widthInside, Vent.CornerPoints[2].Y - offset - widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // sill

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VS-04";
                e.Type = "Sill";
                e.Profile = Vent.Profile;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[0].X - interlockOffsetL) - (Vent.CornerPoints[3].X + interlockOffsetR) + (doubleVentOffsetL + doubleVentOffsetR));
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - doubleVentOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + doubleVentOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + doubleVentOffsetL + width, Vent.CornerPoints[0].Y + offset + width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - doubleVentOffsetR - width, Vent.CornerPoints[3].Y + offset + width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[1].Y + e.CornerPoints[2].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - doubleVentOffsetR, Vent.CornerPoints[3].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + doubleVentOffsetL, Vent.CornerPoints[0].Y + offset));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL + doubleVentOffsetL + widthInside, Vent.CornerPoints[0].Y + offset + widthInside));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR - doubleVentOffsetR - widthInside, Vent.CornerPoints[3].Y + offset + widthInside));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                if(interlockOffsetL != 0) //if there is a left interlock offset, then left jamb cover cap should be drawn
                {
                    // left jamb Cover Cap

                    e = new TraceData.Part();
                    e.ID = Vent.ID + "-CC-05";
                    e.Type = "Interlock Cover Cap";
                    e.Profile = Vent.Profile;
                    e.Finish = Vent.Finish;
                    e.Quantity = 1;
                    e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                    e.TextAngle = 90;
                    e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL, Vent.CornerPoints[0].Y + offset));
                    e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL, Vent.CornerPoints[1].Y - offset));
                    e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL - 20, Vent.CornerPoints[1].Y - offset));
                    e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL - 20, Vent.CornerPoints[0].Y + offset));

                    e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                    e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL, Vent.CornerPoints[0].Y + offset));
                    e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL, Vent.CornerPoints[1].Y - offset));
                    e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X - interlockOffsetL - 20, Vent.CornerPoints[1].Y - offset));
                    e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X - interlockOffsetL - 20, Vent.CornerPoints[0].Y + offset));

                    e.EndCut = "90/90";
                    e.DrawOnProposal = true;
                    Vent.Extrusions.Add(e);
                }

                if(interlockOffsetR != 0) // if there is a right interlock offset, then right jamb cover cap should be drawn
                {
                    // right jamb Cover Cap

                    e = new TraceData.Part();
                    e.ID = Vent.ID + "-CC-05";
                    e.Type = "Interlock Cover Cap";
                    e.Profile = Vent.Profile;
                    e.Finish = Vent.Finish;
                    e.Quantity = 1;
                    e.Length = Math.Abs((Vent.CornerPoints[2].Y - topOffset) - (Vent.CornerPoints[3].Y + offset));
                    e.TextAngle = 90;
                    e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR, Vent.CornerPoints[2].Y - offset));
                    e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR, Vent.CornerPoints[3].Y + offset));
                    e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR + 20, Vent.CornerPoints[3].Y + offset));
                    e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR + 20, Vent.CornerPoints[2].Y - offset));

                    e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                    e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR, Vent.CornerPoints[2].Y - offset));
                    e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR, Vent.CornerPoints[3].Y + offset));
                    e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X + interlockOffsetR + 20, Vent.CornerPoints[3].Y + offset));
                    e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X + interlockOffsetR + 20, Vent.CornerPoints[2].Y - offset));

                    e.EndCut = "90/90";
                    e.DrawOnProposal = true;
                    Vent.Extrusions.Add(e);
                }

                //Interlock for in-between units. Quantity and cover cap length based on Vent number and vent count
                if (InterlockReinforcement == true)
                {
                    if (ventCount == 3) //Middle vent will have one interlock with short cover cap
                    {
                        //Steel Tube
                        e = new TraceData.Part();
                        e.ID = Vent.ID + "-IR-01";
                        e.Type = "Steel Tube";
                        e.Profile = "201056"; //should get from unified, for now hard coded (as only one option)
                        e.Finish = "Galvanized Steel";
                        e.Quantity = 1;
                        e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                        e.TextAngle = 90;
                        e.EndCut = "90/90";
                        e.DrawOnProposal = false;

                        Vent.Extrusions.Add(e);

                        //Plastic Retaining Profile

                        e = new TraceData.Part();
                        e.ID = Vent.ID + "-IR-02";
                        e.Type = "Plastic Retention Profile";
                        e.Profile = "224129"; //should get from unified, for now hard coded (as only one option)
                        e.Finish = "Grey";
                        e.Quantity = 1;
                        e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                        e.TextAngle = 90;
                        e.EndCut = "90/90";
                        e.DrawOnProposal = false;

                        Vent.Extrusions.Add(e);

                        //Cover Cap (Short) x1

                        e = new TraceData.Part();
                        e.ID = Vent.ID + "-IR-03";
                        e.Type = "Cover Profile";
                        e.Profile = "105620"; //should get from unified, for now hard coded (as only one option)
                        e.Finish = Vent.Finish;
                        e.Quantity = 1;
                        e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 40;
                        e.TextAngle = 90;
                        e.EndCut = "90/90";
                        e.DrawOnProposal = false;

                        Vent.Extrusions.Add(e);

                    }
                    else if (ventCount == 4) //in-between units have two reinforcements each, one short and one long cover cap
                    {
                        //Steel Tube
                        e = new TraceData.Part();
                        e.ID = Vent.ID + "-IR-01";
                        e.Type = "Steel Tube";
                        e.Profile = "201056"; //should get from unified, for now hard coded (as only one option)
                        e.Finish = "Galvanized Steel";
                        e.Quantity = 1;
                        e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                        e.TextAngle = 90;
                        e.EndCut = "90/90";
                        e.DrawOnProposal = false;

                        Vent.Extrusions.Add(e);
                        Vent.Extrusions.Add(e);

                        //Plastic Retaining Profile

                        e = new TraceData.Part();
                        e.ID = Vent.ID + "-IR-02";
                        e.Type = "Plastic Retention Profile";
                        e.Profile = "224129"; //should get from unified, for now hard coded (as only one option)
                        e.Finish = "Grey";
                        e.Quantity = 1;
                        e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                        e.TextAngle = 90;
                        e.EndCut = "90/90";
                        e.DrawOnProposal = false;

                        Vent.Extrusions.Add(e);
                        Vent.Extrusions.Add(e);

                        //Cover Cap (Long) x1

                        e = new TraceData.Part();
                        e.ID = Vent.ID + "-IR-03";
                        e.Type = "Cover Profile";
                        e.Profile = "105620"; //should get from unified, for now hard coded (as only one option)
                        e.Finish = Vent.Finish;
                        e.Quantity = 1;
                        e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                        e.TextAngle = 90;
                        e.EndCut = "90/90";
                        e.DrawOnProposal = false;

                        Vent.Extrusions.Add(e);

                        //Cover Cap (Short) x1

                        e = new TraceData.Part();
                        e.ID = Vent.ID + "-IR-04";
                        e.Type = "Cover Profile";
                        e.Profile = "105620"; //should get from unified, for now hard coded (as only one option)
                        e.Finish = Vent.Finish;
                        e.Quantity = 1;
                        e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 40;
                        e.TextAngle = 90;
                        e.EndCut = "90/90";
                        e.DrawOnProposal = false;

                        Vent.Extrusions.Add(e);
                    }
                    else if (ventCount == 6) //units 2 and 5 will have one reinforcement with short cap, while 3 and 4 have two reinforcement each, one short and one long
                    {
                        if (Vent.Number == 2 || Vent.Number == 5)
                        {
                            //Steel Tube
                            e = new TraceData.Part();
                            e.ID = Vent.ID + "-IR-01";
                            e.Type = "Steel Tube";
                            e.Profile = "201056"; //should get from unified, for now hard coded (as only one option)
                            e.Finish = "Galvanized Steel";
                            e.Quantity = 1;
                            e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                            e.TextAngle = 90;
                            e.EndCut = "90/90";
                            e.DrawOnProposal = false;

                            Vent.Extrusions.Add(e);

                            //Plastic Retaining Profile

                            e = new TraceData.Part();
                            e.ID = Vent.ID + "-IR-02";
                            e.Type = "Plastic Retention Profile";
                            e.Profile = "224129"; //should get from unified, for now hard coded (as only one option)
                            e.Finish = "Grey";
                            e.Quantity = 1;
                            e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                            e.TextAngle = 90;
                            e.EndCut = "90/90";
                            e.DrawOnProposal = false;

                            Vent.Extrusions.Add(e);

                            //Cover Cap (Short) x1

                            e = new TraceData.Part();
                            e.ID = Vent.ID + "-IR-03";
                            e.Type = "Cover Profile";
                            e.Profile = "105620"; //should get from unified, for now hard coded (as only one option)
                            e.Finish = Vent.Finish;
                            e.Quantity = 1;
                            e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 40;
                            e.TextAngle = 90;
                            e.EndCut = "90/90";
                            e.DrawOnProposal = false;

                            Vent.Extrusions.Add(e);
                        }
                        else if (Vent.Number == 3 || Vent.Number == 4)
                        {
                            //Steel Tube
                            e = new TraceData.Part();
                            e.ID = Vent.ID + "-IR-01";
                            e.Type = "Steel Tube";
                            e.Profile = "201056"; //should get from unified, for now hard coded (as only one option)
                            e.Finish = "Galvanized Steel";
                            e.Quantity = 1;
                            e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                            e.TextAngle = 90;
                            e.EndCut = "90/90";
                            e.DrawOnProposal = false;

                            Vent.Extrusions.Add(e);
                            Vent.Extrusions.Add(e);

                            //Plastic Retaining Profile

                            e = new TraceData.Part();
                            e.ID = Vent.ID + "-IR-02";
                            e.Type = "Plastic Retention Profile";
                            e.Profile = "224129"; //should get from unified, for now hard coded (as only one option)
                            e.Finish = "Grey";
                            e.Quantity = 1;
                            e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 50;
                            e.TextAngle = 90;
                            e.EndCut = "90/90";
                            e.DrawOnProposal = false;

                            Vent.Extrusions.Add(e);
                            Vent.Extrusions.Add(e);

                            //Cover Cap (Long) x1

                            e = new TraceData.Part();
                            e.ID = Vent.ID + "-IR-03";
                            e.Type = "Cover Profile";
                            e.Profile = "105620"; //should get from unified, for now hard coded (as only one option)
                            e.Finish = Vent.Finish;
                            e.Quantity = 1;
                            e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset));
                            e.TextAngle = 90;
                            e.EndCut = "90/90";
                            e.DrawOnProposal = false;

                            Vent.Extrusions.Add(e);

                            //Cover Cap (Short) x1

                            e = new TraceData.Part();
                            e.ID = Vent.ID + "-IR-04";
                            e.Type = "Cover Profile";
                            e.Profile = "105620"; //should get from unified, for now hard coded (as only one option)
                            e.Finish = Vent.Finish;
                            e.Quantity = 1;
                            e.Length = Math.Abs((Vent.CornerPoints[1].Y - topOffset) - (Vent.CornerPoints[0].Y + offset)) - 40;
                            e.TextAngle = 90;
                            e.EndCut = "90/90";
                            e.DrawOnProposal = false;

                            Vent.Extrusions.Add(e);
                        }
                    }
                }

            }
        }

        public static void CreateSlidingExtraVentExtrusions(TraceData TD)
        {
            //These are required for vent frames of ASE. Hard coding with ASE 60 articles for now, in future could expand to include ASE 80 articles too.
            int index = 1;
            foreach (var v in TD.Vents)
            {
                //220777 Interlock Profile

                var e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-01";
                e.Profile = "220777";
                e.Length = v.Extrusions.Single(x => x.Type == "Right Jamb").Length - 30;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Interlock Profile";
                e.Finish = "Black";

                //2A: vent 0 has 1, vent 1 has 1 (2)
                //2A/1: vent 0 has 1, vent 1 has 1 (2)
                //2D/1: vent 0 has 1, vent 1 has 1, vent 2 has 1, vent 3 has 1 (4)
                //3E: vent 0 has 1, vent 1 has 2, vent 2 has 1 (4)
                //3E/1: vent 0 has 1, vent 1 has 2, vent 2 has 1 (4)
                //3F: vent 0 has 1, vent 1 has 2, vent 2 has 1, vent 3 has 1, vent 4 has 2, vent 5 has 1 (8)

                if (TD.Project.VentOperableType.Contains("2A"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("2D"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("3E"))
                {
                    if (index == 2)
                    {
                        v.Extrusions.Add(e);
                        v.Extrusions.Add(e);
                    }
                    else
                    {
                        v.Extrusions.Add(e);
                    }
                }
                else if (TD.Project.VentOperableType.Contains("3F"))
                {
                    if (index == 2 || index == 5)
                    {
                        v.Extrusions.Add(e);
                        v.Extrusions.Add(e);
                    }
                    else
                    {
                        v.Extrusions.Add(e);
                    }
                }

                //220787 Cover Profile (for interlock profile)

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-02";
                e.Profile = "220787";
                e.Length = TD.Vents[0].Extrusions.Single(x => x.Type == "Right Jamb").Length - 30;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Cover Profile";
                e.Finish = "Black";

                //2A: vent 0 has 1, vent 1 has 1 (2)
                //2A/1: vent 0 has 1, vent 1 has 1 (2)
                //2D/1: vent 0 has 1, vent 1 has 1, vent 2 has 1, vent 3 has 1 (4)
                //3E: vent 0 has 1, vent 1 has 2, vent 2 has 1 (4)
                //3E/1: vent 0 has 1, vent 1 has 2, vent 2 has 1 (4)
                //3F: vent 0 has 1, vent 1 has 2, vent 2 has 1, vent 3 has 1, vent 4 has 2, vent 5 has 1 (8)

                if (TD.Project.VentOperableType.Contains("2A"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("2D"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("3E"))
                {
                    if (index == 2)
                    {
                        v.Extrusions.Add(e);
                        v.Extrusions.Add(e);
                    }
                    else
                    {
                        v.Extrusions.Add(e);
                    }
                }
                else if (TD.Project.VentOperableType.Contains("3F"))
                {
                    if (index == 2 || index == 5)
                    {
                        v.Extrusions.Add(e);
                        v.Extrusions.Add(e);
                    }
                    else
                    {
                        v.Extrusions.Add(e);
                    }
                }


                //490240 Cover Profile

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-03";
                e.Profile = "490240";
                e.Length = TD.Vents[0].Extrusions.Single(x => x.Type == "Right Jamb").Length;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Cover Profile";
                e.Finish = TD.Project.Color;

                //2A: vent 0 has 1, vent 1 has 1 (2)
                //2A/1: vent 0 has 1, vent 1 has 1 (2)
                //2D/1: vent 0 has 1, vent 1 has 1, vent 2 has 1, vent 3 has 1 (4)
                //3E: vent 0 has 1, vent 1 has 2, vent 2 has 1 (4)
                //3E/1: vent 0 has 1, vent 1 has 2, vent 2 has 1 (4)
                //3F: vent 0 has 1, vent 1 has 2, vent 2 has 1, vent 3 has 1, vent 4 has 2, vent 5 has 1 (8)

                if (TD.Project.VentOperableType.Contains("2A"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("2D"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("3E"))
                {
                    if (index == 2)
                    {
                        v.Extrusions.Add(e);
                        v.Extrusions.Add(e);
                    }
                    else
                    {
                        v.Extrusions.Add(e);
                    }
                }
                else if (TD.Project.VentOperableType.Contains("3F"))
                {
                    if (index == 2 || index == 5)
                    {
                        v.Extrusions.Add(e);
                        v.Extrusions.Add(e);
                    }
                    else
                    {
                        v.Extrusions.Add(e);
                    }
                }

                //513000 Structural Profile

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-04";
                e.Profile = "513000";
                e.Length = TD.Vents[0].Extrusions.Single(x => x.Type == "Right Jamb").Length - 140;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Structural Profile";
                e.Finish = TD.Project.Color;

                //2A: vent 0 has 1, vent 1 has 1 (2)
                //2A/1: vent 0 has 1, vent 1 has 1 (2)
                //2D/1: vent 0 has 1, vent 1 has 1, vent 2 has 1, vent 3 has 1 (4)
                //3E: vent 0 has 1, vent 1 has 1, vent 2 has 1 (3)
                //3E/1: vent 0 has 1, vent 1 has 1, vent 2 has 1 (3)
                //3F: vent 0 has 1, vent 1 has 1, vent 2 has 1, vent 3 has 1, vent 4 has 1, vent 5 has 1 (6)

                if (TD.Project.VentOperableType.Contains("2A"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("2D"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("3E"))
                {
                    v.Extrusions.Add(e);
                }
                else if (TD.Project.VentOperableType.Contains("3F"))
                {
                    v.Extrusions.Add(e);
                }

                //278419 Slider Profile

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-05";
                e.Profile = "278419";
                e.Length = TD.Vents[0].Extrusions.Single(x => x.Type == "Head").Length - 110;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Slider Profile";
                e.Finish = "Black";

                v.Extrusions.Add(e);
                v.Extrusions.Add(e);

                //278852 Cover Profile

                var ventcount = TD.Vents.Count;

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-06";
                e.Profile = "278852";
                e.Length = TD.Vents[0].Extrusions.Single(x => x.Type == "Right Jamb").Length - 62;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Cover Profile";
                e.Finish = "Black";

                if (v.Type == "Sliding")
                {
                    if ((ventcount == 4 && v.Number == 3) || (ventcount == 6 && v.Number == 2) || (ventcount == 6 && v.Number == 4) || (ventcount == 6 && v.Number == 5) || (ventcount == 3 && v.Number == 2))
                    {
                        
                    }
                    else
                    {
                        v.Extrusions.Add(e);
                    }
                }


                //489970 Double Vent Profile

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-07";
                e.Profile = "489970";
                e.Length = TD.Vents[0].Extrusions.Single(x => x.Type == "Right Jamb").Length - 62;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Double Vent Profile";
                e.Finish = TD.Project.Color;


                if ((ventcount == 4 && v.Number == 3) || (ventcount == 6 && v.Number == 4))
                {
                    v.Extrusions.Add(e);
                }
                


                //278436 Cover Profile, Double Vent

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-08";
                e.Profile = "278436";
                e.Length = TD.Vents[0].Extrusions.Single(x => x.Type == "Right Jamb").Length - 62;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Cover Profile";
                e.Finish = "Black";


                if ((ventcount == 4 && v.Number == 3) || (ventcount == 6 && v.Number == 4))
                {
                    v.Extrusions.Add(e);
                }
                

                //246434 Glazing Bead Extension Profile

                e = new TraceData.Part();

                e.ID = TD.Project.OrderNumber + "-V" + index.ToString() + "-VP-09";
                e.Profile = "246434";
                e.Length = TD.Vents[0].Extrusions.Single(x => x.Type == "Right Jamb").Length - 164;
                e.DrawOnProposal = false;
                e.EndCut = "90/90";
                e.Quantity = 1;
                e.Type = "Glazing Bead Extension Profile";
                e.Finish = "Black";

                //If on track 1, has 1. If triple track and on track 2, has 1. 

                if (v.Track == 1)
                {
                    v.Extrusions.Add(e);
                }
                else if (v.Track == 2 && ((ventcount == 3) || (ventcount == 6)))
                {
                    v.Extrusions.Add(e);
                }
                

                index++;
            }
            //Foams, gaskets, etc. should go in loose parts

        }

        public static void CreateDoorVentExtrusions(ref TraceData.Vent Vent, double Scale, int[] sideMemberTypes, double[] sideMembersInsideW, double[] sideMembersOutsideW, int Passive)
        {

            double[] insideOffset = new double[4];
            double[] outsideOffset = new double[4];
            double width = Vent.DoorLeafOutsideW;
            double widthInside = Vent.DoorLeafInsideW;
            double sillWidth = Vent.DoorSillOutsideW;
            double sillInsideWidth = Vent.DoorSillInsideW;
            double passiveWidth = Vent.DoorPassiveJambOutsideW;
            double passiveInsideWidth = Vent.DoorPassiveJambInsideW;
            var e = new TraceData.Part();

            //set offsets

            for (var i = 0; i < 4; i++)
            {
                if (sideMemberTypes[i] == 1)
                {
                    insideOffset[i] = sideMembersInsideW[i] - 5;
                    outsideOffset[i] = sideMembersOutsideW[i] - 14;
                }
                else if (sideMemberTypes[i] == 31)
                {
                    insideOffset[i] = 8;
                    outsideOffset[i] = 8;
                }
                else
                {
                    insideOffset[i] = sideMembersInsideW[i] / 2 - 5;
                    outsideOffset[i] = sideMembersOutsideW[i] / 2 - 5;
                }
            }

            double horOffset = 20;

            if (Passive == 1 || Passive == 2)
            {
                horOffset = 15;
            }

            // left jamb

            e.ID = Vent.ID + "-LJ-01";
            e.Type = "Left Jamb";
            e.Profile = Vent.DoorLeafArticleName;
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[1].Y - 18) - (Vent.CornerPoints[0].Y));
            e.TextAngle = 90;
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + outsideOffset[0], Vent.CornerPoints[0].Y + outsideOffset[3]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + outsideOffset[0], Vent.CornerPoints[1].Y - outsideOffset[0]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + outsideOffset[0] + width, Vent.CornerPoints[1].Y - outsideOffset[0] - width));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + outsideOffset[0] + width, Vent.CornerPoints[0].Y + outsideOffset[3]));

            e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);
                        
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + 5, Vent.CornerPoints[0].Y + 8));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + 5, Vent.CornerPoints[1].Y - 5));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + 5 + widthInside, Vent.CornerPoints[1].Y - widthInside - 5));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + 5 + widthInside, Vent.CornerPoints[0].Y + 8));

            e.EndCut = "90/45";
            e.DrawOnProposal = true;
            Vent.Extrusions.Add(e);


            // head

            e = new TraceData.Part();
            e.ID = Vent.ID + "-VH-02";
            e.Type = "Head";
            e.Profile = Vent.DoorLeafArticleName;
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[2].X - horOffset) - Vent.CornerPoints[1].X);
            e.TextAngle = 0;
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + outsideOffset[1], Vent.CornerPoints[1].Y - outsideOffset[1]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - outsideOffset[1], Vent.CornerPoints[2].Y - outsideOffset[1]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - outsideOffset[1] - width, Vent.CornerPoints[2].Y - outsideOffset[1] - width));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + outsideOffset[1] + width, Vent.CornerPoints[1].Y - outsideOffset[1] - width));

            e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[3].Y) / 2);

            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + 5, Vent.CornerPoints[1].Y - 5));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - 5, Vent.CornerPoints[2].Y - 5));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - 5 - widthInside, Vent.CornerPoints[2].Y - 5 - widthInside));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[1].X + 5 + widthInside, Vent.CornerPoints[1].Y - 5 - widthInside));

            e.EndCut = "45/45";
            e.DrawOnProposal = true;
            Vent.Extrusions.Add(e);

            // right jamb

            e = new TraceData.Part();
            e.ID = Vent.ID + "-RJ-03";
            e.Type = "Right Jamb";
            e.Profile = Vent.DoorLeafArticleName;
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[2].Y - 18)- Vent.CornerPoints[3].Y);
            e.TextAngle = 90;
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - outsideOffset[2], Vent.CornerPoints[2].Y - outsideOffset[2]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - outsideOffset[2], Vent.CornerPoints[3].Y + outsideOffset[3]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - outsideOffset[2] - width, Vent.CornerPoints[3].Y + outsideOffset[3]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - outsideOffset[2] - width, Vent.CornerPoints[2].Y - outsideOffset[2] - width));

            e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - 5, Vent.CornerPoints[2].Y - 5));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - 5, Vent.CornerPoints[3].Y + 8));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - 5 - widthInside, Vent.CornerPoints[3].Y + 8));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[2].X - 5 - widthInside, Vent.CornerPoints[2].Y - widthInside - 5));

            e.EndCut = "45/90";
            e.DrawOnProposal = true;
            Vent.Extrusions.Add(e);

            // sill

            
            e = new TraceData.Part();
            e.ID = Vent.ID + "-VS-04";
            e.Type = "Sill";
            e.Profile = Vent.DoorSillArticleName;
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[0].X + width + horOffset) - (Vent.CornerPoints[2].X - width));
            e.TextAngle = 0;
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - outsideOffset[2] - width, Vent.CornerPoints[3].Y + outsideOffset[3]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + outsideOffset[0] + width, Vent.CornerPoints[0].Y + outsideOffset[3]));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + outsideOffset[0] + width, Vent.CornerPoints[0].Y + outsideOffset[3] + sillWidth));
            e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - outsideOffset[2] - width, Vent.CornerPoints[3].Y + outsideOffset[3] + sillWidth));

            e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[1].Y + e.CornerPoints[2].Y) / 2);

            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - width - 5, Vent.CornerPoints[3].Y + 8));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + width + 5, Vent.CornerPoints[0].Y + 8));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[0].X + width + 5, Vent.CornerPoints[0].Y + 8 + sillInsideWidth));
            e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[3].X - width - 5, Vent.CornerPoints[3].Y + 8 + sillInsideWidth));

            e.EndCut = "90/90";
            e.DrawOnProposal = true;
            Vent.Extrusions.Add(e);

            if (Passive == 1)    //passive on left from inside view
            {
                // passive left jamb (middle)

                e = new TraceData.Part();
                e.ID = Vent.ID + "-LJ-05";
                e.Type = "Passive Left Jamb";
                e.Profile = Vent.DoorPassiveJambArticleName;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[1].Y - 18) - Vent.CornerPoints[0].Y);
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X, Vent.CornerPoints[0].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X, Vent.CornerPoints[1].Y - outsideOffset[0]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + (passiveWidth - width), Vent.CornerPoints[1].Y - outsideOffset[0]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + passiveWidth, Vent.CornerPoints[1].Y - outsideOffset[0] - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + passiveWidth, Vent.CornerPoints[0].Y + outsideOffset[3]));
                

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[4].X + (widthInside - passiveInsideWidth), Vent.CornerPoints[0].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + (widthInside - passiveInsideWidth), Vent.CornerPoints[1].Y - (widthInside - passiveInsideWidth) - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + (widthInside - passiveInsideWidth) + passiveInsideWidth, Vent.CornerPoints[1].Y - widthInside - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[4].X + (widthInside - passiveInsideWidth) + passiveInsideWidth, Vent.CornerPoints[0].Y + 8));

                e.EndCut = "90/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);


                // passive head

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VH-06";
                e.Type = "Passive Head";
                e.Profile = Vent.DoorLeafArticleName;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].X - 15) - Vent.CornerPoints[1].X);
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + passiveWidth - width, Vent.CornerPoints[1].Y - outsideOffset[1]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - outsideOffset[1], Vent.CornerPoints[2].Y - outsideOffset[1]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - outsideOffset[1] - width, Vent.CornerPoints[2].Y - outsideOffset[1] - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + passiveWidth, Vent.CornerPoints[1].Y - outsideOffset[1] - width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[3].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + (widthInside - passiveInsideWidth), Vent.CornerPoints[1].Y - 5));      //top right point
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - 5, Vent.CornerPoints[2].Y - 5));     //top left point 
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - 5 - widthInside, Vent.CornerPoints[2].Y - widthInside - 5));      //bottom left point
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + (widthInside - passiveInsideWidth) + passiveInsideWidth, Vent.CornerPoints[1].Y - widthInside - 5));  //bottom right point
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + (widthInside - passiveInsideWidth), Vent.CornerPoints[1].Y - (widthInside - passiveInsideWidth) - 5));    //middle right point

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // right passive jamb

                e = new TraceData.Part();
                e.ID = Vent.ID + "-RJ-07";
                e.Type = "Right Jamb";
                e.Profile = Vent.DoorLeafArticleName;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].Y - 18) - Vent.CornerPoints[3].Y);
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - outsideOffset[2], Vent.CornerPoints[2].Y - outsideOffset[2]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - outsideOffset[2], Vent.CornerPoints[3].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - outsideOffset[2] - width, Vent.CornerPoints[3].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - outsideOffset[2] - width, Vent.CornerPoints[2].Y - outsideOffset[2] - width));
                
                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - 5, Vent.CornerPoints[2].Y - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[7].X - 5, Vent.CornerPoints[3].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[7].X - 5 - widthInside, Vent.CornerPoints[3].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - 5 - widthInside, Vent.CornerPoints[2].Y - widthInside - 5));

                e.EndCut = "45/90";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // passive sill

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VS-08";
                e.Type = "Sill";
                e.Profile = Vent.DoorSillArticleName;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[0].X + width + 15) - (Vent.CornerPoints[2].X - width));
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - outsideOffset[2] - width, Vent.CornerPoints[3].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + passiveWidth, Vent.CornerPoints[0].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + passiveWidth, Vent.CornerPoints[0].Y + outsideOffset[3] + sillWidth));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - outsideOffset[2] - width, Vent.CornerPoints[3].Y + outsideOffset[3] + sillWidth));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[1].Y + e.CornerPoints[2].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[7].X - 5 - (widthInside - passiveInsideWidth) - passiveInsideWidth, Vent.CornerPoints[3].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[4].X + width, Vent.CornerPoints[0].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[4].X + width, Vent.CornerPoints[0].Y + 8 + sillInsideWidth));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[7].X - 5 - (widthInside - passiveInsideWidth) - passiveInsideWidth, Vent.CornerPoints[3].Y + 8 + sillInsideWidth));

                e.EndCut = "90/90";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);
            }
            if (Passive == 2)    //passive on right from inside view
            {
                // passive left jamb

                e = new TraceData.Part();
                e.ID = Vent.ID + "-LJ-05";
                e.Type = "Left Jamb";
                e.Profile = Vent.DoorLeafArticleName;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[1].Y - 18) - Vent.CornerPoints[0].Y);
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + outsideOffset[0], Vent.CornerPoints[0].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + outsideOffset[0], Vent.CornerPoints[1].Y - outsideOffset[0]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + outsideOffset[0] + width, Vent.CornerPoints[1].Y - outsideOffset[0] - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + outsideOffset[0] + width, Vent.CornerPoints[0].Y + outsideOffset[3]));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[4].X + 5, Vent.CornerPoints[0].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + 5, Vent.CornerPoints[1].Y - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + 5 + widthInside, Vent.CornerPoints[1].Y - widthInside - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[4].X + 5 + widthInside, Vent.CornerPoints[0].Y + 8));

                e.EndCut = "90/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);


                // passive head

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VH-06";
                e.Type = "Passive Head";
                e.Profile = Vent.DoorLeafArticleName;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].X - 15) - Vent.CornerPoints[1].X);
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + outsideOffset[1], Vent.CornerPoints[1].Y - outsideOffset[1]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - passiveWidth + width, Vent.CornerPoints[2].Y - outsideOffset[1]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - passiveWidth, Vent.CornerPoints[2].Y - outsideOffset[1] - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + outsideOffset[1] + width, Vent.CornerPoints[1].Y - outsideOffset[1] - width));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[3].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + 5, Vent.CornerPoints[1].Y - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - (widthInside - passiveInsideWidth), Vent.CornerPoints[2].Y - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - (widthInside - passiveInsideWidth), Vent.CornerPoints[2].Y - (widthInside - passiveInsideWidth) - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - (widthInside - passiveInsideWidth) - passiveInsideWidth, Vent.CornerPoints[2].Y - widthInside - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[5].X + 5 + widthInside, Vent.CornerPoints[1].Y - widthInside - 5));

                e.EndCut = "45/45";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // right passive jamb (middle)

                e = new TraceData.Part();
                e.ID = Vent.ID + "-RJ-07";
                e.Type = "Passive Right Jamb";
                e.Profile = Vent.DoorPassiveJambArticleName;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[2].Y - 18) - Vent.CornerPoints[3].Y);
                e.TextAngle = 90;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X, Vent.CornerPoints[2].Y - outsideOffset[2]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X, Vent.CornerPoints[3].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - passiveWidth, Vent.CornerPoints[3].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - passiveWidth, Vent.CornerPoints[2].Y - outsideOffset[2] - width));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - (passiveWidth - width), Vent.CornerPoints[2].Y - outsideOffset[2]));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[3].X) / 2, (e.CornerPoints[0].Y + e.CornerPoints[1].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - (widthInside - passiveInsideWidth), Vent.CornerPoints[2].Y - (widthInside - passiveInsideWidth) - 5));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[7].X - (widthInside - passiveInsideWidth), Vent.CornerPoints[3].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[7].X - (widthInside - passiveInsideWidth) - passiveInsideWidth, Vent.CornerPoints[3].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[6].X - (widthInside - passiveInsideWidth) - passiveInsideWidth, Vent.CornerPoints[2].Y - widthInside - 5));

                e.EndCut = "45/90";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);

                // passive sill

                e = new TraceData.Part();
                e.ID = Vent.ID + "-VS-08";
                e.Type = "Sill";
                e.Profile = Vent.DoorSillArticleName;
                e.Finish = Vent.Finish;
                e.Quantity = 1;
                e.Length = Math.Abs((Vent.CornerPoints[0].X + width + 15) - (Vent.CornerPoints[2].X - width));
                e.TextAngle = 0;
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - passiveWidth, Vent.CornerPoints[3].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + outsideOffset[0] + width, Vent.CornerPoints[0].Y + outsideOffset[3]));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + outsideOffset[0] + width, Vent.CornerPoints[0].Y + outsideOffset[3] + sillWidth));
                e.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - passiveWidth, Vent.CornerPoints[3].Y + outsideOffset[3] + sillWidth));

                e.TextLocation = new TraceData.Point((e.CornerPoints[0].X + e.CornerPoints[1].X) / 2, (e.CornerPoints[1].Y + e.CornerPoints[2].Y) / 2);

                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[7].X - (widthInside - passiveInsideWidth) - passiveInsideWidth, Vent.CornerPoints[3].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[4].X + width + 5, Vent.CornerPoints[0].Y + 8));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[4].X + width + 5, Vent.CornerPoints[0].Y + 8 + sillInsideWidth));
                e.CornerPointsInside.Add(new TraceData.Point(Vent.CornerPoints[7].X - (widthInside - passiveInsideWidth) - passiveInsideWidth, Vent.CornerPoints[3].Y + 8 + sillInsideWidth));

                e.EndCut = "90/90";
                e.DrawOnProposal = true;
                Vent.Extrusions.Add(e);
            }

            //Automatic Door Seal

            e = new TraceData.Part();
            e.ID = Vent.ID + "-DS-01";
            e.Type = "Automatic Door Seal";
            e.Profile = "";
            e.Finish = "N/A";
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[0].X + width + horOffset) - (Vent.CornerPoints[2].X - width)) + 116;
            e.DrawOnProposal = false;
            e.EndCut = "90/90";

            //vent width = head length
            double ventWidth = Math.Abs((Vent.CornerPoints[2].X - horOffset) - Vent.CornerPoints[1].X);

            //Deteremine article number for door seal based on vent width (min 330, max 1500)
            if (ventWidth >= 330 && ventWidth <= 355)
            {
                e.Profile = "266785";
            }
            else if (ventWidth >= 356 && ventWidth <= 435)
            {
                e.Profile = "266786";
            }
            else if (ventWidth >= 436 && ventWidth <= 500)
            {
                e.Profile = "266787";
            }
            else if(ventWidth >= 501 && ventWidth <= 700)
            {
                e.Profile = "266788";
            }
            else if (ventWidth >= 701 && ventWidth <= 900)
            {
                e.Profile = "266789";
            }
            else if (ventWidth >= 901 && ventWidth <= 1100)
            {
                e.Profile = "266790";
            }
            else if (ventWidth >= 1101 && ventWidth <= 1300)
            {
                e.Profile = "266791";
            }
            else if (ventWidth >= 1301 && ventWidth <= 1500)
            {
                e.Profile = "266792";
            }

            if (Passive != 1 && Passive != 2)
            {
                Vent.Extrusions.Add(e);
            }
            else
            {
                Vent.Extrusions.Add(e);
                Vent.Extrusions.Add(e);
            }

           

            //Retention Profile

            e = new TraceData.Part();
            e.ID = Vent.ID + "-DS-02";
            e.Type = "Retention Profile";
            e.Profile = "244693";
            e.Finish = "Black";
            e.Quantity = 1;
            e.Length = Math.Abs((Vent.CornerPoints[0].X + width + horOffset) - (Vent.CornerPoints[2].X - width));
            e.DrawOnProposal = false;
            e.EndCut = "90/90";

            if (Passive != 1 && Passive != 2)
            {
                Vent.Extrusions.Add(e);
            }
            else
            {
                Vent.Extrusions.Add(e);
                Vent.Extrusions.Add(e);
            }

            
            //Rebate Profile Jamb Left

            e = new TraceData.Part();
            e.ID = Vent.ID + "-RP-01";
            e.Type = "Rebate Profile Left Jamb";
            e.Profile = "398670";
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.DrawOnProposal = false;
            e.Length = Math.Abs((Vent.CornerPoints[2].Y - 18) - Vent.CornerPoints[3].Y) + 17.5;
            e.EndCut = "90/45";

            Vent.Extrusions.Add(e);

            //Rebate Profile Jamb Right

            e = new TraceData.Part();
            e.ID = Vent.ID + "-RP-02";
            e.Type = "Rebate Profile Right Jamb";
            e.Profile = "398670";
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.DrawOnProposal = false;
            e.Length = Math.Abs((Vent.CornerPoints[2].Y - 18) - Vent.CornerPoints[3].Y) + 17.5;
            e.EndCut = "45/90";

            Vent.Extrusions.Add(e);

            //Rebate Profile Head

            e = new TraceData.Part();
            e.ID = Vent.ID + "-RP-03";
            e.Type = "Rebate Profile Head";
            e.Profile = "398670";
            e.Finish = Vent.Finish;
            e.Quantity = 1;
            e.DrawOnProposal = false;
            e.EndCut = "45/45";

            if (Passive != 1 && Passive != 2)
            {
                e.Length = Math.Abs((Vent.CornerPoints[2].X - horOffset) - Vent.CornerPoints[1].X) + 19;
            }
            else
            {
                e.Length = Math.Abs((Vent.CornerPoints[7].X) - Vent.CornerPoints[0].X) - 1;
            }
            


            Vent.Extrusions.Add(e);
        }

        public static void CreateVentGlass(ref TraceData.Vent Vent, double Scale)
        {
            double offset = Vent.ProfileInsideWidth + 8 * Scale;
            var g = new TraceData.Part();

            g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset, Vent.CornerPoints[0].Y + offset));
            g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset, Vent.CornerPoints[1].Y - offset));
            g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset, Vent.CornerPoints[2].Y - offset));
            g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset, Vent.CornerPoints[3].Y + offset));
            g.TextLocation = new TraceData.Point((g.CornerPoints[0].X + g.CornerPoints[3].X) / 2, (g.CornerPoints[0].Y + g.CornerPoints[1].Y) / 2);
            g.Width = Math.Abs(g.CornerPoints[2].X - g.CornerPoints[1].X);
            g.Height = Math.Abs(g.CornerPoints[1].Y - g.CornerPoints[0].Y);
            g.TextAngle = 0;
            Vent.Glass = g;
        }

        public static void CreateDoorGlass(ref TraceData.Vent Vent, double Scale, int Passive)
        {
            double offset = Vent.DoorLeafInsideW + 8 * Scale;
            double sillOffset = Vent.DoorSillInsideW + 8 * Scale;
            double passiveOffset = Vent.DoorPassiveJambInsideW + 8 * Scale;
            var g = new TraceData.Part();

            g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset, Vent.CornerPoints[0].Y + sillOffset));
            g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset, Vent.CornerPoints[1].Y - offset - 5));
            g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset, Vent.CornerPoints[2].Y - offset - 5));
            g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset, Vent.CornerPoints[3].Y + sillOffset));
            g.TextLocation = new TraceData.Point((g.CornerPoints[0].X + g.CornerPoints[3].X) / 2, (g.CornerPoints[0].Y + g.CornerPoints[1].Y) / 2);
            g.Width = Math.Abs(g.CornerPoints[2].X - g.CornerPoints[1].X);
            g.Height = Math.Abs(g.CornerPoints[1].Y - g.CornerPoints[0].Y);
            g.TextAngle = 0;
            Vent.DoorGlass.Add(g);

            if(Passive == 1)    // Passive on left from inside view
            {
                g = new TraceData.Part();

                g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + passiveOffset + (offset - passiveOffset) + 5, Vent.CornerPoints[4].Y + sillOffset));
                g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + passiveOffset + (offset - passiveOffset) + 5, Vent.CornerPoints[5].Y - offset - 5));
                g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - offset, Vent.CornerPoints[6].Y - offset - 5));
                g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - offset, Vent.CornerPoints[7].Y + sillOffset));
                g.TextLocation = new TraceData.Point((g.CornerPoints[0].X + g.CornerPoints[3].X) / 2, (g.CornerPoints[0].Y + g.CornerPoints[1].Y) / 2);
                g.Width = Math.Abs(g.CornerPoints[2].X - g.CornerPoints[1].X);
                g.Height = Math.Abs(g.CornerPoints[1].Y - g.CornerPoints[0].Y);
                g.TextAngle = 0;
                Vent.DoorGlass.Add(g);
            }
            else if (Passive == 2)  // Passive on right from inside view
            {
                g = new TraceData.Part();

                g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + offset, Vent.CornerPoints[4].Y + sillOffset));
                g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + offset, Vent.CornerPoints[5].Y - offset - 5));
                g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - passiveOffset - (offset - passiveOffset) - 5, Vent.CornerPoints[6].Y - offset - 5));
                g.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - passiveOffset - (offset - passiveOffset) - 5, Vent.CornerPoints[7].Y + sillOffset));
                g.TextLocation = new TraceData.Point((g.CornerPoints[0].X + g.CornerPoints[3].X) / 2, (g.CornerPoints[0].Y + g.CornerPoints[1].Y) / 2);
                g.Width = Math.Abs(g.CornerPoints[2].X - g.CornerPoints[1].X);
                g.Height = Math.Abs(g.CornerPoints[1].Y - g.CornerPoints[0].Y);
                g.TextAngle = 0;
                Vent.DoorGlass.Add(g);
            }


        }

        public static void CreateVentGlazingBeads(ref TraceData.Vent Vent, double Scale, TraceData td)
        {
            double offset = 0;
            double width = Vent.ProfileInsideWidth;
            double beadWidth = 22;          //hard coded for now, future would be nice to grab from JSON
            var b = new TraceData.Part();
            string profile = "";

            if(td.Glazing.DoubleGlazing == true)
            {
                profile = "184090"; //bead for 25 mm glazing in vent 
            }
            else
            {
                profile = "184050"; //bead for 44 mm glazing in vent
            }
            

            // Left Jamb

            b.ID = Vent.ID + "-VB-01";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 90;
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset + width, Vent.CornerPoints[0].Y + offset + width + beadWidth));
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset + width, Vent.CornerPoints[1].Y - offset - width - beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X + beadWidth, b.CornerPoints[1].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X + beadWidth, b.CornerPoints[0].Y));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 + 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
            b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y);

            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            // Head

            b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-02";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 0;
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + offset + width, Vent.CornerPoints[1].Y - offset - width));
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset - width, Vent.CornerPoints[2].Y - offset - width));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y - beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y - beadWidth));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 - 40);
            b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X);
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            // Right Jamb

            b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-01";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 90;
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - offset - width, Vent.CornerPoints[2].Y - offset - width - beadWidth));
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset - width, Vent.CornerPoints[3].Y + offset + width + beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X - beadWidth, b.CornerPoints[1].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X - beadWidth, b.CornerPoints[0].Y));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 - 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
            b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y);
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            // Sill

            b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-02";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 0;
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - offset - width, Vent.CornerPoints[3].Y + offset + width));
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + offset + width, Vent.CornerPoints[0].Y + offset + width));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y + beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y + beadWidth));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 + 40);
            b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X);
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);
        }

        public static void CreateSlidingVentGlazingBeads(ref TraceData.Vent Vent, double Scale, TraceData td, List<SlidingDoorSystem> SlidingList)
        {
            double beadWidth = 22;              //hard coded for now, future would be nice to grab from JSON 
            double topOffset = 19;              //hard coded for now, can remove if I revise vent sizing on Proposal to consider this offset from top
            var ventCount = SlidingList[0].VentFrames.Count;
            string profile = "";
            string profile2 = "";

            if (td.Glazing.DoubleGlazing == true)
            {
                profile = "184040"; //bead for 25 mm glazing in vent 
                profile2 = "306320"; //bead for 25 mm glazing in vent on certain jambs
            }

            string chosenProfileR = profile;
            string chosenProfileL = profile;

            //Need to assign secondary glazing bead number to correct vents as needed.

            if (Vent.Track == 1) //outermost vents will always have one smaller glazing bead
            {
                if (Vent.Number == 1) //leftmost vent has its left glazing bead small
                {
                    chosenProfileL = profile2;
                }
                else if (Vent.Number == ventCount) //rightmost vent has its right glazing bead small
                {
                    chosenProfileR = profile2;
                }

            }
            else if (Vent.Track == 2 && (ventCount == 3 || ventCount == 6))
            {
                if(ventCount == 3) //3E or 3E/1
                { 
                    if (td.Vents[0].Track == 1) //if first vent is on outermost track, dependent will match its small bead
                    {
                        chosenProfileL = profile2;
                    }
                    else //else, dependent will have bead on opposite side
                    {
                        chosenProfileR = profile2;
                    }
                }
                else if (ventCount == 6) //3F
                {
                    if (Vent.Number == 2) //dependent vent on left has left glazing bead small
                    {
                        chosenProfileL = profile2;
                    }
                    else if (Vent.Number == 5) //dependent vent on right has right glazing bead small
                    {
                        chosenProfileR = profile2;
                    }
                }
            }

            string ID = "-VB-01";
            if (chosenProfileL != chosenProfileR)
            {
                ID = "-VB-03";
            }

            //Using new logic to grab extrusion corner points, which already consider offsets and such

            // Left Jamb

            var b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-01";
            b.Type = "Glazing Bead";
            b.Profile = chosenProfileL;
            b.Quantity = 1;
            b.TextAngle = 90;
            b.CornerPoints.Add(new TraceData.Point(Vent.Extrusions[0].CornerPoints[2].X, Vent.Extrusions[0].CornerPoints[2].Y));
            b.CornerPoints.Add(new TraceData.Point(Vent.Extrusions[0].CornerPoints[2].X, Vent.Extrusions[0].CornerPoints[3].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X - beadWidth, b.CornerPoints[1].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X - beadWidth, b.CornerPoints[0].Y));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 + 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
            b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y) - topOffset;
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);
            
            // Head

            b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-02";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 0;
            b.CornerPoints.Add(new TraceData.Point(Vent.Extrusions[1].CornerPoints[2].X + beadWidth, Vent.Extrusions[1].CornerPoints[2].Y));
            b.CornerPoints.Add(new TraceData.Point(Vent.Extrusions[1].CornerPoints[3].X - beadWidth, Vent.Extrusions[1].CornerPoints[3].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y + beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y + beadWidth));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 - 40);
            b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X);
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            // Right Jamb

            b = new TraceData.Part();
            b.ID = Vent.ID + ID;
            b.Type = "Glazing Bead";
            b.Profile = chosenProfileR;
            b.Quantity = 1;
            b.TextAngle = 90;
            b.CornerPoints.Add(new TraceData.Point(Vent.Extrusions[2].CornerPoints[2].X, Vent.Extrusions[2].CornerPoints[2].Y));
            b.CornerPoints.Add(new TraceData.Point(Vent.Extrusions[2].CornerPoints[3].X, Vent.Extrusions[2].CornerPoints[3].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X + beadWidth, b.CornerPoints[1].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X + beadWidth, b.CornerPoints[0].Y));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 - 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
            b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y) - topOffset;
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            // Sill

            b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-02";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 0;
            b.CornerPoints.Add(new TraceData.Point(Vent.Extrusions[3].CornerPoints[2].X - beadWidth, Vent.Extrusions[3].CornerPoints[2].Y));
            b.CornerPoints.Add(new TraceData.Point(Vent.Extrusions[3].CornerPoints[3].X + beadWidth, Vent.Extrusions[3].CornerPoints[3].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y - beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y - beadWidth));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 + 40);
            b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X);
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);
        }

        public static void CreateDoorGlazingBeads(ref TraceData.Vent Vent, double Scale, int Passive, TraceData td)
        {
            
            double width = Vent.DoorLeafInsideW;
            double sillWidth = Vent.DoorSillInsideW + 8;
            double beadWidth = 22;              //hard coded for now, future would be nice to grab from JSON
            var b = new TraceData.Part();
            string profile = "";

            if (td.Glazing.DoubleGlazing == true)
            {
                profile = "184070"; //bead for 25 mm glazing in vent 
            }
            else
            {
                profile = "184040"; //bead for 44 mm glazing in vent
            }

            double horOffset = 20;

            if (Passive == 1 || Passive == 2)
            {
                horOffset = 15;
            }


            // Left Jamb

            b.ID = Vent.ID + "-VB-01";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 90;
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + width, Vent.CornerPoints[0].Y + sillWidth + beadWidth));
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + width, Vent.CornerPoints[1].Y - width - beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X + beadWidth, b.CornerPoints[1].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X + beadWidth, b.CornerPoints[0].Y));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 + 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
            b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y) - 10;
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            // Head
            b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-02";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 0;
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[1].X + width, Vent.CornerPoints[1].Y - width));
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - width, Vent.CornerPoints[2].Y - width));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y - beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y - beadWidth));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 - 40);
            b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X) - horOffset;
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            // Right Jamb

            b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-01";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 90;
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[2].X - width, Vent.CornerPoints[2].Y - width - beadWidth));
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - width, Vent.CornerPoints[3].Y + sillWidth + beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X - beadWidth, b.CornerPoints[1].Y));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X - beadWidth, b.CornerPoints[0].Y));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 - 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
            b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y) - 10;
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            // Sill

            b = new TraceData.Part();
            b.ID = Vent.ID + "-VB-02";
            b.Type = "Glazing Bead";
            b.Profile = profile;
            b.Quantity = 1;
            b.TextAngle = 0;
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[3].X - width, Vent.CornerPoints[3].Y + sillWidth));
            b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[0].X + width, Vent.CornerPoints[0].Y + sillWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y + beadWidth));
            b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y + beadWidth));

            b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 + 40);
            b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X) - horOffset;
            b.DrawOnProposal = true;
            Vent.GlazingBeads.Add(b);

            if (Passive == 1)    // Passive on left from inside view
            {
                // Left Jamb

                b = new TraceData.Part();
                b.ID = Vent.ID + "-VB-01";
                b.Type = "Glazing Bead";
                b.Profile = profile;
                b.Quantity = 1;
                b.TextAngle = 90;
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + width - 5, Vent.CornerPoints[4].Y + sillWidth + beadWidth));
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + width - 5, Vent.CornerPoints[5].Y - width - beadWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X + beadWidth, b.CornerPoints[1].Y));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X + beadWidth, b.CornerPoints[0].Y));

                b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 + 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
                b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y) - 10;
                b.DrawOnProposal = true;
                Vent.GlazingBeads.Add(b);

                // Head

                b = new TraceData.Part();
                b.ID = Vent.ID + "-VB-02";
                b.Type = "Glazing Bead";
                b.Profile = profile;
                b.Quantity = 1;
                b.TextAngle = 0;
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + width - 5, Vent.CornerPoints[5].Y - width));
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - width, Vent.CornerPoints[6].Y - width));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y - beadWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y - beadWidth));

                b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 - 40);
                b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X) - 15;
                b.DrawOnProposal = true;
                Vent.GlazingBeads.Add(b);

                // Right Jamb

                b = new TraceData.Part();
                b.ID = Vent.ID + "-VB-01";
                b.Type = "Glazing Bead";
                b.Profile = profile;
                b.Quantity = 1;
                b.TextAngle = 90;
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - width, Vent.CornerPoints[6].Y - width - beadWidth));
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - width, Vent.CornerPoints[7].Y + sillWidth + beadWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X - beadWidth, b.CornerPoints[1].Y));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X - beadWidth, b.CornerPoints[0].Y));

                b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 - 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
                b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y) - 10;
                b.DrawOnProposal = true;
                Vent.GlazingBeads.Add(b);

                // Sill

                b = new TraceData.Part();
                b.ID = Vent.ID + "-VB-02";
                b.Type = "Glazing Bead";
                b.Profile = profile;
                b.Quantity = 1;
                b.TextAngle = 0;
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - width, Vent.CornerPoints[7].Y + sillWidth));
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + width - 5, Vent.CornerPoints[4].Y + sillWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y + beadWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y + beadWidth));

                b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 + 40);
                b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X) - 15;
                b.DrawOnProposal = true;
                Vent.GlazingBeads.Add(b);
            }
            else if (Passive == 2)    // Passive on right from inside view
            {
                // Left Jamb

                b = new TraceData.Part();
                b.ID = Vent.ID + "-VB-01";
                b.Type = "Glazing Bead";
                b.Profile = profile;
                b.Quantity = 1;
                b.TextAngle = 90;
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + width, Vent.CornerPoints[4].Y + sillWidth + beadWidth));
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + width, Vent.CornerPoints[5].Y - width - beadWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X + beadWidth, b.CornerPoints[1].Y));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X + beadWidth, b.CornerPoints[0].Y));

                b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 + 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
                b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y) - 10;
                b.DrawOnProposal = true;
                Vent.GlazingBeads.Add(b);

                // Head

                b = new TraceData.Part();
                b.ID = Vent.ID + "-VB-02";
                b.Type = "Glazing Bead";
                b.Profile = profile;
                b.Quantity = 1;
                b.TextAngle = 0;
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[5].X + width, Vent.CornerPoints[5].Y - width));
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - width + 5, Vent.CornerPoints[6].Y - width));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y - beadWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y - beadWidth));

                b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 - 40);
                b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X) - 15;
                b.DrawOnProposal = true;
                Vent.GlazingBeads.Add(b);

                // Right Jamb

                b = new TraceData.Part();
                b.ID = Vent.ID + "-VB-01";
                b.Type = "Glazing Bead";
                b.Profile = profile;
                b.Quantity = 1;
                b.TextAngle = 90;
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[6].X - width + 5, Vent.CornerPoints[6].Y - width - beadWidth));
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - width + 5, Vent.CornerPoints[7].Y + sillWidth + beadWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X - beadWidth, b.CornerPoints[1].Y));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X - beadWidth, b.CornerPoints[0].Y));

                b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[3].X) / 2 - 40, (b.CornerPoints[0].Y + b.CornerPoints[1].Y) / 2);
                b.Length = Math.Abs(b.CornerPoints[1].Y - b.CornerPoints[0].Y) - 10;
                b.DrawOnProposal = true;
                Vent.GlazingBeads.Add(b);

                // Sill

                b = new TraceData.Part();
                b.ID = Vent.ID + "-VB-02";
                b.Type = "Glazing Bead";
                b.Profile = profile;
                b.Quantity = 1;
                b.TextAngle = 0;
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[7].X - width + 5, Vent.CornerPoints[7].Y + sillWidth));
                b.CornerPoints.Add(new TraceData.Point(Vent.CornerPoints[4].X + width, Vent.CornerPoints[4].Y + sillWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[1].X, b.CornerPoints[1].Y + beadWidth));
                b.CornerPoints.Add(new TraceData.Point(b.CornerPoints[0].X, b.CornerPoints[0].Y + beadWidth));

                b.TextLocation = new TraceData.Point((b.CornerPoints[0].X + b.CornerPoints[1].X) / 2, (b.CornerPoints[0].Y + b.CornerPoints[3].Y) / 2 + 40);
                b.Length = Math.Abs(b.CornerPoints[1].X - b.CornerPoints[0].X) - 15;
                b.DrawOnProposal = true;
                Vent.GlazingBeads.Add(b);
            }

        }

        public static double VentOffset(BpsUnifiedModel Model, int MemberType, int SectionIndex)
        {
            double offset;
            offset = Model.ModelInput.Geometry.Sections.Single(x => x.SectionID == SectionIndex).InsideW;
            if (MemberType == 1)
            {
                offset = (offset - 5);
            }
            else if (MemberType == 31)
            {
                offset = 0;
            }
            else
            {
                offset = offset / 2 - 5;
            }
            return offset;
        }
        #endregion
    }
}

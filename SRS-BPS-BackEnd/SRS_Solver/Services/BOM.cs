using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;

namespace SRS_Solver.Services
{
    class BOM
    {

        /// <summary>
        /// Generate BOM Workbook and its Worksheets
        /// </summary>
        public class BOMExcelWorkbook
        {
            public static IWorkbook BOMWorkbook = new XSSFWorkbook();
            public static ISheet we = BOMWorkbook.CreateSheet("Extrusions");
            //public static ISheet ws = BOMWorkbook.CreateSheet("Loose Parts");
            //public static ISheet wf = BOMWorkbook.CreateSheet("Fittings");
        }

        /// <summary>
        /// Sets the properties that will be written into each row of the BOM
        /// </summary>
        public class BOMProperties
        {
            public string PartNumber { get; set; }
            public string ArticleNumber { get; set; }
            public string Extrusion { get; set; }
            public string Length { get; set; }
            public string SurfaceFinish { get; set; }
            public string Quantity { get; set; }
            public string Units { get; set; }
            public string PartName { get; set; }
            public string End { get; set; } //First number is left for horizontal, bottom for vertical. 90/90, 90/45, 45/90, 45/45, or Linear. For BOM
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

        /// <summary>
        /// Add row to BOM worksheet
        /// </summary>
        /// <param name="Worksheet"></param>
        /// <param name="CurrentRow"></param>
        /// <param name="values"></param>
        public static void AddRow(string Worksheet, int CurrentRow, BOMProperties Properties)
        {

            IRow row;

            if (Worksheet == "we")
            {
                row = BOMExcelWorkbook.we.CreateRow(CurrentRow);

                row.CreateCell(0).SetCellValue(Properties.PartNumber);
                row.CreateCell(1).SetCellValue(Properties.ArticleNumber);
                row.CreateCell(2).SetCellValue(Properties.Extrusion);
                row.CreateCell(3).SetCellValue(Properties.Length);
                row.CreateCell(4).SetCellValue(Properties.SurfaceFinish);
                row.CreateCell(5).SetCellValue(Properties.End);
                row.CreateCell(6).SetCellValue(Properties.Quantity);

            }
            //else if (Worksheet == "ws")
            //{
            //    row = BOMExcelWorkbook.ws.CreateRow(CurrentRow);

            //    row.CreateCell(0).SetCellValue(Properties.PartName);
            //    row.CreateCell(1).SetCellValue(Properties.ArticleNumber);
            //    row.CreateCell(2).SetCellValue(Properties.Extrusion);
            //    row.CreateCell(3).SetCellValue(Properties.Quantity);
            //    row.CreateCell(4).SetCellValue(Properties.Units);

            //}
            //else if (Worksheet == "wf")
            //{
            //    row = BOMExcelWorkbook.wf.CreateRow(CurrentRow);

            //    row.CreateCell(0).SetCellValue(Properties.PartName);
            //    row.CreateCell(1).SetCellValue(Properties.ArticleNumber);
            //    row.CreateCell(2).SetCellValue(Properties.Extrusion);
            //    row.CreateCell(3).SetCellValue(Properties.Quantity);
            //    row.CreateCell(4).SetCellValue(Properties.Units);

            //}

        }

        /// <summary>
        /// Write values into BOM workbook
        /// </summary>
        public static void WriteBOMWorkbook(TraceData td)
        {
            //Take extrusions, loose parts, and fittings and write them into the workbook

            WriteBOMExtrusions(td);
            //WriteBOMLooseParts(td);

        }

        /// <summary>
        /// OLD method for gathering data without TD
        /// </summary>
        /// <param name="model"></param>
        public static void BOMModelData(BpsUnifiedModelLib.BpsUnifiedModel model)
            {
                string ProductType = "";

                //Check product in use and assign to ProductType string

                if (model.ProblemSetting.ProductType == "Window" && model.ModelInput.Geometry.DoorSystems == null)
                {
                    ProductType = "Window";
                }
                else if (model.ProblemSetting.ProductType == "Window" && model.ModelInput.Geometry.DoorSystems != null)
                {
                    ProductType = "Door";
                }
                else if (model.ProblemSetting.ProductType == "Sliding Door")
                {
                    ProductType = "Sliding Door";
                }

                //Extrusions

                List<List<string>> ExtrusionValues = new List<List<string>>();
                List<string> MemberValues = new List<string>();

                //Set headers for Extrusion table
                MemberValues.Add("Part Name");
                MemberValues.Add("Article Number");
                MemberValues.Add("Extrusion");
                MemberValues.Add("Length (mm)");
                MemberValues.Add("Surface Finish");
                MemberValues.Add("Quantity");

                ExtrusionValues.Add(MemberValues);

                #region Outer Framing


                //Add header from Framing section

                MemberValues = new List<string>();
                MemberValues.Add("Outer Framing");
                ExtrusionValues.Add(MemberValues);

                //Get surface finish
                string SurfaceFinish = model.ModelInput.FrameSystem.AluminumColor;

                var modelGeo = model.ModelInput.Geometry;
                var width = modelGeo.Points.Max(x => x.X);
                var height = modelGeo.Points.Max(x => x.Y);
                var outerFrameInsideW = modelGeo.Sections.Single(x => x.SectionID == 1).InsideW;
                double mullionInsideW = 0;
                double transomInsideW = 0;
                double mullionOutsideW = 0;
                double transomOutsideW = 0;
                string mullionArticle = "";
                string transomArticle = "";

                //Checks for intermediates and if they exist grab mullion/transom information
                if (modelGeo.Members.Count > 4)
                {
                    for (var i = 5; i <= modelGeo.Members.Count; i++)
                    {
                        var PointAID = modelGeo.Members.Single(x => x.MemberID == i).PointA;
                        var PointBID = modelGeo.Members.Single(x => x.MemberID == i).PointB;
                        var MemberType = modelGeo.Members.Single(x => x.MemberID == i).MemberType;

                        //don't grab from door profiles
                        if (MemberType == 31 || MemberType == 33)
                        {
                            continue;
                        }

                        //determine whether it's a mullion or transom and grab info
                        if (modelGeo.Points.Single(x => x.PointID == PointAID).X == modelGeo.Points.Single(x => x.PointID == PointBID).X)
                        {
                            mullionArticle = modelGeo.Sections.Single(x => x.SectionType == MemberType).ArticleName;
                            mullionInsideW = modelGeo.Sections.Single(x => x.SectionType == MemberType).InsideW;
                            mullionOutsideW = modelGeo.Sections.Single(x => x.SectionType == MemberType).OutsideW;
                        }
                        else
                        {
                            transomArticle = modelGeo.Sections.Single(x => x.SectionType == MemberType).ArticleName;
                            transomInsideW = modelGeo.Sections.Single(x => x.SectionType == MemberType).InsideW;
                            transomOutsideW = modelGeo.Sections.Single(x => x.SectionType == MemberType).OutsideW;
                        }
                    }
                }
                else
                {
                    mullionInsideW = 0;
                    transomInsideW = 0;
                }


                //Set part name, article number, length, quantity for outer framing members
                foreach (var m in model.ModelInput.Geometry.Members)
                {
                    MemberValues = new List<string>();

                    //Check for threshold and if found skip it
                    if (m.SectionID == 31)
                    {
                        continue;
                    }

                    //Get x & y for Points A & B that define the member
                    var PointAx = model.ModelInput.Geometry.Points.Single(x => x.PointID == m.PointA).X;
                    var PointAy = model.ModelInput.Geometry.Points.Single(x => x.PointID == m.PointA).Y;
                    var PointBx = model.ModelInput.Geometry.Points.Single(x => x.PointID == m.PointB).X;
                    var PointBy = model.ModelInput.Geometry.Points.Single(x => x.PointID == m.PointB).Y;

                    double nominalLength = 0;
                    string direction;
                    string memberExtrusion = "";
                    string partNumber = "";
                    int quantity = 1;


                    //get article number
                    string memberArticle = model.ModelInput.Geometry.Sections.Single(x => x.SectionID == m.SectionID).ArticleName;

                    //Set nominal length
                    if (PointAx == PointBx)  //vertical profile
                    {
                        direction = "Vertical";
                        nominalLength = Math.Abs(PointAy - PointBy);
                    }
                    else //horizontal profile
                    {
                        direction = "Horizontal";
                        nominalLength = Math.Abs(PointAx - PointBx);
                    }


                    //Get Extrusion type and assign part number (AWS & ADS)

                    if (m.MemberID == 1)
                    {
                        memberExtrusion = "JambLeft";
                        partNumber = model.SRSProblemSetting.OrderNumber + "-JL-01";        //altering logic to have two jambs as different parts instead of two of same quantity, might need to rever later
                    }
                    else if (m.MemberID == 2)
                    {
                        memberExtrusion = "JambRight";
                        partNumber = model.SRSProblemSetting.OrderNumber + "-JR-02";
                    }
                    else if (m.MemberID == 3)
                    {
                        memberExtrusion = "Sill";
                        partNumber = model.SRSProblemSetting.OrderNumber + "-S-03";
                    }
                    else if (m.MemberID == 4)
                    {
                        memberExtrusion = "Head";
                        partNumber = model.SRSProblemSetting.OrderNumber + "-H-04";
                    }
                    else if (direction == "Horizontal" && m.MemberType == 33)
                    {
                        memberExtrusion = "Sidelight Sill";
                        partNumber = model.SRSProblemSetting.OrderNumber + "-SL-0" + m.MemberID.ToString();
                    }
                    else if (direction == "Horizontal" && m.MemberID > 4)
                    {
                        memberExtrusion = "Transom";
                        partNumber = model.SRSProblemSetting.OrderNumber + "-T-0" + m.MemberID.ToString();
                    }
                    else if (direction == "Vertical" && m.MemberID > 4)
                    {
                        memberExtrusion = "Mullion";
                        partNumber = model.SRSProblemSetting.OrderNumber + "-M-0" + m.MemberID.ToString();
                    }

                    //Get actual lengths of intermediates (AWS & ADS)

                    switch (memberExtrusion)
                    {
                        case "Sidelight Sill":

                            //Determine Point A connection
                            if (PointAx == 0 || PointAx == width)
                            {
                                nominalLength = nominalLength - outerFrameInsideW;   //connect to outer frame
                            }
                            else
                            {
                                nominalLength = nominalLength - (mullionInsideW / 2);   //connect to mullion
                            }

                            //Determine Point B connection
                            if (PointBx == 0 || PointBx == width)
                            {
                                nominalLength = nominalLength - outerFrameInsideW;   //connect to outer frame
                            }
                            else
                            {
                                nominalLength = nominalLength - (mullionInsideW / 2);   //connect to mullion
                            }

                            break;

                        case "Mullion":

                            //Determine Point A connection
                            if ((PointAy == 0 && ProductType == "Window") || PointAy == height) //connect to outer frame
                            {
                                if (memberArticle == "382280")   //regular
                                {
                                    nominalLength = nominalLength - outerFrameInsideW;
                                }
                                //structural has no length change

                            }
                            else if (PointAy == 0 && ProductType == "Door")
                            {
                                //connect to base of outer frame in door has no length change
                            }
                            else    //connect to intermediate
                            {
                                //regular
                                if (memberArticle == "382280")   //regular reduces by half the transom inside width
                                {
                                    nominalLength = nominalLength - (transomInsideW / 2);
                                }
                                //small structural
                                else if (memberArticle == "368650")
                                {
                                    //other intermediate is same size or bigger
                                    if (mullionArticle == transomArticle || transomArticle == "368660")
                                    {
                                        nominalLength = nominalLength - (transomInsideW / 2);   //half the box width, which typically is same as inside width
                                    }
                                    //other intermediate is regular
                                    else if (transomArticle == "382280")
                                    {
                                        //3-way vs 4-way
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointAx)) && (1 < modelGeo.Points.Count(x => x.Y == PointAy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (transomOutsideW / 2);  //3-way
                                        }
                                    }

                                }
                                //big structural
                                else if (memberArticle == "368660")
                                {
                                    //other intermediate is same size
                                    if (mullionArticle == transomArticle)
                                    {
                                        nominalLength = nominalLength - (transomInsideW / 2);   //half the box width, which typically is same as inside width
                                    }
                                    //other is regular
                                    else if (transomArticle == "382280")
                                    {
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointAx)) && (1 < modelGeo.Points.Count(x => x.Y == PointAy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (transomOutsideW / 2);  //3-way
                                        }
                                    }
                                    //other is smaller
                                    else if (transomArticle == "368650")
                                    {
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointAx)) && (1 < modelGeo.Points.Count(x => x.Y == PointAy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (transomInsideW / 2);  //3-way
                                        }
                                    }
                                }
                            }

                            //Determine Point B connection
                            if ((PointBy == 0 && ProductType == "Window") || PointBy == height) //connect to outer frame
                            {
                                if (memberArticle == "382280")   //regular
                                {
                                    nominalLength = nominalLength - outerFrameInsideW;
                                }
                                //structural has no length change

                            }
                            else if (PointBy == 0 && ProductType == "Door")
                            {
                                //connect to base of outer frame in door has no length change
                            }
                            else    //connect to intermediate
                            {
                                //regular
                                if (memberArticle == "382280")   //regular reduces by half the transom inside width
                                {
                                    nominalLength = nominalLength - (transomInsideW / 2);
                                }
                                //small structural
                                else if (memberArticle == "368650")
                                {
                                    //other intermediate is same size or bigger
                                    if (mullionArticle == transomArticle || transomArticle == "368660")
                                    {
                                        nominalLength = nominalLength - (transomInsideW / 2);   //half the box width, which typically is same as inside width
                                    }
                                    //other intermediate is regular
                                    else if (transomArticle == "382280")
                                    {
                                        //3-way vs 4-way
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointBx)) && (1 < modelGeo.Points.Count(x => x.Y == PointBy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (transomOutsideW / 2);  //3-way
                                        }
                                    }

                                }
                                //big structural
                                else if (memberArticle == "368660")
                                {
                                    //other intermediate is same size
                                    if (mullionArticle == transomArticle)
                                    {
                                        nominalLength = nominalLength - (transomInsideW / 2);   //half the box width, which typically is same as inside width
                                    }
                                    //other is regular
                                    else if (transomArticle == "382280")
                                    {
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointBx)) && (1 < modelGeo.Points.Count(x => x.Y == PointBy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (transomOutsideW / 2);  //3-way
                                        }
                                    }
                                    //other is smaller
                                    else if (transomArticle == "368650")
                                    {
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointBx)) && (1 < modelGeo.Points.Count(x => x.Y == PointBy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (transomInsideW / 2);  //3-way
                                        }
                                    }
                                }
                            }

                            break;

                        case "Transom":

                            //Determine Point A connection
                            if (PointAx == 0 || PointAx == width) //connect to outer frame
                            {
                                if (memberArticle == "382280")   //regular
                                {
                                    nominalLength = nominalLength - outerFrameInsideW;
                                }
                                //structural has no length change

                            }
                            else    //connect to intermediate
                            {
                                //regular
                                if (memberArticle == "382280")   //regular reduces by half the mullion inside width
                                {
                                    nominalLength = nominalLength - (mullionInsideW / 2);
                                }
                                //small structural
                                else if (memberArticle == "368650")
                                {
                                    //other intermediate is same size or bigger
                                    if (mullionArticle == transomArticle || mullionArticle == "368660")
                                    {
                                        nominalLength = nominalLength - (mullionInsideW / 2);   //half the box width, which typically is same as inside width
                                    }
                                    //other intermediate is regular
                                    else if (mullionArticle == "382280")
                                    {
                                        //3-way vs 4-way
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointAx)) && (1 < modelGeo.Points.Count(x => x.Y == PointAy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (mullionOutsideW / 2);  //3-way
                                        }
                                    }

                                }
                                //big structural
                                else if (memberArticle == "368660")
                                {
                                    //other intermediate is same size
                                    if (mullionArticle == transomArticle)
                                    {
                                        nominalLength = nominalLength - (mullionInsideW / 2);   //half the box width, which typically is same as inside width
                                    }
                                    //other is regular
                                    else if (mullionArticle == "382280")
                                    {
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointAx)) && (1 < modelGeo.Points.Count(x => x.Y == PointAy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (mullionOutsideW / 2);  //3-way
                                        }
                                    }
                                    //other is smaller
                                    else if (mullionArticle == "368650")
                                    {
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointAx)) && (1 < modelGeo.Points.Count(x => x.Y == PointAy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (mullionInsideW / 2);  //3-way
                                        }
                                    }
                                }
                            }

                            //Determine Point B connection
                            if (PointBx == 0 || PointBx == width) //connect to outer frame
                            {
                                if (memberArticle == "382280")   //regular
                                {
                                    nominalLength = nominalLength - outerFrameInsideW;
                                }
                                //structural has no length change

                            }
                            else    //connect to intermediate
                            {
                                //regular
                                if (memberArticle == "382280")   //regular reduces by half the mullion inside width
                                {
                                    nominalLength = nominalLength - (mullionInsideW / 2);
                                }
                                //small structural
                                else if (memberArticle == "368650")
                                {
                                    //other intermediate is same size or bigger
                                    if (mullionArticle == transomArticle || mullionArticle == "368660")
                                    {
                                        nominalLength = nominalLength - (mullionInsideW / 2);   //half the box width, which typically is same as inside width
                                    }
                                    //other intermediate is regular
                                    else if (mullionArticle == "382280")
                                    {
                                        //3-way vs 4-way
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointBx)) && (1 < modelGeo.Points.Count(x => x.Y == PointBy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (mullionOutsideW / 2);  //3-way
                                        }
                                    }

                                }
                                //big structural
                                else if (memberArticle == "368660")
                                {
                                    //other intermediate is same size
                                    if (mullionArticle == transomArticle)
                                    {
                                        nominalLength = nominalLength - (mullionInsideW / 2);   //half the box width, which typically is same as inside width
                                    }
                                    //other is regular
                                    else if (mullionArticle == "382280")
                                    {
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointBx)) && (1 < modelGeo.Points.Count(x => x.Y == PointBy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (mullionOutsideW / 2);  //3-way
                                        }
                                    }
                                    //other is smaller
                                    else if (mullionArticle == "368650")
                                    {
                                        if ((1 < modelGeo.Points.Count(x => x.X == PointBx)) && (1 < modelGeo.Points.Count(x => x.Y == PointBy)))
                                        {
                                            //4-way intersection has no length change
                                        }
                                        else
                                        {
                                            nominalLength = nominalLength + (mullionInsideW / 2);  //3-way
                                        }
                                    }
                                }
                            }

                            break;

                        default:

                            break;
                    }

                    //Add values to member's list
                    MemberValues.Add(partNumber);
                    MemberValues.Add(memberArticle);
                    MemberValues.Add(memberExtrusion);
                    MemberValues.Add(nominalLength.ToString());
                    MemberValues.Add(SurfaceFinish);
                    MemberValues.Add(quantity.ToString());

                    //Add member list to extrusion values list
                    ExtrusionValues.Add(MemberValues);

                }



                #endregion

                #region Fixed Glazing Bars

                foreach (var g in modelGeo.Infills)
                {
                    int Number = 00;
                    if (g.OperabilitySystemID == -1) //fixed glass
                    {
                        var index = 0;
                        double[] controlPoints = new double[4];

                        //define glass x,y based on bounding members
                        foreach (var b in g.BoundingMembers)
                        {
                            var PointX = modelGeo.Points.Single(x => x.PointID == modelGeo.Members.Single(y => y.MemberID == b).PointA).X;
                            var PointY = modelGeo.Points.Single(x => x.PointID == modelGeo.Members.Single(y => y.MemberID == b).PointA).Y;

                            if (index % 2 == 0) //check if odd or even
                            {
                                controlPoints[index] = PointX;
                            }
                            else
                            {
                                controlPoints[index] = PointY;
                            }
                            index++;
                        }

                        //just keeping track of x,y coordinates for the glass (not considering offset here). Maybe do this independent of fixed/vent and then adjust offset accordingly
                        double[] topLeft = new double[] { controlPoints[0], controlPoints[1] };
                        double[] topRight = new double[] { controlPoints[2], controlPoints[1] };
                        double[] bottomLeft = new double[] { controlPoints[0], controlPoints[3] };
                        double[] bottomRight = new double[] { controlPoints[2], controlPoints[3] };

                        //Horizontal Glazing Beads

                        MemberValues = new List<string>();


                        //member article changes with glass thickness
                        string memberArticle = "000000";
                        int quantity = 2;
                        string memberExtrusion = "Glazing Bead";
                        string partNumber = model.SRSProblemSetting.OrderNumber + "-GB-" + Number.ToString("D2");
                        //length will change with offset, based on framing
                        double nominalLength = Math.Abs(controlPoints[0] - controlPoints[2]);

                        MemberValues.Add(partNumber);
                        MemberValues.Add(memberArticle);
                        MemberValues.Add(memberExtrusion);
                        MemberValues.Add(nominalLength.ToString());
                        MemberValues.Add(SurfaceFinish);
                        MemberValues.Add(quantity.ToString());

                        //Add member list to extrusion values list
                        ExtrusionValues.Add(MemberValues);

                        Number++;

                        //Vertical Glazing Beads

                        MemberValues = new List<string>();

                        memberArticle = "000000";
                        quantity = 2;
                        memberExtrusion = "Glazing Bead";
                        partNumber = model.SRSProblemSetting.OrderNumber + "-GB-" + Number.ToString("D2");
                        nominalLength = Math.Abs(controlPoints[1] - controlPoints[3]);

                        MemberValues.Add(partNumber);
                        MemberValues.Add(memberArticle);
                        MemberValues.Add(memberExtrusion);
                        MemberValues.Add(nominalLength.ToString());
                        MemberValues.Add(SurfaceFinish);
                        MemberValues.Add(quantity.ToString());

                        //Add member list to extrusion values list
                        ExtrusionValues.Add(MemberValues);

                        Number++;

                    }
                    else
                    {
                        continue;
                    }
                }

                #endregion


                //Write extrusion values into workbook
                int CurrentRow = 0;

                //foreach (var m in ExtrusionValues)
                //{
                //    AddRow("we", CurrentRow, m);
                //    CurrentRow++;
                //}

            }

        /// <summary>
        /// Read TD, write Extrusion info to BOM
        /// </summary>
        /// <param name="td"></param>
        public static void WriteBOMExtrusions(TraceData td)
        {
            //Check product, create extrusion list

            var MemberValues = new BOMProperties();
            List<BOMProperties> ExtrusionValues = new List<BOMProperties>();

            //Set headers for Extrusion table

            MemberValues.PartNumber = "Part Number";
            MemberValues.ArticleNumber = "Article Number";
            MemberValues.Extrusion = "Extrusion";
            MemberValues.Length = "Length";
            MemberValues.SurfaceFinish = "Surface Finish";
            MemberValues.End = "Cut";
            MemberValues.Quantity = "Quantity";

            ExtrusionValues.Add(MemberValues);

            MemberValues = new BOMProperties();
            MemberValues.PartNumber = "Outer Frame";
            ExtrusionValues.Add(MemberValues);


            //Set values for each extrusion in model

            foreach (var e in td.Framing.Extrusions)
            {
                if (e.Type == "Door Threshold") continue;

                MemberValues = new BOMProperties();

                MemberValues.PartNumber = e.ID;
                MemberValues.ArticleNumber = e.Profile;
                MemberValues.Extrusion = e.Type;
                MemberValues.Length = e.Length.ToString("F2");
                MemberValues.SurfaceFinish = e.Finish;
                MemberValues.End = e.EndCut;
                MemberValues.Quantity = "1";

                ExtrusionValues.Add(MemberValues);
            }

            //Add fixed glazing beads if any present in model

            if (td.Glazing.Beads.Count > 0)
            {
                foreach (var b in td.Glazing.Beads)
                {
                    MemberValues = new BOMProperties();

                    MemberValues.PartNumber = b.ID;
                    MemberValues.ArticleNumber = b.Profile;
                    MemberValues.Extrusion = b.Type;
                    MemberValues.Length = b.Length.ToString("F2");
                    MemberValues.SurfaceFinish = td.Project.Color;
                    MemberValues.End = "90/90";     //glazing beads are flat cut in most cases (can revise later if we have mitered ones)
                    MemberValues.Quantity = "1";

                    ExtrusionValues.Add(MemberValues);

                }
            }

            //If there are vents, add extrusions

            if (td.Vents.Count > 0)
            {
                var ventcount = 1;

                foreach (var v in td.Vents)
                {

                    MemberValues = new BOMProperties();
                    MemberValues.PartNumber = "Vent " + ventcount.ToString();
                    ExtrusionValues.Add(MemberValues);

                    ventcount++;

                    //Add extrusions per vent

                    foreach (var e in v.Extrusions)
                    {
                        MemberValues = new BOMProperties();

                        MemberValues.PartNumber = e.ID;
                        MemberValues.ArticleNumber = e.Profile;
                        MemberValues.Extrusion = e.Type;
                        MemberValues.Length = e.Length.ToString("F2");
                        MemberValues.SurfaceFinish = e.Finish;
                        MemberValues.End = e.EndCut;
                        MemberValues.Quantity = "1";

                        ExtrusionValues.Add(MemberValues);
                    }

                    //Add glazing beads per vent

                    foreach (var b in v.GlazingBeads)
                    {
                        MemberValues = new BOMProperties();

                        MemberValues.PartNumber = b.ID;
                        MemberValues.ArticleNumber = b.Profile;
                        MemberValues.Extrusion = b.Type;
                        MemberValues.Length = b.Length.ToString("F2");
                        MemberValues.SurfaceFinish = td.Project.Color;
                        MemberValues.End = "90/90"; //glazing beads are flat cut. We can revise later if we add mitered options
                        MemberValues.Quantity = "1";

                        ExtrusionValues.Add(MemberValues);
                    }

                }
            }

            //Adjust quantity based on existence of duplicates

            foreach (var m in ExtrusionValues)
            {
                if (m.PartNumber == "" || m.PartNumber == "Part Number" || m.PartNumber == "Outer Frame" || m.PartNumber.Contains("Vent"))
                {
                    continue;
                }

                var quantity = 0;

                foreach (var c in ExtrusionValues)
                {
                    if (c.PartNumber == "" || c.PartNumber == "Part Number" || c.PartNumber == "Outer Frame" || c.PartNumber.Contains("Vent"))
                    {
                        continue;
                    }

                    if (m.PartNumber == c.PartNumber && m.ArticleNumber == c.ArticleNumber && m.Length == c.Length && m.End == c.End)
                    {
                        quantity++;
                        m.Quantity = quantity.ToString();
                    }

                }

            }

            //Grab first instance of a duplicate (cull based on quantity)

            var DistinctItems = ExtrusionValues.GroupBy(x => x.PartNumber).Select(y => y.First());

            //Add blank lines into list

            MemberValues = new BOMProperties();
            var clean = DistinctItems.ToList();

            foreach (var n in DistinctItems)
            {

                if (n.PartNumber.Contains("Vent") || n.PartNumber.Contains("Outer Frame"))
                {
                    var index = clean.IndexOf(n);
                    clean.Insert(index, MemberValues);
                }

            }

            //Add Rows with Extrusion info

            int CurrentRow = 0;

            foreach (var m in clean)
            {
                AddRow("we", CurrentRow, m);
                CurrentRow++;
            }

        }

        /// <summary>
        /// Read TD, write Loose Part info to BOM
        /// </summary>
        /// <param name="td"></param>
        public static void WriteBOMLooseParts(TraceData td)
        {
            //Check product, create loose part list
        }

        /// <summary>
        /// Read TD, write Fitting info to BOM
        /// </summary>
        /// <param name="td"></param>
        public static void WriteBOMFittings(TraceData td)
        {
            //Check product, create fittings list
        }

        /// <summary>
        /// Save BOM workbook to desktop
        /// </summary>
        public static void SaveBOMWorkbook(string BOMOutputFile)
        {
            //Resize extrusion columns
            for (var i = 0; i < 6; i++)
            {
                BOMExcelWorkbook.we.AutoSizeColumn(i);
            }

            //save file to desktop
            FileStream stream = new FileStream(BOMOutputFile, FileMode.Create, FileAccess.Write);
            BOMExcelWorkbook.BOMWorkbook.Write(stream);
        }

        
    }
}

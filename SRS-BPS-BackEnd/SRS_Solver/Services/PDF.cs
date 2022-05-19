using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SRS_Solver.Services
{
    class PDF
    {
        #region Services

        public class BoundingBox
        {

            public BoundingBox(List<TraceData.Point> Points)
            {
                xmin = 1.0E30;
                ymin = 1.0E30;
                xmax = -1.0E30;
                ymax = -1.0E30;

                foreach (var p in Points)
                {
                    xmin = Math.Min(xmin, p.X);
                    xmax = Math.Max(xmax, p.X);
                    ymin = Math.Min(ymin, p.Y);
                    ymax = Math.Max(ymax, p.Y);
                }
            }
            public double xmin { get; set; }
            public double xmax { get; set; }
            public double ymin { get; set; }
            public double ymax { get; set; }

        }
        public class Layout
        {
            public Layout(double XC, double YC, double Width, double Height)
            {
                this.Width = Width;
                this.Height = Height;
                this.Xoffset = XC;
                this.Yoffset = YC;
                this.Scale = 1.0;
            }
            public double Width { get; set; }
            public double Height { get; set; }
            public double Xoffset { get; set; }
            public double Yoffset { get; set; }
            public double Scale { get; set; }


        }

        #endregion

        #region Draw

        public static void DrawElevation(List<TraceData.Point> Points, TraceData TD, PdfDocument myTemplate)
        {
            XGraphics g = XGraphics.FromPdfPage(myTemplate.Pages[2]);

            //RGB for Profile color (Handle and Hinge RGBs handled within DrawVents)
            int[] RGB = new int[3];

            if(TD.Project.Color.Contains("7001"))   //Silver Grey
            {
                RGB[0] = 140;
                RGB[1] = 150;
                RGB[2] = 157;
            }
            else if (TD.Project.Color.Contains("9016"))   //Traffic White
            {
                RGB[0] = 241;
                RGB[1] = 240;
                RGB[2] = 234;
            }
            else if (TD.Project.Color.Contains("9010"))   //Pure White
            {
                RGB[0] = 241;
                RGB[1] = 236;
                RGB[2] = 225;
            }
            else if (TD.Project.Color.Contains("9005"))   //Jet Black
            {
                RGB[0] = 14;
                RGB[1] = 14;
                RGB[2] = 16;
            }
            else if (TD.Project.Color.Contains("7016"))   //Anthracite Grey
            {
                RGB[0] = 56;
                RGB[1] = 62;
                RGB[2] = 66;
            }
            else if (TD.Project.Color.Contains("8022"))   //Black-Brown
            {

                RGB[0] = 26;
                RGB[1] = 23;
                RGB[2] = 24;
            }
            else if (TD.Project.Color.Contains("INOX"))   //Stainless Steel
            {
                RGB[0] = 172;
                RGB[1] = 178;
                RGB[2] = 180;
            }
            else //Color Missing
            {
                RGB[0] = 0;
                RGB[1] = 255;
                RGB[2] = 0;
            }
            


            // Draw the front Elevation

            Layout front = new Layout(0, 55, 612, 300);
            Scale(Points, ref front);

            
            DrawGlass(g, TD, front);
            DrawFrame(g, TD, front, RGB);
            DrawVents(g, TD, front, RGB);
            DrawSlidingReinforcement(g, front, TD, "Front", RGB);
           
           
            DrawDimensions(g, Points, front, "Vertical");
            DrawDimensions(g, Points, front, "Horizontal");

            // draw the back elevation

            Layout back = new Layout(0, 65 + 335, 612, 300);
            Scale(Points, ref back);

            XMatrix mirrorMatrix = new XMatrix(-1, 0, 0, 1, back.Width, 0);
            g.MultiplyTransform(mirrorMatrix);

            DrawGlass(g, TD, back);
            DrawFrame(g, TD, back, RGB);
            DrawGlazingBeads(g, TD, back, RGB);
            DrawDimensions(g, Points, back, "Horizontal", true);
            DrawVents(g, TD, back, RGB, "Back");
            DrawSlidingReinforcement(g, back, TD, "Back", RGB);


            g.Save();
            g.Dispose();

        }
        public static void DrawGlass(XGraphics g, TraceData TD, Layout L)
        {
            XPoint[] curvePoints = new XPoint[4];

            XPen bluePen = new XPen(XColor.FromArgb(168, 204, 215), .025);
            XBrush blueBrush = new XSolidBrush(XColor.FromArgb(168, 204, 215));


            foreach (var p in TD.Glazing.Panes)
            {
                for (int i = 0; i < 4; i++)
                {
                    curvePoints[i] = LayoutPoint(p.CornerPoints[i], L);
                }
                g.DrawPolygon(bluePen, blueBrush, curvePoints, XFillMode.Alternate);
            }

        }
        public static void DrawFrame(XGraphics g, TraceData TD, Layout L, int[] RGB)
        {
            XPoint[] curvePoints = new XPoint[4];

            

            XPen grayPen = new XPen(XColor.FromArgb(38, 38, 38), .125);
            XBrush grayBrush = new XSolidBrush(XColor.FromArgb(RGB[0], RGB[1], RGB[2]));

            foreach (var e in TD.Framing.Extrusions)
            {
                if(e.DrawOnProposal == false)
                {
                    continue;
                }
                for (int i = 0; i < 4; i++)
                {
                    curvePoints[i] = LayoutPoint(e.CornerPoints[i], L);
                }
                g.DrawPolygon(grayPen, grayBrush, curvePoints, XFillMode.Alternate);
            }

        }
        public static void DrawGlazingBeads(XGraphics g, TraceData TD, Layout L, int[] RGB)
        {
            XPoint[] curvePoints = new XPoint[4];

            XPen grayPen = new XPen(XColor.FromArgb(38, 38, 38), .125);
            XBrush grayBrush = new XSolidBrush(XColor.FromArgb(RGB[0], RGB[1], RGB[2]));

            foreach (var gb in TD.Glazing.Beads)
            {
                for (int i = 0; i < 4; i++)
                {
                    curvePoints[i] = LayoutPoint(gb.CornerPoints[i], L);
                }
                g.DrawPolygon(grayPen, grayBrush, curvePoints, XFillMode.Alternate);
            }

        }
        public static void DrawVents(XGraphics g, TraceData TD, Layout L, int[] RGB, string Side = "Front")
        {
            XPoint[] curvePoints = new XPoint[4];
            XPoint[] passiveCurvePoints = new XPoint[5];

            XPen grayPen = new XPen(XColor.FromArgb(38, 38, 38), .125);
            XBrush grayBrush = new XSolidBrush(XColor.FromArgb(RGB[0], RGB[1], RGB[2]));
            
            if(TD.Vents.Count > 0 && TD.Vents[0].SlidingDoorSystemID == 1 && Side == "Front")
            {
                TD.Vents.Sort((x, y) => y.Track.CompareTo(x.Track)); // desc
            }
            else if (TD.Vents.Count > 0 && TD.Vents[0].SlidingDoorSystemID == 1 && Side == "Back")
            {                
                TD.Vents.Sort((x, y) => x.Track.CompareTo(y.Track)); // asc
            }


            if (Side == "Front")
            {
                foreach (var v in TD.Vents)
                {
                    foreach (var e in v.Extrusions)
                    {
                        if (e.Type == "Passive Right Jamb" || e.Type == "Passive Left Jamb")
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                passiveCurvePoints[j] = LayoutPoint(e.CornerPoints[j], L);
                            }
                            g.DrawPolygon(grayPen, grayBrush, passiveCurvePoints, XFillMode.Alternate);
                        }
                        else if (e.DrawOnProposal == false)
                        {
                            continue;
                        }
                        else
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                curvePoints[j] = LayoutPoint(e.CornerPoints[j], L);
                            }
                            g.DrawPolygon(grayPen, grayBrush, curvePoints, XFillMode.Alternate);
                        }
                    }
                    DrawOpeningSymbol(g, v, L, Side);
                if(v.Type == "SingleDoor" || v.Type == "DoubleDoor")
                    {
                        DrawHandle(g, v, L, Side);
                    }
                else if (v.Type == "Sliding")
                    {
                        DrawHandle(g, v, L, Side);
                    }
                   

                }
                
            }
            else
            {
                foreach (var v in TD.Vents)
                {
                    foreach (var e in v.Extrusions)
                    {
                        if (e.Type == "Passive Head")
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                passiveCurvePoints[j] = LayoutPoint(e.CornerPointsInside[j], L);
                            }
                            g.DrawPolygon(grayPen, grayBrush, passiveCurvePoints, XFillMode.Alternate);
                        }
                        else if (e.DrawOnProposal == false)
                        {
                            continue;
                        }
                        else
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                curvePoints[j] = LayoutPoint(e.CornerPointsInside[j], L);
                            }
                            g.DrawPolygon(grayPen, grayBrush, curvePoints, XFillMode.Alternate);
                        }
                    }

                    foreach (var gb in v.GlazingBeads)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            curvePoints[j] = LayoutPoint(gb.CornerPoints[j], L);
                        }
                        g.DrawPolygon(grayPen, grayBrush, curvePoints, XFillMode.Alternate);
                    }
                    DrawOpeningSymbol(g, v, L, Side);
                    if (v.Type != "Sliding Fixed" && v.Type != "Passive Sliding")
                    {
                        DrawHandle(g, v, L, Side);
                    }
                   
                }
            }



        }
        public static void DrawHandle(XGraphics g, TraceData.Vent V, Layout L, string Side)
        {

            XPen pen = new XPen(XColor.FromArgb(30, 30, 30), .125);

            //Set up RGB values for hinges and handles

            string InsideHandleColor = "";
            string OutsideHandleColor = "";
            string HingeColor = "";

            if(V.Type != "SingleDoor" && V.Type != "DoubleDoor" && V.Type != "Sliding")
            {
                InsideHandleColor = V.HandleColor;
            }
            else if (V.Type == "SingleDoor" || V.Type == "DoubleDoor")
            {
                InsideHandleColor = V.InsideHandleColor;
                OutsideHandleColor = V.OutsideHandleColor;
                HingeColor = V.HingeColor;
            }
            else if (V.Type == "Sliding")
            {
                InsideHandleColor = V.InsideHandleColor;
                OutsideHandleColor = V.OutsideHandleColor;
            }

            int[] InsideRGB = new int[3];
            int[] OutsideRGB = new int[3];
            int[] HingeRGB = new int[3];

            {   //Assign RGB for Inside Handle

                if (InsideHandleColor == null)
                {
                    InsideRGB[0] = 140;
                    InsideRGB[1] = 150;
                    InsideRGB[2] = 157;
                }
                if (InsideHandleColor.Contains("7001"))   //Silver Grey
                {
                    InsideRGB[0] = 140;
                    InsideRGB[1] = 150;
                    InsideRGB[2] = 157;
                }
                else if (InsideHandleColor.Contains("9016"))   //Traffic White
                {
                    InsideRGB[0] = 241;
                    InsideRGB[1] = 240;
                    InsideRGB[2] = 234;
                }
                else if (InsideHandleColor.Contains("9010"))   //Pure White
                {
                    InsideRGB[0] = 241;
                    InsideRGB[1] = 236;
                    InsideRGB[2] = 225;
                }
                else if (InsideHandleColor.Contains("9005"))   //Jet Black
                {
                    InsideRGB[0] = 14;
                    InsideRGB[1] = 14;
                    InsideRGB[2] = 16;
                }
                else if (InsideHandleColor.Contains("7016"))   //Anthracite Grey
                {
                    InsideRGB[0] = 56;
                    InsideRGB[1] = 62;
                    InsideRGB[2] = 66;
                }
                else if (InsideHandleColor.Contains("8022"))   //Black-Brown
                {

                    InsideRGB[0] = 26;
                    InsideRGB[1] = 23;
                    InsideRGB[2] = 24;
                }
                else if (InsideHandleColor.Contains("INOX"))   //Stainless Steel
                {
                    InsideRGB[0] = 172;
                    InsideRGB[1] = 178;
                    InsideRGB[2] = 180;
                }
                else
                {
                    InsideRGB[0] = 0;
                    InsideRGB[1] = 255;
                    InsideRGB[2] = 0;
                }

            }
            {   //Assign RGB for Outside Handle

                if (OutsideHandleColor == null)
                {
                    OutsideRGB[0] = 140;
                    OutsideRGB[1] = 150;
                    OutsideRGB[2] = 157;
                }
                else if (OutsideHandleColor.Contains("7001"))   //Silver Grey
                {
                    OutsideRGB[0] = 140;
                    OutsideRGB[1] = 150;
                    OutsideRGB[2] = 157;
                }
                else if (OutsideHandleColor.Contains("9016"))   //Traffic White
                {
                    OutsideRGB[0] = 241;
                    OutsideRGB[1] = 240;
                    OutsideRGB[2] = 234;
                }
                else if (OutsideHandleColor.Contains("9005"))   //Jet Black
                {
                    OutsideRGB[0] = 14;
                    OutsideRGB[1] = 14;
                    OutsideRGB[2] = 16;
                }
                else if (OutsideHandleColor.Contains("9010"))   //Pure White
                {
                    OutsideRGB[0] = 241;
                    OutsideRGB[1] = 236;
                    OutsideRGB[2] = 225;
                }
                else if (OutsideHandleColor.Contains("7016"))   //Anthracite Grey
                {
                    OutsideRGB[0] = 56;
                    OutsideRGB[1] = 62;
                    OutsideRGB[2] = 66;
                }
                else if (OutsideHandleColor.Contains("8022"))   //Black-Brown
                {

                    OutsideRGB[0] = 26;
                    OutsideRGB[1] = 23;
                    OutsideRGB[2] = 24;
                }
                else if (OutsideHandleColor.Contains("INOX"))   //Stainless Steel
                {
                    OutsideRGB[0] = 172;
                    OutsideRGB[1] = 178;
                    OutsideRGB[2] = 180;
                }
                else
                {
                    OutsideRGB[0] = 0;
                    OutsideRGB[1] = 255;
                    OutsideRGB[2] = 0;
                }

            }

            {   //Assign RGB for Door Hinges
                if (HingeColor == null)
                {
                    HingeRGB[0] = 140;
                    HingeRGB[1] = 150;
                    HingeRGB[2] = 157;
                }
                else if (HingeColor.Contains("7001"))   //Silver Grey
                {
                    HingeRGB[0] = 140;
                    HingeRGB[1] = 150;
                    HingeRGB[2] = 157;
                }
                else if (HingeColor.Contains("9016"))   //Traffic White
                {
                    HingeRGB[0] = 241;
                    HingeRGB[1] = 240;
                    HingeRGB[2] = 234;
                }
                else if (HingeColor.Contains("9005"))   //Jet Black
                {
                    HingeRGB[0] = 14;
                    HingeRGB[1] = 14;
                    HingeRGB[2] = 16;
                }
                else if (HingeColor.Contains("7016"))   //Anthracite Grey
                {
                    HingeRGB[0] = 56;
                    HingeRGB[1] = 62;
                    HingeRGB[2] = 66;
                }
                else if (HingeColor.Contains("8022"))   //Black-Brown
                {

                    HingeRGB[0] = 26;
                    HingeRGB[1] = 23;
                    HingeRGB[2] = 24;
                }
                else if (HingeColor.Contains("INOX"))   //Stainless Steel
                {
                    HingeRGB[0] = 172;
                    HingeRGB[1] = 178;
                    HingeRGB[2] = 180;
                }
                else
                {
                    HingeRGB[0] = 0;
                    HingeRGB[1] = 255;
                    HingeRGB[2] = 0;
                }

            }

            XBrush brushI = new XSolidBrush(XColor.FromArgb(InsideRGB[0], InsideRGB[1], InsideRGB[2]));
            XBrush brushO = new XSolidBrush(XColor.FromArgb(OutsideRGB[0], OutsideRGB[1], OutsideRGB[2]));
            XBrush brushH = new XSolidBrush(XColor.FromArgb(HingeRGB[0], HingeRGB[1], HingeRGB[2]));
            

            double d1 = 33 * L.Scale;
            double d2 = 25 * L.Scale;
            double d3 = 36 * L.Scale;
            double d4 = 66 * L.Scale;
            double d5 = 35 * L.Scale;   //recess outside width && sliding handle rosette width
            double d6 = 25 * L.Scale;   //recess inside width
            double d7 = 23.5 * L.Scale; //sliding handle width
            double h = 128 * L.Scale;
            double h1 = 75 * L.Scale;
            double h2 = 148 * L.Scale;
            double h3 = 800 * L.Scale;
            double h4 = 1000 * L.Scale;
            double h5 = 45.5 * L.Scale;
            double h6 = 300 * L.Scale;
            double h7 = 105 * L.Scale;   //recess outside height
            double h8 = 70 * L.Scale;   //recess inside height
            double h9 = 160 * L.Scale;   //sliding handle rosette height
            double h10 = 250 * L.Scale; //sliding handle height
            double h11 = 194.5 * L.Scale; //sliding handle offset

            //set point where handle will be drawn (need two for door system)
            TraceData.Point p = V.HandleLocation;
            TraceData.Point cp = V.CornerPoints[0];
            TraceData.Point cp1 = V.CornerPoints[1];
            TraceData.Point cp2 = V.CornerPoints[2];
            

            XPoint p0 = LayoutPoint(new TraceData.Point(0, 0), L);
            XPoint p1 = LayoutPoint(p, L);
            XPoint p2 = LayoutPoint(cp, L);
            XPoint p3 = LayoutPoint(cp1, L);
            XPoint p4 = LayoutPoint(cp2, L);
            

            double halfH = 0;
            if(p2.Y > p3.Y)
            {
                halfH = (p2.Y - p3.Y) / 2;
            }
            else
            {
                halfH = (p3.Y - p2.Y) / 2;
            }
            bool mirror = true;
            
            if(V.Type != "SingleDoor" && V.Type != "DoubleDoor" && V.Type != "Sliding")
            {
                //draw window handle geometry
                g.DrawEllipse(pen, brushI, p1.X - d1 / 2, p1.Y - d1 / 2, d1, d1);
                g.DrawEllipse(pen, brushI, p1.X - d2 / 2, p1.Y + h - d2 / 2, d2, d2);
                g.DrawRectangle(pen, brushI, p1.X - d2 / 2, p1.Y, d2, h);
                g.DrawEllipse(pen, brushI, p1.X - d2 / 2, p1.Y - d2 / 2, d2, d2);
            }
            else if (V.Type == "DoubleDoor")
            {

                TraceData.Point cp3 = V.CornerPoints[4];
                TraceData.Point cp4 = V.CornerPoints[6];
               XPoint p5 = LayoutPoint(cp3, L);
                XPoint p6 = LayoutPoint(cp4, L);

                if (Side == "Front")
                {
                    //draw front door handle
                    mirror = false;
                    if (V.OutsideHandleArticleName != "240099")
                    {
                        // draw standard handle
                        if (V.HandleSide == "Right")
                        {
                            g.DrawRoundedRectangle(pen, brushO, p1.X - (d1 / 2) - (d2 / 2) - 3, p1.Y - (h1 / 2), d1, h1, d1, d1);
                            g.DrawRoundedRectangle(pen, brushO, p1.X - h2 - 3, p1.Y - d2 / 2, h2, d2, d2, d2);
                        }
                        else
                        {
                            g.DrawRoundedRectangle(pen, brushO, p1.X - (d2 / 2) + 4, p1.Y - (h1 / 2), d1, h1, d1, d1);
                            g.DrawRoundedRectangle(pen, brushO, p1.X + 4, p1.Y - d2 / 2, h2, d2, d2, d2);
                        }
                    }
                    else if (V.OutsideHandleArticleName == "240099")
                    {
                        //draw door pull
                        if (V.HandleSide == "Right")
                        {
                            g.DrawRoundedRectangle(pen, brushO, p1.X - h2 - 3, p1.Y - (h3 / 2), h2, d2, d2, d2);
                            g.DrawRoundedRectangle(pen, brushO, p1.X - h2 - 3, p1.Y + (h3 / 2), h2, d2, d2, d2);
                            g.DrawRectangle(pen, brushO, p1.X - h2 - 3, p1.Y - (h4 / 2), d1, h4);
                        }
                        else
                        {
                            g.DrawRoundedRectangle(pen, brushO, p1.X + 4, p1.Y - (h3 / 2), h2, d2, d2, d2);
                            g.DrawRoundedRectangle(pen, brushO, p1.X + 4, p1.Y + (h3 / 2), h2, d2, d2, d2);
                            g.DrawRectangle(pen, brushO, p1.X + h2 + 4 - (d2 / 2), p1.Y - (h4 / 2), d1, h4);
                        }
                    }

                }
                else
                {
                    //draw back door handle
                    if (V.HandleSide == "Right")
                    {
                        g.DrawRoundedRectangle(pen, brushI, p1.X - (d1 / 2) - (d2 / 2), p1.Y - (h1 / 2), d1, h1, d1, d1);
                        g.DrawRoundedRectangle(pen, brushI, p1.X - h2, p1.Y - d2 / 2, h2, d2, d2, d2);

                        //bottom active surface mounted hinge
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - h, d3, h5);
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p2.Y - h - h5, d4, h5);
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - h - (h5 * 2), d3, h5);

                        //top active surface mounted hinge
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h, d3, h5);
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p3.Y + h + h5, d4, h5);
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h + (h5 * 2), d3, h5);

                        //bottom passive surface mounted hinge
                        g.DrawRectangle(pen, brushH, p6.X + 2, p2.Y - h, -d3, h5);
                        g.DrawRectangle(pen, brushH, p6.X + 1, p2.Y - h - h5, -d4, h5);
                        g.DrawRectangle(pen, brushH, p6.X + 2, p2.Y - h - (h5 * 2), -d3, h5);

                        //top passive surface mounted hinge
                        g.DrawRectangle(pen, brushH, p6.X + 2, p3.Y + h, -d3, h5);
                        g.DrawRectangle(pen, brushH, p6.X + 1, p3.Y + h + h5, -d4, h5);
                        g.DrawRectangle(pen, brushH, p6.X + 2, p3.Y + h + (h5 * 2), -d3, h5);

                        if (V.HingeCondition == 2)
                        {
                            //middle active hinge
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - halfH - h5, d3, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p2.Y - halfH, d4, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - halfH + h5, d3, h5);

                            //middle passive hinge
                            g.DrawRectangle(pen, brushH, p6.X + 2, p2.Y - halfH - h5, -d3, h5);
                            g.DrawRectangle(pen, brushH, p6.X + 1, p2.Y - halfH, -d4, h5);
                            g.DrawRectangle(pen, brushH, p6.X + 2, p2.Y - halfH + h5, -d3, h5);
                        }
                        if (V.HingeCondition == 3)
                        {
                            //top lower active hinge
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h6, d3, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p3.Y + h6 + h5, d4, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h6 + (h5 * 2), d3, h5);
                                                       
                            //top lower passive hinge
                            g.DrawRectangle(pen, brushH, p6.X + 2, p3.Y + h6, -d3, h5);
                            g.DrawRectangle(pen, brushH, p6.X + 1, p3.Y + h6 + h5, -d4, h5);
                            g.DrawRectangle(pen, brushH, p6.X + 2, p3.Y + h6 + (h5 * 2), -d3, h5);
                        }
                        if (V.HingeCondition == 4)
                        {
                            //middle active hinge
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - halfH - h5, d3, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p2.Y - halfH, d4, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - halfH + h5, d3, h5);

                            //top lower active hinge
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h6, d3, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p3.Y + h6 + h5, d4, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h6 + (h5 * 2), d3, h5);

                            //middle passive hinge
                            g.DrawRectangle(pen, brushH, p6.X + 2, p2.Y - halfH - h5, -d3, h5);
                            g.DrawRectangle(pen, brushH, p6.X + 1, p2.Y - halfH, -d4, h5);
                            g.DrawRectangle(pen, brushH, p6.X + 2, p2.Y - halfH + h5, -d3, h5);

                            //top lower passive hinge
                            g.DrawRectangle(pen, brushH, p6.X + 2, p3.Y + h6, -d3, h5);
                            g.DrawRectangle(pen, brushH, p6.X + 1, p3.Y + h6 + h5, -d4, h5);
                            g.DrawRectangle(pen, brushH, p6.X + 2, p3.Y + h6 + (h5 * 2), -d3, h5);
                        }

                    }
                    else
                    {
                        //handle
                        g.DrawRoundedRectangle(pen, brushI, p1.X - (d2 / 2) + 1, p1.Y - (h1 / 2), d1, h1, d1, d1);
                        g.DrawRoundedRectangle(pen, brushI, p1.X, p1.Y - d2 / 2, h2, d2, d2, d2);


                        //bottom active surface mounted hinge
                        g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - h, -d3, h5);
                        g.DrawRectangle(pen, brushH, p4.X + 1, p2.Y - h - h5, -d4, h5);
                        g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - h - (h5 * 2), -d3, h5);

                        //top active surface mounted hinge
                        g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h, -d3, h5);
                        g.DrawRectangle(pen, brushH, p4.X + 1, p3.Y + h + h5, -d4, h5);
                        g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h + (h5 * 2), -d3, h5);

                        //bottom passive surface mounted hinge
                        g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p2.Y - h, d3, h5);
                        g.DrawRectangle(pen, brushH, p5.X - d3 + 4, p2.Y - h - h5, d4, h5);
                        g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p2.Y - h - (h5 * 2), d3, h5);

                        //top passive surface mounted hinge
                        g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p3.Y + h, d3, h5);
                        g.DrawRectangle(pen, brushH, p5.X - d3 + 4, p3.Y + h + h5, d4, h5);
                        g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p3.Y + h + (h5 * 2), d3, h5);

                        if (V.HingeCondition == 2)
                        {
                            //middle active hinge
                            g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - halfH - h5, -d3, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 1, p2.Y - halfH, -d4, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - halfH + h5, -d3, h5);

                            //middle passive hinge
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p2.Y - halfH - h5, d3, h5);
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 4, p2.Y - halfH, d4, h5);
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p2.Y - halfH + h5, d3, h5);
                        }
                        if (V.HingeCondition == 3)
                        {
                            //top lower active hinge
                            g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h6, -d3, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 1, p3.Y + h6 + h5, -d4, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h6 + (h5 * 2), -d3, h5);

                            //top lower passive hinge
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p3.Y + h6, d3, h5);
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 4, p3.Y + h6 + h5, d4, h5);
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p3.Y + h6 + (h5 * 2), d3, h5);
                        }
                        if (V.HingeCondition == 4)
                        {
                            //middle active hinge
                            g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - halfH - h5, -d3, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 1, p2.Y - halfH, -d4, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - halfH + h5, -d3, h5);

                            //top lower active hinge
                            g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h6, -d3, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 1, p3.Y + h6 + h5, -d4, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h6 + (h5 * 2), -d3, h5);

                            //middle passive hinge
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p2.Y - halfH - h5, d3, h5);
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 4, p2.Y - halfH, d4, h5);
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p2.Y - halfH + h5, d3, h5);

                            //top lower passive hinge
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p3.Y + h6, d3, h5);
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 4, p3.Y + h6 + h5, d4, h5);
                            g.DrawRectangle(pen, brushH, p5.X - d3 + 3, p3.Y + h6 + (h5 * 2), d3, h5);
                        }
                    }

                    
                }
            }
            else if (V.Type == "SingleDoor")
            {
                if (Side == "Front")
                {
                    //draw front door handle
                    mirror = false;
                    if (V.OutsideHandleArticleName != "240099")
                    {
                        // draw standard handle
                        if (V.HandleSide == "Left")
                        {
                            g.DrawRoundedRectangle(pen, brushO, p1.X - (d1 / 2) - (d2 / 2) - 3, p1.Y - (h1 / 2), d1, h1, d1, d1);
                            g.DrawRoundedRectangle(pen, brushO, p1.X - h2 - 3, p1.Y - d2 / 2, h2, d2, d2, d2);
                        }
                        else
                        {
                            g.DrawRoundedRectangle(pen, brushO, p1.X - (d2 / 2) + 4, p1.Y - (h1 / 2), d1, h1, d1, d1);
                            g.DrawRoundedRectangle(pen, brushO, p1.X + 4, p1.Y - d2 / 2, h2, d2, d2, d2);
                        }
                    }
                    else if (V.OutsideHandleArticleName == "240099")
                    {
                        //draw door pull
                        if (V.HandleSide == "Left")
                        {
                            g.DrawRoundedRectangle(pen, brushO, p1.X - h2 - 3, p1.Y - (h3 / 2), h2, d2, d2, d2);
                            g.DrawRoundedRectangle(pen, brushO, p1.X - h2 - 3, p1.Y + (h3 / 2), h2, d2, d2, d2);
                            g.DrawRectangle(pen, brushO, p1.X - h2 - 3, p1.Y - (h4 / 2), d1, h4);
                        }
                        else
                        {
                            g.DrawRoundedRectangle(pen, brushO, p1.X + 4, p1.Y - (h3 / 2), h2, d2, d2, d2);
                            g.DrawRoundedRectangle(pen, brushO, p1.X + 4, p1.Y + (h3 / 2), h2, d2, d2, d2);
                            g.DrawRectangle(pen, brushO, p1.X + h2 + 4 - (d2/2), p1.Y - (h4 / 2), d1, h4);
                        }
                    }

                }
                else
                {
                    //draw back door handle
                   
                    if(V.HandleSide == "Left")
                    {
                        //handle
                        g.DrawRoundedRectangle(pen, brushI, p1.X - (d1 / 2) - (d2 / 2), p1.Y - (h1 / 2), d1, h1, d1, d1);
                        g.DrawRoundedRectangle(pen, brushI, p1.X - h2, p1.Y - d2 / 2, h2, d2, d2, d2);

                        //bottom surface mounted hinge
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - h, d3, h5);
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p2.Y - h - h5, d4, h5);
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - h - (h5*2), d3, h5);

                        //top surface mounted hinge
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h, d3, h5);
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p3.Y + h + h5, d4, h5);
                        g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h + (h5 * 2), d3, h5);

                        if(V.HingeCondition == 2)
                        {
                            //middle hinge
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - halfH - h5 , d3, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p2.Y - halfH, d4, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - halfH + h5, d3, h5);
                        }
                        if (V.HingeCondition == 3)
                        {
                            //top lower hinge
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h6, d3, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p3.Y + h6 + h5, d4, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h6 + (h5 * 2), d3, h5);
                        }
                        if (V.HingeCondition == 4)
                        {
                            //middle hinge
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - halfH - h5, d3, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p2.Y - halfH, d4, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p2.Y - halfH + h5, d3, h5);

                            //top lower hinge
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h6, d3, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 4, p3.Y + h6 + h5, d4, h5);
                            g.DrawRectangle(pen, brushH, p2.X - d3 + 3, p3.Y + h6 + (h5 * 2), d3, h5);
                        }
                    }
                    else
                    {
                        //handle
                         g.DrawRoundedRectangle(pen, brushI, p1.X - (d2 / 2) + 1, p1.Y - (h1 / 2), d1, h1, d1, d1);
                         g.DrawRoundedRectangle(pen, brushI, p1.X, p1.Y - d2 / 2, h2, d2, d2, d2);

                        //bottom surface mounted hinge
                        g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - h, -d3, h5);
                        g.DrawRectangle(pen, brushH, p4.X + 1, p2.Y - h - h5, -d4, h5);
                        g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - h - (h5 * 2), -d3, h5);

                        //top surface mounted hinge
                        g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h, -d3, h5);
                        g.DrawRectangle(pen, brushH, p4.X + 1, p3.Y + h + h5, -d4, h5);
                        g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h + (h5 * 2), -d3, h5);

                        if (V.HingeCondition == 2)
                        {
                            //middle hinge
                            g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - halfH - h5, -d3, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 1, p2.Y - halfH, -d4, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - halfH + h5, -d3, h5);
                        }
                        if (V.HingeCondition == 3)
                        {
                            //top lower hinge
                            g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h6, -d3, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 1, p3.Y + h6 + h5, -d4, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h6 + (h5 * 2), -d3, h5);
                        }
                        if (V.HingeCondition == 4)
                        {
                            //middle hinge
                            g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - halfH - h5, -d3, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 1, p2.Y - halfH, -d4, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 2, p2.Y - halfH + h5, -d3, h5);

                            //top lower hinge
                            g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h6, -d3, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 1, p3.Y + h6 + h5, -d4, h5);
                            g.DrawRectangle(pen, brushH, p4.X + 2, p3.Y + h6 + (h5 * 2), -d3, h5);
                        }

                    }
                   
                }
            }
            else if (V.Type == "Sliding")
            {
                if(Side == "Front") //Handle Recess (would need to not draw if reinforcement doesn't hide it)
                {
                    //draw handle recess geometry
                    g.DrawRoundedRectangle(pen, brushO, p1.X - d5 / 2, p1.Y - d5 / 2, d5, h7, d5, d5);
                    g.DrawRoundedRectangle(pen, brushO, p1.X - d6 / 2, p1.Y, d6, h8, d6, d6);


                }
                else if (Side == "Back")    //Lift and Slide Handle
                {
                    //draw sliding handle geometry
                    g.DrawRoundedRectangle(pen, brushI, p1.X - d5 / 2, p1.Y - d5 / 2, d5, h9, d5, d5);
                    g.DrawRoundedRectangle(pen, brushI, p1.X - d7 / 2, p1.Y - d7 / 2 - h11, d7, h10, d7, d7);
                }

            }


            if(Side != "Front")
            {
                //Dim on Front elevation doesn't look great for door handles
                p1.X = p0.X;

                string dimText = Proposal.TraceFraction(p.Y / 25.4) + "\" (" + p.Y.ToString("F0") + ")";
                DimensionLine(g, p0, p1, "Vertical", dimText, 0, mirror);
            }
            

        }

        public static void DrawSlidingReinforcement(XGraphics g, Layout L, TraceData td, string Side, int[] RGB)
        {
            
            if(td.Vents.Count > 0 && td.Vents[0].SlidingDoorSystemID != 1)
            {
                return;
            }
            

            XPen pen = new XPen(XColor.FromArgb(30, 30, 30), .125);
           
            XBrush brush = new XSolidBrush(XColor.FromArgb(RGB[0], RGB[1], RGB[2]));

            foreach (var V in td.Vents)
            {
                if(V.InterlockReinforcement != true)
                {
                    continue;
                }

                //set point where handle will be drawn (need two for door system)
                TraceData.Point p = V.HandleLocation;
                TraceData.Point cp = V.CornerPoints[0];
                TraceData.Point cp1 = V.CornerPoints[1];
                TraceData.Point cp2 = V.CornerPoints[2];


                XPoint p1 = LayoutPoint(p, L);
                XPoint p2 = LayoutPoint(cp, L);
                XPoint p3 = LayoutPoint(cp1, L);
                XPoint p4 = LayoutPoint(cp2, L);

                int ventcount = td.Vents.Count;
                if (V.Number != ventcount)
                {
                    if ((ventcount == 6 && V.Number == 3) || (ventcount == 4 && V.Number == 2))
                    {
                        if (Side == "Front")
                        {
                            g.DrawRectangle(pen, brush, p1.X - 22 * L.Scale, p2.Y - 50 * L.Scale, 44 * L.Scale, -1 * (Math.Abs(p2.Y - p3.Y) - (100 * L.Scale)));
                        }
                        else if (Side == "Back")
                        {
                            continue;
                        }
                    }
                    
                    else
                    {
                        g.DrawRectangle(pen, brush, p4.X - 22 * L.Scale, p2.Y - 50 * L.Scale, 44 * L.Scale, -1 * (Math.Abs(p2.Y - p3.Y) - (100 * L.Scale)));
                    }
                    if ((ventcount == 6 && V.Number == 4) || (ventcount == 4 && V.Number == 3))
                    {
                        if (Side == "Front")
                        {
                            g.DrawRectangle(pen, brush, p1.X - 22 * L.Scale, p2.Y - 50 * L.Scale, 44 * L.Scale, -1 * (Math.Abs(p2.Y - p3.Y) - (100 * L.Scale)));
                        }
                        else if (Side == "Back")
                        {
                            continue;
                        }
                    }
                }

            }

            //draw fake double vent profile for 2D/1 and 3F
            if(td.Vents.Count == 4)
            {
                foreach (var v in td.Vents)
                {
                    if (v.Number == 2)
                    {                        
                        TraceData.Point cp2 = v.CornerPoints[2];
                        TraceData.Point cp3 = v.CornerPoints[3];

                        XPoint p3 = LayoutPoint(cp2, L);
                        XPoint p4 = LayoutPoint(cp3, L);

                        if (Side == "Front")
                        {
                            g.DrawRectangle(pen, brush, p3.X - (4 * L.Scale), p3.Y + (40 * L.Scale), 8 * L.Scale, (Math.Abs(p3.Y - p4.Y) - (80 * L.Scale)));
                        }
                        else if (Side == "Back")
                        {
                            g.DrawRectangle(pen, brush, p3.X - (4 * L.Scale), p3.Y + (40 * L.Scale), 8 * L.Scale, (Math.Abs(p3.Y - p4.Y) - (80 * L.Scale)));
                        }

                    } 
                }
            }
            else if (td.Vents.Count == 6)
            {
                foreach (var v in td.Vents)
                {
                    if (v.Number == 3)
                    {
                        TraceData.Point cp2 = v.CornerPoints[2];
                        TraceData.Point cp3 = v.CornerPoints[3];

                        XPoint p3 = LayoutPoint(cp2, L);
                        XPoint p4 = LayoutPoint(cp3, L);

                        if (Side == "Front")
                        {
                            g.DrawRectangle(pen, brush, p3.X - (4 * L.Scale), p3.Y + (40 * L.Scale), 8 * L.Scale, (Math.Abs(p3.Y - p4.Y) - (80 * L.Scale)));
                        }
                        else if (Side == "Back")
                        {
                            g.DrawRectangle(pen, brush, p3.X - (4 * L.Scale), p3.Y + (40 * L.Scale), 8  * L.Scale, (Math.Abs(p3.Y - p4.Y) - (80 * L.Scale)));
                        }

                    }
                }
            }
        }

        public static void DrawOpeningSymbol(XGraphics g, TraceData.Vent V, Layout L, string Side)
        {
            XPen pen = new XPen(XColor.FromArgb(130, 130, 130), .125);

            XPoint p0 = LayoutPoint(V.CornerPoints[0], L);
            XPoint p1 = LayoutPoint(V.CornerPoints[1], L);
            XPoint p2 = LayoutPoint(V.CornerPoints[2], L);
            XPoint p3 = LayoutPoint(V.CornerPoints[3], L);
            XPoint p4 = LayoutPoint(V.CornerPoints[0], L);
            XPoint p5 = LayoutPoint(V.CornerPoints[1], L);
            XPoint p6 = LayoutPoint(V.CornerPoints[2], L);
            XPoint p7 = LayoutPoint(V.CornerPoints[3], L);

            if (V.Type == "DoubleDoor")
            {
                p4 = LayoutPoint(V.CornerPoints[4], L);
                p5 = LayoutPoint(V.CornerPoints[5], L);
                p6 = LayoutPoint(V.CornerPoints[6], L);
                p7 = LayoutPoint(V.CornerPoints[7], L);
            }

            if (Side == "Front" && V.Type != "Sliding" && V.Type != "Passive Sliding" && V.Type != "Sliding Fixed")
            {
                pen.DashStyle = XDashStyle.Custom;
                pen.DashPattern = new double[] { 20, 20, 20 };

            }
            if (V.Type == "TurnTilt")
            {
                g.DrawLine(pen, p0, new XPoint((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2));
                g.DrawLine(pen, new XPoint((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2), p3);

                if (V.HandleSide == "Right")
                {
                    g.DrawLine(pen, p0, new XPoint((p2.X + p3.X) / 2, (p2.Y + p3.Y) / 2));
                    g.DrawLine(pen, new XPoint((p2.X + p3.X) / 2, (p2.Y + p3.Y) / 2), p1);

                }
                if (V.HandleSide == "Left")
                {
                    g.DrawLine(pen, p3, new XPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2));
                    g.DrawLine(pen, new XPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2), p2);

                }
            }
            else if (V.Type == "SingleDoor")
            {
                
                if (V.HandleSide == "Left")
                {
                    g.DrawLine(pen, p0, new XPoint((p2.X + p3.X) / 2, (p2.Y + p3.Y) / 2));
                    g.DrawLine(pen, new XPoint((p2.X + p3.X) / 2, (p2.Y + p3.Y) / 2), p1);

                }
                if (V.HandleSide == "Right")
                {
                    g.DrawLine(pen, p3, new XPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2));
                    g.DrawLine(pen, new XPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2), p2);

                }
            }
            else if (V.Type == "DoubleDoor")
            {
                if (V.HandleSide == "Left")
                {
                    g.DrawLine(pen, p3, new XPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2));
                    g.DrawLine(pen, new XPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2), p2);
                    g.DrawLine(pen, p5, new XPoint((p6.X + p7.X) / 2, (p6.Y + p7.Y) / 2));
                    g.DrawLine(pen, new XPoint((p6.X + p7.X) / 2, (p6.Y + p7.Y) / 2), p4);

                }
                if (V.HandleSide == "Right")
                {
                    g.DrawLine(pen, p0, new XPoint((p2.X + p3.X) / 2, (p2.Y + p3.Y) / 2));
                    g.DrawLine(pen, new XPoint((p2.X + p3.X) / 2, (p2.Y + p3.Y) / 2), p1);
                    g.DrawLine(pen, p6, new XPoint((p4.X + p5.X) / 2, (p4.Y + p5.Y) / 2));
                    g.DrawLine(pen, new XPoint((p4.X + p5.X) / 2, (p4.Y + p5.Y) / 2), p7);

                }

            }
            else if (V.Type == "Sliding" || V.Type == "Passive Sliding")
            {

                var midX = (p1.X + p2.X) / 2;
                var midY = (p0.Y + p1.Y) / 2;
                var lineLength = 250 * L.Scale;
                var triLength = 100 * L.Scale;
                var triangle = new XPoint[3];
                var triangleL = new XPoint(midX + (triLength/2), midY);
                var triangleR = new XPoint(midX - (triLength / 2), midY);
                var triangleT = new XPoint(midX, midY - triLength);

                triangle[0] = triangleL;
                triangle[1] = triangleR;
                triangle[2] = triangleT;

                g.DrawLine(pen, new XPoint(midX, midY), new XPoint(midX, midY + lineLength));
                g.DrawPolygon(pen, triangle);
                
                if (V.HandleSide == "Right")
                {
                    g.DrawLine(pen, new XPoint(midX, midY - triLength), new XPoint(midX + lineLength, midY - triLength));

                    triangle[0] = new XPoint(midX + lineLength, midY - triLength + triLength/2);
                    triangle[1] = new XPoint(midX + lineLength, midY - triLength - triLength/2);
                    triangle[2] = new XPoint(midX + lineLength + triLength, midY - triLength);

                    g.DrawPolygon(pen, triangle);

                }
                if (V.HandleSide == "Left")
                {
                    g.DrawLine(pen, new XPoint(midX, midY - triLength), new XPoint(midX - lineLength, midY - triLength));

                    triangle[0] = new XPoint(midX - lineLength, midY - triLength + triLength / 2);
                    triangle[1] = new XPoint(midX - lineLength, midY - triLength - triLength / 2);
                    triangle[2] = new XPoint(midX - lineLength - triLength, midY - triLength);

                    g.DrawPolygon(pen, triangle);
                }
            }
            else if (V.Type == "Sliding Fixed")
            {
                var midX = (p1.X + p2.X) / 2;
                var midY = (p0.Y + p1.Y) / 2;
                var lineLength = 200 * L.Scale;
                g.DrawLine(pen, new XPoint(midX, midY - lineLength), new XPoint(midX, midY + lineLength));
                g.DrawLine(pen, new XPoint(midX - lineLength, midY), new XPoint(midX + lineLength, midY));
            }
        
        
        
        }
        public static void DrawDimensions(XGraphics g, List<TraceData.Point> Points, Layout L,
                                          string Direction, bool Mirror = false)
        {

            List<TraceData.Point> pts = new List<TraceData.Point>();

            if (Direction == "Vertical")
            {
                foreach (var point in Points) pts.Add(new TraceData.Point(0, point.Y));
                pts = pts.OrderByDescending(p => p.Y).ToList();
                pts.Reverse();

            }
            else
            {
                foreach (var point in Points) pts.Add(new TraceData.Point(point.X, 0));
                pts = pts.OrderByDescending(p => p.X).ToList();
                pts.Reverse();

            }

            // Add inline Dimensions


            int icount = 0;
            for (int i = 0; i < pts.Count - 2; i++)
            {
                double DX = Math.Abs(pts.ElementAt(i + 1).X - pts.ElementAt(i).X);
                double DY = Math.Abs(pts.ElementAt(i + 1).Y - pts.ElementAt(i).Y);
                double value = Math.Sqrt(DX * DX + DY * DY);
                string dimText = Proposal.TraceFraction(value / 25.4) + "\" (" + value.ToString("F0") + ")";

                if (DX > .01 || DY > .01)
                {
                    icount++;
                    XPoint start = LayoutPoint(pts[i], L);
                    XPoint end = LayoutPoint(pts[i + 1], L);

                    DimensionLine(g, start, end, Direction, dimText, 0, Mirror);

                }
            }

            //// Add overall Dimsnsion.

            if (icount > 1)
            {
                double DX = Math.Abs(pts[0].X - pts[pts.Count - 1].X);
                double DY = Math.Abs(pts[0].Y - pts[pts.Count - 1].Y);
                double value = Math.Sqrt(DX * DX + DY * DY);
                string dimText = Proposal.TraceFraction(value / 25.4) + "\" (" + value.ToString("F0") + ")";

                XPoint start = LayoutPoint(pts[0], L);
                XPoint end = LayoutPoint(pts[pts.Count - 1], L);

                DimensionLine(g, start, end, Direction, dimText, 1, Mirror);

            }


        }
        public static void DimensionLine(XGraphics g, XPoint P1, XPoint P2,
                                         string Direction, string DimText, int Level = 0, bool Mirror = false)
        {

            XPen Pen1 = new XPen(XColor.FromArgb(38, 38, 38), .125);
            XPen Pen2 = new XPen(XColor.FromArgb(138, 138, 138), .0625);

            double d = 12 * (Level + 1);

            if (Direction == "Vertical")
            {
                g.DrawLine(Pen1, new XPoint(P1.X - d, P1.Y), new XPoint(P2.X - d, P2.Y));
                g.DrawLine(Pen2, new XPoint(P1.X - (d + 3), P1.Y), new XPoint(P1.X - 3, P1.Y));
                g.DrawLine(Pen2, new XPoint(P2.X - (d + 3), P2.Y), new XPoint(P2.X - 3, P2.Y));
                if (P1.Y > P2.Y)
                {
                    DrawArrow(g, new XPoint(P1.X - d, P1.Y), "Down");
                    DrawArrow(g, new XPoint(P2.X - d, P2.Y), "Up");

                }
                else
                {
                    DrawArrow(g, new XPoint(P1.X - d, P1.Y), "Up");
                    DrawArrow(g, new XPoint(P2.X - d, P2.Y), "Down");
                }

                DrawText(g, new XPoint(P1.X - d, (P1.Y + P2.Y) / 2), "Vertical", DimText, Mirror);

            }
            else
            {
                g.DrawLine(Pen1, new XPoint(P1.X, P1.Y + d), new XPoint(P2.X, P2.Y + d));
                g.DrawLine(Pen2, new XPoint(P1.X, P1.Y + d + 3), new XPoint(P1.X, P1.Y + 3));
                g.DrawLine(Pen2, new XPoint(P2.X, P2.Y + d + 3), new XPoint(P2.X, P2.Y + 3));

                if (P1.X > P2.X)
                {
                    DrawArrow(g, new XPoint(P1.X, P1.Y + d), "Right");
                    DrawArrow(g, new XPoint(P2.X, P2.Y + d), "Left");
                }
                else
                {
                    DrawArrow(g, new XPoint(P1.X, P1.Y + d), "Left");
                    DrawArrow(g, new XPoint(P2.X, P2.Y + d), "Right");
                }

                DrawText(g, new XPoint((P1.X + P2.X) / 2, P1.Y + d), "Horizontal", DimText, Mirror);
            }

        }
        public static void Specifications(TraceData TD, PdfDocument myTemplate, List<TraceData.Point> Points)
        {
            InsertValue(myTemplate, "Project", TD.Project.Name);
            InsertValue(myTemplate, "Address", TD.Project.Location);
            InsertValue(myTemplate, "Configuration", TD.Project.Configuration);

            InsertValue(myTemplate, "UserName", TD.Project.Client.ToUpper());       //Dealer's username
            InsertValue(myTemplate, "Date", DateTime.Today.ToString("d"));          //Date proposal was created

            if (TD.Vents.Count > 0 && TD.Vents[0].SlidingType != null)
            {
                InsertValue(myTemplate, "SystemType", "ASE 60");
            }
            else
            {
                InsertValue(myTemplate, "SystemType", "AWS 75-SI+");
            }
            
            InsertValue(myTemplate, "FrameColor", TD.Project.Color);

            var bb = new BoundingBox(Points);
            double dx = Math.Abs(bb.xmax - bb.xmin) / 25.4;
            double dy = Math.Abs(bb.ymax - bb.ymin) / 25.4;
            string text = Proposal.TraceFraction(dx, 16) + "\" X " + Proposal.TraceFraction(dy, 16) + "\"";
            InsertValue(myTemplate, "Size1", text);
            text = Proposal.TraceFraction(dx + 1, 16) + "\" X " + Proposal.TraceFraction(dy + 1, 16) + "\"";
            InsertValue(myTemplate, "Size2", text);

            InsertValue(myTemplate, "GlassBrand", TD.Glazing.Manufacturer);
            InsertValue(myTemplate, "GlassComp", TD.Glazing.Makeup);

           
            if (TD.Vents.Count > 0)
            {
                if (TD.Vents[0].DoorSystemType != null)         //ADS Hardware
                {
                    InsertValue(myTemplate, "VentType", "ADS 75.SI+");

                    //Get RAL colors of door hardware

                    string insideColor;
                    string outsideColor;
                    string hingeColor;
                    string[] separator = { " - " };

                    if (TD.Vents[0].InsideHandleColor == "INOX")
                    {
                        insideColor = "INOX";
                    }
                    else if (TD.Vents[0].InsideHandleColor == null)
                    {
                        insideColor = "N/A";
                    }
                    else
                    {
                        string[] insideWords = TD.Vents[0].InsideHandleColor.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        insideColor = insideWords[1];
                    }

                    if (TD.Vents[0].OutsideHandleColor == "INOX")
                    {
                        outsideColor = "INOX";
                    }
                    else if (TD.Vents[0].OutsideHandleColor == null)
                    {
                        outsideColor = "N/A";
                    }
                    else
                    {
                        string[] outsideWords = TD.Vents[0].OutsideHandleColor.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        outsideColor = outsideWords[1];
                    }

                    if (TD.Vents[0].HingeColor == "INOX")
                    {
                        hingeColor = "INOX";
                    }
                    else if (TD.Vents[0].HingeColor == null)
                    {
                        hingeColor = "N/A";
                    }
                    else
                    {
                        string[] hingeWords = TD.Vents[0].HingeColor.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        hingeColor = hingeWords[1];
                    }


                    InsertValue(myTemplate, "VentHardware", ("Inside Handle: " + insideColor + ", Outside Handle: " + outsideColor + ", \nHinges: " + hingeColor));
                }
                else if (TD.Vents[0].SlidingType != null) //ASE Hardware
                {
                    InsertValue(myTemplate, "VentType", "ASE 60");

                    //Get RAL colors of door hardware

                    string insideColor = "N/A";
                    string outsideColor = "N/A";
                    
                    string[] separator = { " - " };

                    foreach (var v in TD.Vents)
                    {
                        if(v.Type != "Sliding")
                        {
                            continue;
                        }

                        if (v.InsideHandleColor == "INOX")
                        {
                            insideColor = "INOX";
                        }
                        else
                        {
                            string[] insideWords = v.InsideHandleColor.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                            insideColor = insideWords[1];
                        }

                        if (v.OutsideHandleColor == "INOX")
                        {
                            outsideColor = "INOX";
                        }
                        else
                        {
                            string[] outsideWords = v.OutsideHandleColor.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                            outsideColor = outsideWords[1];
                        }

                    }

                    InsertValue(myTemplate, "VentHardware", ("Inside Handle: " + insideColor + ", Outside Handle: " + outsideColor));
                }

                else    //Window Hardware
                {
                    InsertValue(myTemplate, "VentHardware", "Handle: " + TD.Vents[0].HandleColor);
                }
            }
            else
            {
                InsertValue(myTemplate, "VentHardware", "N/A");
            }
            

            
            InsertValue(myTemplate, "AluAlloy", TD.Project.Alloy);

            InsertValue(myTemplate, "GlassWind Load Pressure", (TD.Specs.WindPressure * 20.89).ToString("F0"));
            InsertValue(myTemplate, "SystemWind Load Pressure", (TD.Specs.WindPressure * 20.89).ToString("F0"));

            InsertValue(myTemplate, "GlassUValue", (TD.Glazing.U_value / 5.678).ToString("F2"));

            if (TD.Specs.U_Value == 0)
            {
                InsertValue(myTemplate, "SystemUValue", "N/A");
            }
            else
            {
                InsertValue(myTemplate, "SystemUValue", (TD.Specs.U_Value / 5.678).ToString("F2"));
            }
            
            InsertValue(myTemplate, "GlassSHGC", (TD.Glazing.SHGC).ToString());

           
            InsertValue(myTemplate, "SystemSHGC", "N/A");
           
           
            InsertValue(myTemplate, "GlassSTC", TD.Glazing.STC.ToString(""));

            if (TD.Specs.STC == 0)
            {
                InsertValue(myTemplate, "SystemSTC", "N/A");
            }
            else
            {
                InsertValue(myTemplate, "SystemSTC", TD.Specs.STC.ToString(""));
            }
            InsertValue(myTemplate, "GlassOITC", TD.Glazing.OITC.ToString(""));

            if (TD.Specs.OITC == 0)
            {
                InsertValue(myTemplate, "SystemOITC", "N/A");
            }
            else
            {
                InsertValue(myTemplate, "SystemOITC", TD.Specs.OITC.ToString(""));
            }
        }

        #endregion

        #region Utility
        public static PdfDocument Create(string TemplateFile, string OutputFile)
        {
            string output = CreateNewPdf(TemplateFile, OutputFile);
            PdfDocument myTemplate = PdfReader.Open(output, PdfDocumentOpenMode.Modify);

            return myTemplate;
        }

        public static void Close(PdfDocument myTemplate, string Output)
        {

            myTemplate.Flatten();
            myTemplate.Save(Output);
            myTemplate.Close();
            
            //System.Diagnostics.Process.Start(Output);
        }
        public static string CreateNewPdf(string source, string destination)
        {

            var fopen = File.Open(destination, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            fopen.Close();

            File.Copy(source, destination, true);
            return destination;

        }
        public static void InsertValue(PdfDocument myTemplate, string fieldName, string fieldValue)
        {
            try
            {

                PdfAcroForm form = myTemplate.AcroForm;

                if (form.Elements.ContainsKey("/NeedAppearances"))
                {
                    form.Elements["/NeedAppearances"] = new PdfSharp.Pdf.PdfBoolean(true);
                }
                else
                {
                    form.Elements.Add("/NeedAppearances", new PdfSharp.Pdf.PdfBoolean(true));
                }

                // Get all form fields of the whole document

                PdfAcroField.PdfAcroFieldCollection fields = form.Fields;

                // this sets the value for the field selected

                PdfAcroField field = fields[fieldName];
                PdfTextField txtField;
                if ((txtField = field as PdfTextField) != null)
                {
                    txtField.ReadOnly = false;
                    txtField.Value = new PdfString(fieldValue);
                    txtField.ReadOnly = true;


                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static void DrawText(XGraphics g, XPoint P, string Direction, string Text, bool Mirror = false)
        {
            XBrush grayBrush = new XSolidBrush(XColor.FromArgb(102, 102, 102));
            XFont drawFont = new XFont("Arial", 6, XFontStyle.Regular);
            XSize size = g.MeasureString(Text, drawFont);
            if (Direction == "Vertical")
            {
                XMatrix mirrorMatrix = new XMatrix(-1, 0, 0, 1, 2 * P.X, 0);


                g.RotateAtTransform(-90, P);

                if (Mirror)
                {
                    g.MultiplyTransform(mirrorMatrix);
                    g.RotateAtTransform(180, P);
                }

                g.DrawString(Text, drawFont, grayBrush, P.X - size.Width / 2, P.Y - 1.5);
                

                if (Mirror)
                {
                    g.MultiplyTransform(mirrorMatrix);
                    g.RotateAtTransform(180, P);
                }

                g.RotateAtTransform(+90, P);

            }
            else
            {
                XMatrix mirrorMatrix = new XMatrix(-1, 0, 0, 1, 2 * P.X, 0);
                if (Mirror) g.MultiplyTransform(mirrorMatrix);
                g.DrawString(Text, drawFont, grayBrush, P.X - size.Width / 2, P.Y - 1.5);
                if (Mirror) g.MultiplyTransform(mirrorMatrix);
            }



        }
        public static void DrawArrow(XGraphics g, XPoint P, string Direction)
        {

            XBrush grayBrush = new XSolidBrush(XColor.FromArgb(102, 102, 102));
            XPen grayPen = new XPen(XColor.FromArgb(38, 38, 38), .25);

            XPoint point1 = new XPoint(P.X, P.Y);
            XPoint point2 = new XPoint(P.X, P.Y);
            XPoint point3 = new XPoint(P.X, P.Y);

            double arrowLength = 5;
            double arrowWidth = 1.0;

            switch (Direction)
            {
                case "Down":
                    point2 = new XPoint(P.X - arrowWidth, P.Y - arrowLength);
                    point3 = new XPoint(P.X + arrowWidth, P.Y - arrowLength);
                    break;
                case "Up":
                    point2 = new XPoint(P.X - arrowWidth, P.Y + arrowLength);
                    point3 = new XPoint(P.X + arrowWidth, P.Y + arrowLength);
                    break;

                case "Left":
                    point2 = new XPoint(P.X + arrowLength, P.Y - arrowWidth);
                    point3 = new XPoint(P.X + arrowLength, P.Y + arrowWidth);
                    break;

                case "Right":
                    point2 = new XPoint(P.X - arrowLength, P.Y - arrowWidth);
                    point3 = new XPoint(P.X - arrowLength, P.Y + arrowWidth);
                    break;

                default:
                    break;

            }

            XPoint[] curvePoints = { point1, point2, point3 };
            g.DrawPolygon(grayPen, grayBrush, curvePoints, XFillMode.Alternate);

        }
        public static XPoint LayoutPoint(TraceData.Point Point, Layout L)
        {
            XPoint p = new XPoint();
            p.X = L.Xoffset + Point.X * L.Scale;
            p.Y = L.Yoffset + (L.Height - Point.Y * L.Scale);
            return p;
        }
        public static void Scale(List<TraceData.Point> Points, ref Layout L)
        {

            BoundingBox bb = new BoundingBox(Points);
            double sx = L.Width / (bb.xmax - bb.xmin);
            double sy = L.Height / (bb.ymax - bb.ymin);
            L.Scale = Math.Min(sx, sy) * 0.9;


            L.Xoffset += (L.Width - (bb.xmax - bb.xmin) * L.Scale) / 2;
            L.Yoffset -= (L.Height - (bb.ymax - bb.ymin) * L.Scale) / 2;



        }

        #endregion

    }
}

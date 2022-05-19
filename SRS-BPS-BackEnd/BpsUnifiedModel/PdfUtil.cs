using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BpsUnifiedModelLib
{
    public class MemberGeometricInfo
    {
        public int MemberID { get; set; }
        public int MemberType { get; set; }
        public double[] PointCoordinates { get; set; }
        public double offsetA { get; set; }
        public double offsetB { get; set; }
        public double width { get; set; }
        public int outerFrameSide { get; set; }
    }

    public class GlassGeometricInfo
    {
        public int GlassID { get; set; }
        public double[] PointCoordinates { get; set; }
        public double[] CornerCoordinates { get; set; }
        public double[] VentCoordinates { get; set; }
        public string VentOpeningDirection { get; set; }
        public string VentOperableType { get; set; }
    }

    public class PdfUtil
    {
        public static List<MemberGeometricInfo> ComputeMemberGeometricInfo(ModelInput input)
        {
            List<MemberGeometricInfo> memberGeometricInfos = new List<MemberGeometricInfo>();
            double tolerance = 1e-4;
            foreach (Member member in input.Geometry.Members)
            {
                Section section = input.Geometry.Sections.Single(item => item.SectionID == member.SectionID);
                Point pointA = input.Geometry.Points.Single(item => item.PointID == member.PointA);
                Point pointB = input.Geometry.Points.Single(item => item.PointID == member.PointB);
                List<Infill> gList = input.Geometry.Infills.Where(x => x.BoundingMembers.Contains(member.MemberID)).ToList();
                double offsetA = 0;
                double offsetB = 0;
                double pAX = pointA.X;
                double pAY = pointA.Y;
                double pBX = pointB.X;
                double pBY = pointB.Y;
                int OuterFrameSide = -1;
                foreach (Infill glass in gList)
                {
                    int[] mullionIndex = { 0, 2 };
                    int[] transomIndex = { 1, 3 };
                    if (member.MemberType == 1 && OuterFrameSide < 0)  //outer frame
                    {
                        foreach (int i in transomIndex)
                        {
                            if (glass.BoundingMembers[i] == member.MemberID)
                            {
                                OuterFrameSide = i;
                                offsetA = section.OutsideW;
                                offsetA = (pBX > pAX) ? offsetA : -offsetA;
                                offsetB = -offsetA;
                            }
                        }
                        foreach (int i in mullionIndex)
                        {
                            if (glass.BoundingMembers[i] == member.MemberID)
                            {
                                OuterFrameSide = i;
                                offsetA = section.OutsideW;
                                offsetA = (pBY > pAY) ? offsetA : -offsetA;
                                offsetB = -offsetA;
                            }
                        }
                    }
                    else if (member.MemberType == 2)  // mullion
                    {
                        foreach (int i in transomIndex)
                        {
                            Member m = input.Geometry.Members.Single(item => item.MemberID == glass.BoundingMembers[i]);
                            double y = input.Geometry.Points.Single(item => item.PointID == m.PointA).Y;
                            if (offsetA < tolerance && Math.Abs(y - pointA.Y) < tolerance)
                            {
                                Section s = input.Geometry.Sections.Single(item => item.SectionID == m.SectionID);
                                offsetA = s.SectionType == 1 ? s.OutsideW : s.OutsideW / 2;
                                offsetA = (pBY > pAY) ? offsetA : -offsetA;
                                continue;
                            }
                            if (offsetB < tolerance && Math.Abs(y - pointB.Y) < tolerance)
                            {
                                Section s = input.Geometry.Sections.Single(item => item.SectionID == m.SectionID);
                                offsetB = s.SectionType == 1 ? s.OutsideW : s.OutsideW / 2;
                                offsetB = (pBY > pAY) ? -offsetB : offsetB;
                                continue;
                            }
                        }
                    }
                    else if (member.MemberType == 3)  //transom
                    {
                        foreach (int i in mullionIndex)
                        {
                            Member m = input.Geometry.Members.Single(item => item.MemberID == glass.BoundingMembers[i]);
                            double x = input.Geometry.Points.Single(item => item.PointID == m.PointA).X;
                            if (Math.Abs(offsetA) < tolerance && Math.Abs(x - pointA.X) < tolerance)
                            {
                                Section s = input.Geometry.Sections.Single(item => item.SectionID == m.SectionID);
                                offsetA = s.SectionType == 1 ? s.OutsideW : s.OutsideW / 2;
                                offsetA = (pBX > pAX) ? offsetA : -offsetA;
                                continue;
                            }
                            if (offsetB < tolerance && Math.Abs(x - pointB.X) < tolerance)
                            {
                                Section s = input.Geometry.Sections.Single(item => item.SectionID == m.SectionID);
                                offsetB = s.SectionType == 1 ? s.OutsideW : s.OutsideW / 2;
                                offsetB = (pBX > pAX) ? -offsetB : offsetB;
                                continue;
                            }
                        }
                    }
                }

                MemberGeometricInfo memberGeometricInfo = new MemberGeometricInfo
                {
                    MemberID = member.MemberID,
                    MemberType = member.MemberType,
                    PointCoordinates = new double[4] { pAX, pAY, pBX, pBY },
                    offsetA = offsetA,
                    offsetB = offsetB,
                    width = section.OutsideW,
                    outerFrameSide = OuterFrameSide,
                };

                memberGeometricInfos.Add(memberGeometricInfo);
                //_PDFResult.MemberGeometricInfos.Add(memberGeometricInfo);
            }
            return memberGeometricInfos;
        }

        public static void PdfDrawStructure(List<MemberGeometricInfo> memberGeometricInfos, List<GlassGeometricInfo> glassGeometricInfos, PdfDocument myTemplate, string solverType, double ModelWidth, double ModelHeight, double ModelOriginX, double ModelOriginY, double OuterFrameWidth, int pageNumber = 1, double xLocation = 300, double yLocation = 310)
        {
            XGraphics g = XGraphics.FromPdfPage(myTemplate.Pages[pageNumber]);
            XPen grayPen = new XPen(XColor.FromArgb(38, 38, 38), .25);
            XPen grayDashPen = new XPen(XColor.FromArgb(38, 38, 38), .6);
            grayDashPen.DashStyle = XDashStyle.Dash;
            grayDashPen.DashPattern = new double[4] { 6, 4, 6, 4 };
            XPen blackPen = new XPen(XColor.FromArgb(0, 0, 0), 0.75);
            XBrush greenBrush = new XSolidBrush(XColor.FromArgb(120, 185, 40));
            XBrush grayBrush = new XSolidBrush(XColor.FromArgb(102, 102, 102));
            XBrush blackBrush = new XSolidBrush(XColor.FromArgb(0, 0, 0));
            XFont drawFont = new XFont("Univers for Schueco 330 Light", 8, XFontStyle.Regular);
            XFont drawBold = new XFont("Univers for Schueco 330 Light", 8, XFontStyle.Bold);
            string drawString = string.Empty;

            double ImageHeight, ImageWidth;

            double modelAspectRatio = ModelWidth / ModelHeight;
            if (modelAspectRatio > 1)
            {
                ImageWidth = 175;
                ImageHeight = ImageWidth / modelAspectRatio;
            }
            else
            {
                ImageHeight = 175;
                ImageWidth = modelAspectRatio * ImageHeight;
            }
            double xs = 0, ys = 0;

            XRect rectMemberID, rectGlassID, rectGlass, rectVent;
            foreach (MemberGeometricInfo memberGeometricInfo in memberGeometricInfos)
            {
                // draw member lines
                double x0 = ModelOriginX;
                double y0 = ModelOriginY;
                double x1 = memberGeometricInfo.PointCoordinates[0];
                double y1 = memberGeometricInfo.PointCoordinates[1];
                double x2 = memberGeometricInfo.PointCoordinates[2];
                double y2 = memberGeometricInfo.PointCoordinates[3];
                double x3 = x1;
                double y3 = y1;
                double x4 = x2;
                double y4 = y2;
                if (memberGeometricInfo.MemberType == 1)
                {
                    double offA = memberGeometricInfo.offsetA;
                    double offB = memberGeometricInfo.offsetB;
                    if (memberGeometricInfo.outerFrameSide == 0)  // vertical outer frame
                    {
                        x4 = x1 + memberGeometricInfo.width;
                        y4 = y1 + offA;
                        x3 = x4;
                        y3 = y2 + offB;
                    }
                    else if (memberGeometricInfo.outerFrameSide == 1)
                    {
                        x4 = x1 + offA;
                        y4 = y1 - memberGeometricInfo.width;
                        x3 = x2 + offB;
                        y3 = y4;
                    }
                    else if (memberGeometricInfo.outerFrameSide == 2)
                    {
                        x4 = x1 - memberGeometricInfo.width;
                        y4 = y1 + offA;
                        x3 = x4;
                        y3 = y2 + offB;
                    }
                    else if (memberGeometricInfo.outerFrameSide == 3)
                    {
                        x4 = x1 + offA;
                        y4 = y1 + memberGeometricInfo.width;
                        x3 = x2 + offB;
                        y3 = y4;
                    }
                }
                else if (memberGeometricInfo.MemberType == 2)
                {
                    y1 = y1 + memberGeometricInfo.offsetA;
                    y2 = y2 + memberGeometricInfo.offsetB;
                }
                else if (memberGeometricInfo.MemberType == 3)
                {
                    x1 = x1 + memberGeometricInfo.offsetA;
                    x2 = x2 + memberGeometricInfo.offsetB;
                }
                x1 = xLocation + (x1 - x0) / ModelWidth * ImageWidth;
                y1 = yLocation - (y1 - y0) / ModelHeight * ImageHeight;
                x2 = xLocation + (x2 - x0) / ModelWidth * ImageWidth;
                y2 = yLocation - (y2 - y0) / ModelHeight * ImageHeight;
                x3 = xLocation + (x3 - x0) / ModelWidth * ImageWidth;
                y3 = yLocation - (y3 - y0) / ModelHeight * ImageHeight;
                x4 = xLocation + (x4 - x0) / ModelWidth * ImageWidth;
                y4 = yLocation - (y4 - y0) / ModelHeight * ImageHeight;
                double w = memberGeometricInfo.width / ModelWidth * ImageWidth;
                if (memberGeometricInfo.MemberType == 1)
                {
                    g.DrawLine(grayPen, x1, y1, x2, y2);
                    g.DrawLine(grayPen, x2, y2, x3, y3);
                    g.DrawLine(grayPen, x3, y3, x4, y4);
                    g.DrawLine(grayPen, x4, y4, x1, y1);
                }
                else if (memberGeometricInfo.MemberType == 2) //mullion
                {
                    //g.DrawLine(grayDashPen, x1, y1, x2, y2); // dash dot line
                    g.DrawLine(grayPen, x1 - w / 2, y1, x2 - w / 2, y2);
                    g.DrawLine(grayPen, x1 + w / 2, y1, x2 + w / 2, y2);
                }
                else if (memberGeometricInfo.MemberType == 3) //transom
                {
                    //g.DrawLine(grayDashPen, x1, y1, x2, y2); // dash dot line
                    g.DrawLine(grayPen, x1, y1 - w / 2, x2, y2 - w / 2);
                    g.DrawLine(grayPen, x1, y1 + w / 2, x2, y2 + w / 2);
                }

                // draw member label
                double LabelOffset = OuterFrameWidth * 1000 * Math.Max(ImageWidth / ModelWidth, ImageHeight / ModelHeight) + 2;
                xs = (x1 + x2) / 2 + LabelOffset + 2;
                ys = (y1 + y2) / 2 - LabelOffset;
                drawString = $"{memberGeometricInfo.MemberID}";
                rectMemberID = new XRect(xs - 2, ys - 7.5, 9, 9);
                if (memberGeometricInfo.MemberType == 2 || memberGeometricInfo.MemberType == 3)
                {
                    XFont memberFont = new XFont("Univers for Schueco 330 Light", 8, XFontStyle.Regular);
                    g.DrawString(drawString, drawFont, blackBrush, xs, ys);
                    g.DrawRectangle(grayPen, rectMemberID);
                }
                else
                {
                    XFont memberFont = new XFont("Univers for Schueco 330 Light", 8, XFontStyle.Regular);
                    g.DrawString(drawString, drawFont, grayBrush, xs, ys);
                    g.DrawRectangle(grayPen, rectMemberID);
                }
            }

            foreach (GlassGeometricInfo glassGeometricInfo in glassGeometricInfos)
            {
                double x0 = ModelOriginX;
                double y0 = ModelOriginY;
                xs = glassGeometricInfo.PointCoordinates[0];
                ys = glassGeometricInfo.PointCoordinates[1];
                xs = xLocation + (xs - x0) / ModelWidth * ImageWidth;
                ys = yLocation - (ys - y0) / ModelHeight * ImageHeight;
                drawString = $"{glassGeometricInfo.GlassID}";
                g.DrawString(drawString, drawFont, grayBrush, xs, ys);

                rectGlassID = new XRect(xs - 2, ys - 7.5, 9, 9);
                g.DrawArc(grayPen, rectGlassID, 0, 360);

                // draw glass and vent
                x0 = ModelOriginX;
                y0 = ModelOriginY;
                double x1 = glassGeometricInfo.CornerCoordinates[0];
                double y1 = glassGeometricInfo.CornerCoordinates[1];
                double x2 = glassGeometricInfo.CornerCoordinates[2];
                double y2 = glassGeometricInfo.CornerCoordinates[3];
                double dx = (x2 - x1) / ModelWidth * ImageWidth;
                double dy = (y2 - y1) / ModelHeight * ImageHeight;
                x1 = xLocation + (x1 - x0) / ModelWidth * ImageWidth;
                y1 = yLocation - (y1 - y0) / ModelHeight * ImageHeight;
                rectGlass = new XRect(x1, y1 - dy, dx, dy);
                g.DrawRectangle(grayPen, rectGlass);

                if (!(glassGeometricInfo.VentCoordinates is null))
                {
                    x1 = glassGeometricInfo.VentCoordinates[0];
                    y1 = glassGeometricInfo.VentCoordinates[1];
                    x2 = glassGeometricInfo.VentCoordinates[2];
                    y2 = glassGeometricInfo.VentCoordinates[3];
                    dx = (x2 - x1) / ModelWidth * ImageWidth;
                    dy = (y2 - y1) / ModelHeight * ImageHeight;
                    x1 = xLocation + (x1 - x0) / ModelWidth * ImageWidth;
                    y1 = yLocation - (y1 - y0) / ModelHeight * ImageHeight;
                    rectVent = new XRect(x1, y1 - dy, dx, dy);
                    g.DrawRectangle(grayPen, rectVent);
                    // draw symbol
                    XPen vSymbolPen;
                    if (glassGeometricInfo.VentOpeningDirection.Contains("Inward"))
                    {
                        vSymbolPen = grayDashPen;
                    }
                    else
                    {
                        vSymbolPen = grayPen;
                    }
                    if (glassGeometricInfo.VentOperableType.Contains("Tilt"))
                    {
                        if ((glassGeometricInfo.VentOperableType.Contains("Right") && glassGeometricInfo.VentOpeningDirection.Contains("Inward")) ||
                            (glassGeometricInfo.VentOperableType.Contains("Left") && glassGeometricInfo.VentOpeningDirection.Contains("Outward")))
                        {
                            g.DrawLine(vSymbolPen, x1, y1, x1 + dx / 2, y1 - dy);
                            g.DrawLine(vSymbolPen, x1 + dx / 2, y1 - dy, x1 + dx, y1);
                            g.DrawLine(vSymbolPen, x1, y1, x1 + dx, y1 - dy / 2);
                            g.DrawLine(vSymbolPen, x1 + dx, y1 - dy / 2, x1, y1 - dy);
                        }
                        else
                        {
                            g.DrawLine(vSymbolPen, x1, y1, x1 + dx / 2, y1 - dy);
                            g.DrawLine(vSymbolPen, x1 + dx / 2, y1 - dy, x1 + dx, y1);
                            g.DrawLine(vSymbolPen, x1, y1 - dy / 2, x1 + dx, y1);
                            g.DrawLine(vSymbolPen, x1, y1 - dy / 2, x1 + dx, y1 - dy);
                        }

                    }
                    else if (glassGeometricInfo.VentOperableType.Contains("Side"))
                    {
                        if ((glassGeometricInfo.VentOperableType.Contains("Right") && glassGeometricInfo.VentOpeningDirection.Contains("Inward")) ||
                            (glassGeometricInfo.VentOperableType.Contains("Left") && glassGeometricInfo.VentOpeningDirection.Contains("Outward")))
                        {
                            g.DrawLine(vSymbolPen, x1, y1, x1 + dx, y1 - dy / 2);
                            g.DrawLine(vSymbolPen, x1 + dx, y1 - dy / 2, x1, y1 - dy);
                        }
                        else
                        {
                            g.DrawLine(vSymbolPen, x1, y1 - dy / 2, x1 + dx, y1);
                            g.DrawLine(vSymbolPen, x1, y1 - dy / 2, x1 + dx, y1 - dy);
                        }
                    }
                    else if (glassGeometricInfo.VentOperableType.Contains("Bottom"))
                    {
                        g.DrawLine(vSymbolPen, x1, y1, x1 + dx / 2, y1 - dy);
                        g.DrawLine(vSymbolPen, x1 + dx / 2, y1 - dy, x1 + dx, y1);
                    }
                }
            }

            // get dimension info
            double[] xdimensions = memberGeometricInfos.Select(item => item.PointCoordinates[0]).Distinct().ToArray();
            double[] ydimensions = memberGeometricInfos.Select(item => item.PointCoordinates[1]).Distinct().ToArray();
            Array.Sort(xdimensions);
            Array.Sort(ydimensions);
            double modelLength = xdimensions.Last() - xdimensions.First();
            double modelHeight = ydimensions.Last() - ydimensions.First();
            xdimensions = xdimensions.Select(item => (item - xdimensions.First()) / xdimensions.Last()).ToArray();
            ydimensions = ydimensions.Select(item => (item - ydimensions.First()) / ydimensions.Last()).ToArray();

            //// draw Structural legend
            if (solverType == "Structual")
            {
                xs = xLocation + 5;
                ys = yLocation + 15 + 11 * xdimensions.Count();
                drawString = $"n   Glass ID";
                if (Thread.CurrentThread.CurrentCulture.Name == "de-DE")
                {
                    drawString = $"n   Glas-Position";
                }
                g.DrawString(drawString, drawFont, grayBrush, xs, ys);
                rectGlassID = new XRect(xs - 2, ys - 7.5, 9, 9);
                g.DrawArc(grayPen, rectGlassID, 0, 360);

                ys = ys + 15;
                drawString = $"n   Structural Member ID";
                if (Thread.CurrentThread.CurrentCulture.Name == "de-DE")
                {
                    drawString = $"n   Statik Mitglied ID";
                }
                drawFont = new XFont("Univers for Schueco 330 Light", 8, XFontStyle.Bold);
                g.DrawString(drawString, drawFont, blackBrush, xs, ys);
                rectMemberID = new XRect(xs - 2, ys - 7.5, 9, 9);
                g.DrawRectangle(blackPen, rectMemberID);
            }

            // draw Thermal legend
            if (solverType == "Thermal")
            {
                xs = xLocation + 5;
                ys = yLocation + 15 + 11 * xdimensions.Count();
                drawString = $"n   Glass ID";
                if (Thread.CurrentThread.CurrentCulture.Name == "de-DE")
                {
                    drawString = $"n   Glas-Position";
                }
                g.DrawString(drawString, drawFont, blackBrush, xs, ys);
                rectGlassID = new XRect(xs - 2, ys - 7.5, 9, 9);
                g.DrawArc(grayPen, rectGlassID, 0, 360);

                ys = ys + 15;
                drawString = $"n   Frame Member ID";
                if (Thread.CurrentThread.CurrentCulture.Name == "de-DE")
                {
                    drawString = $"n   Positionsnummer";
                }
                //drawFont = new XFont("Univers for Schueco 330 Light", 8, XFontStyle.Bold);
                g.DrawString(drawString, drawFont, blackBrush, xs, ys);
                rectMemberID = new XRect(xs - 2, ys - 7.5, 9, 9);
                g.DrawRectangle(grayPen, rectMemberID);

                ys = ys + 15;
                drawString = "     Article Combination";
                if (Thread.CurrentThread.CurrentCulture.Name == "de-DE")
                {
                    drawString = "     Artikelkombination";
                }
                g.DrawString(drawString, drawBold, blackBrush, xs - 2, ys);

                g.DrawString("n", drawBold, blackBrush, xs - 2, ys - 2);
                g.DrawLine(blackPen, xs + 5, ys - 5, xs + 5, ys + 5);
            }

            // draw Acoustic legend
            if (solverType == "Acoustic")
            {
                xs = xLocation + 5;
                ys = yLocation + 15 + 11 * xdimensions.Count();
                drawString = $"n   Glass ID";
                if (Thread.CurrentThread.CurrentCulture.Name == "de-DE")
                {
                    drawString = $"n   Glas-Position";
                }
                g.DrawString(drawString, drawFont, grayBrush, xs, ys);
                rectGlassID = new XRect(xs - 2, ys - 7.5, 9, 9);
                g.DrawArc(grayPen, rectGlassID, 0, 360);

                ys = ys + 15;
                drawString = $"n   Frame Member ID";
                if (Thread.CurrentThread.CurrentCulture.Name == "de-DE")
                {
                    drawString = $"n   Positionsnummer";
                }
                drawFont = new XFont("Univers for Schueco 330 Light", 8, XFontStyle.Bold);
                g.DrawString(drawString, drawFont, blackBrush, xs, ys);
                rectMemberID = new XRect(xs - 2, ys - 7.5, 9, 9);
                g.DrawRectangle(blackPen, rectMemberID);

            }

            g.Save();
            g.Dispose();

            // draw dimensions
            double xdimLocation = xLocation;
            double ydimLocation = yLocation + 20;
            for (int i = 1; i < xdimensions.Count(); i++)
            {
                PdfDrawDimension(myTemplate.Pages[pageNumber], 0, xdimensions[i], i - 1, xdimensions[i] * modelLength, xdimLocation, ydimLocation, ImageWidth);
            }

            xdimLocation = xLocation + ImageWidth;
            ydimLocation = yLocation;
            for (int i = 1; i < ydimensions.Count(); i++)
            {
                PdfDrawDimension(myTemplate.Pages[pageNumber], 0, ydimensions[i], i - 1, ydimensions[i] * modelHeight, xdimLocation, ydimLocation, ImageHeight, true);
            }
        }

        public static void PdfDrawDimension(PdfPage myPage, double xStart, double xEnd, int nc, double value, double xLocation, double yLocation, double Length, bool isVertical = false)
        {
            // Draw beam Dimention Line
            XGraphics g = XGraphics.FromPdfPage(myPage);

            if (isVertical)
            {
                XPoint rotationCenter = new XPoint(xLocation, yLocation);
                g.RotateAtTransform(-90, rotationCenter);
                yLocation = yLocation + 20;
            }

            XPen pen = new XPen(XColor.FromArgb(38, 38, 38), .25);
            XBrush brush = new XSolidBrush(XColor.FromArgb(102, 102, 102));

            double width, x, y, beamLength;

            beamLength = Length;

            x = xLocation + xStart * beamLength;
            y = yLocation + nc * 11;
            width = (xEnd - xStart) * beamLength;

            g.DrawLine(pen, x, y, x + width, y);
            g.DrawLine(pen, x, y + 2, x, y - 11);
            g.DrawLine(pen, x + width, y + 2, x + width, y - (nc + 1) * 11);

            XPoint point1 = new XPoint(x, y);
            XPoint point2 = new XPoint(x + 5, y - 1.5);
            XPoint point3 = new XPoint(x + 5, y + 1.5);
            XPoint[] curvePoints = { point1, point2, point3 };
            g.DrawPolygon(pen, brush, curvePoints, XFillMode.Alternate);

            curvePoints[0] = new XPoint(x + width, y);
            curvePoints[1] = new XPoint(x + width - 5, y - 1.5);
            curvePoints[2] = new XPoint(x + width - 5, y + 1.5);

            g.DrawPolygon(pen, brush, curvePoints, XFillMode.Alternate);

            // draw the dimension text
            XFont drawFont = new XFont("Univers for Schueco 330 Light", 6, XFontStyle.Regular);
            String drawString = $"{value,0:N0}mm";

            XSize size = g.MeasureString(drawString, drawFont);

            double xt = x + width / 2 - size.Width / 2;

            g.DrawString(drawString, drawFont, brush, xt, y - .75);

            g.Save();
            g.Dispose();
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using netDxf;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;
using Attribute = netDxf.Entities.Attribute;
using FontStyle = netDxf.Tables.FontStyle;
using Image = netDxf.Entities.Image;
using Point = netDxf.Entities.Point;
using Trace = netDxf.Entities.Trace;
using TriangleNet;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Smoothing;
using TriangleNet.Tools;
using System.Drawing.Drawing2D;
using StructuralSolverSBA;
using BpsUnifiedModelLib;

namespace SBA
{
    public class DXFSectionProperty

    {
        List<Polygon> polygons_Al;
        List<Polygon> polygons_Plastic;
        List<Polygon> polygons_PT;

        public void ReadDXF(string filename, ref Section section)
        {
            polygons_Al = new List<Polygon>();
            polygons_Plastic = new List<Polygon>();
            polygons_PT = new List<Polygon>();

            GeneratePolygons(filename);

            List<PolyBasicProperty> pbpList = new List<PolyBasicProperty>();
            for (int i = 0; i <= 1; i++)
            {
                var poly = polygons_Al.ElementAt(i);

                // compute mesh basic property
                PolyBasicProperty pbp=GetBasicProperty(poly);
                pbpList.Add(pbp);
            }

            // make sure pbpList[0] is for the top part, pbpList[1] is for the bottom part;
            if (pbpList[1].CenteroidY > pbpList[0].CenteroidY)
            {
                PolyBasicProperty pTemp = pbpList[1];
                pbpList[1] = pbpList[0];
                pbpList[0] = pTemp;
            }

            section.Zoo = pbpList[0].Ztop;         // mm
            section.Zou = pbpList[0].Zbot;         // mm
            section.Zol = pbpList[0].Zleft;        // mm
            section.Zor = pbpList[0].Zright;       // mm
            section.Ao = pbpList[0].Area;          // mm2
            section.Io = pbpList[0].IxxC;          // mm4
            section.Ioyy = pbpList[0].IyyC;        // mm4 
            double Wo = 9.80665 * pbpList[0].Area * 2.7 / 1000; // weight in N/m

            section.Woyp = pbpList[0].Spx;
            section.Woyn = pbpList[0].Snx;
            section.Wozp = pbpList[0].Spy;
            section.Wozn = pbpList[0].Sny;

            section.Zuo = pbpList[1].Ztop;
            section.Zuu = pbpList[1].Zbot;
            section.Zul = pbpList[1].Zleft;        // mm
            section.Zur = pbpList[1].Zright;       // mm
            section.Au = pbpList[1].Area;
            section.Iu = pbpList[1].IxxC;
            section.Iuyy = pbpList[1].IyyC;   // by Wei
            double Wu = 9.80665 * pbpList[1].Area * 2.7 / 1000; // weight in N/m
            
            section.Wuyp = pbpList[1].Spx;
            section.Wuyn = pbpList[1].Snx;
            section.Wuzp = pbpList[1].Spy;
            section.Wuzn = pbpList[1].Sny;

            section.d = pbpList[0].Ztop + pbpList[0].CenteroidY - (pbpList[1].CenteroidY - pbpList[1].Zbot);

            double Wl=0, Wr=0;
            for (int i = 0; i < polygons_Plastic.Count; i++)
            {
                var poly = polygons_Plastic.ElementAt(i);

                double polyArea = GetPolyArea(poly);
                if (i == 0)  //plastic left
                {
                    Wl = 9.80665 * polyArea * 1.2 * 1.27 / 1000;
                }
                else if (i == 1)  // plastic right
                {
                    Wr = 9.80665 * polyArea * 1.2 * 1.27 / 1000;
                }
            }

            section.Weight = Wo + Wu + Wl + Wr;

        }

        // Open profile
        private static DxfDocument OpenProfileDXF(string file)
        {
            // open the profile file
            FileInfo fileInfo = new FileInfo(file);

            // check if profile file is valid
            if (!fileInfo.Exists)
            {
                Console.WriteLine("THE FILE {0} DOES NOT EXIST", file);
                Console.WriteLine();
                return null;
            }
            DxfDocument dxf = DxfDocument.Load(file, new List<string> { @".\Support" });

            // check if there has been any problems loading the file,
            if (dxf == null)
            {
                Console.WriteLine("ERROR LOADING {0}", file);
                Console.WriteLine();
                Console.WriteLine("Press a key to continue...");
                Console.ReadLine();
                return null;
            }

            return dxf;
        }

        // Store polygon from dxf file
        public void GeneratePolygons(string filename)
        {
            // read the dxf file
            DxfDocument dxfDoc;

            dxfDoc = OpenProfileDXF(filename);

            var Poly = new Polygon();

            // loop over all relevant blacks and store the hatch boundaries
            //var modelSpaceBlocks = dxfDoc.Blocks.Where(x => x.Name == "*Model_Space");
            var blocks = dxfDoc.Blocks;
            foreach (var bl in blocks)
            {
                // loop over the enteties in the block and decompose them if they belong to an aluminum layer
                var entities = bl.Entities.Where(x => x.GetType().Name == "Hatch");
                foreach (var ent in entities)
                {
                    switch (ent.Layer.Name.ToString())
                    {
                        case "0S-Alu hatch":
                            Poly = GetPolygon(ent);
                            polygons_Al.Add(Poly);
                            break;
                        case "0S-Plastic hatch":
                            Poly = GetPolygon(ent);
                            polygons_Plastic.Add(Poly);
                            break;
                        case "0S-PT hatch":
                            Poly = GetPolygon(ent);
                            polygons_PT.Add(Poly);
                            break;
                    }
                }
            }
        }


        public Polygon GetPolygon(EntityObject ent)
        {
            var Poly = new Polygon();
            HatchPattern hp = HatchPattern.Solid;
            Hatch myHatch = new Hatch(hp, false);
            myHatch = (Hatch)ent;
            int pathNumber = 0;

            foreach (var bPath in myHatch.BoundaryPaths)
            {
                pathNumber++;

                Contour contour = GetContour(bPath);

                bool hole = true;
                // Add to the poly
                if (pathNumber == 1)
                {
                    hole = false;
                }
                Poly.Add(contour, hole);
            }
            return Poly;
        }

        public Contour GetContour(HatchBoundaryPath bPath)
        {
            List<Vertex> points = new List<Vertex>();
            int numberSegments = 16;
            for (int i = 0; i < bPath.Edges.Count; i++)
            {
                switch (bPath.Edges[i].Type.ToString().ToLower())
                {
                    case "polyline":
                        var myPolyline = (netDxf.Entities.HatchBoundaryPath.Polyline)bPath.Edges[i];
                        foreach (var vertex in myPolyline.Vertexes)
                        {
                            var vPolyline = new Vertex
                            {
                                X = vertex.X,
                                Y = vertex.Y
                            };
                            points.Add(vPolyline);
                        }
                        break;

                    case "line":
                        var myLine = (netDxf.Entities.HatchBoundaryPath.Line)bPath.Edges[i];
                        var vLine = new Vertex
                        {
                            X = myLine.Start.X,
                            Y = myLine.Start.Y
                        };
                        points.Add(vLine);
                        break;

                    case "arc":
                        var myArc = (netDxf.Entities.HatchBoundaryPath.Arc)bPath.Edges[i];

                        double delta = (myArc.EndAngle - myArc.StartAngle) / numberSegments;

                        for (int j = 0; j < numberSegments; j++)
                        {
                            var vArc = new Vertex();
                            double angleArc = (myArc.StartAngle + j * delta) * Math.PI / 180.0;
                            if (myArc.IsCounterclockwise == true)
                            {
                                vArc.X = myArc.Center.X + myArc.Radius * Math.Cos(angleArc);
                                vArc.Y = myArc.Center.Y + myArc.Radius * Math.Sin(angleArc);
                            }
                            else
                            {
                                vArc.X = myArc.Center.X + myArc.Radius * Math.Cos(Math.PI + angleArc);
                                vArc.Y = myArc.Center.Y + myArc.Radius * Math.Sin(Math.PI - angleArc);
                            }

                            points.Add(vArc);
                        }
                        break;

                    case "ellipse":
                        var myEllipse = (netDxf.Entities.HatchBoundaryPath.Ellipse)bPath.Edges[i];
                        double deltaEllipse = (myEllipse.EndAngle - myEllipse.StartAngle) / numberSegments;


                        for (int j = 0; j < numberSegments; j++)
                        {
                            var vEllipse = new Vertex();
                            var ellipseRadius = Math.Sqrt(Math.Pow(myEllipse.EndMajorAxis.X, 2) + Math.Pow(myEllipse.EndMajorAxis.Y, 2));

                            double angleEllipse = (myEllipse.StartAngle + j * deltaEllipse) * Math.PI / 180.0;
                            if (myEllipse.IsCounterclockwise == true)
                            {
                                vEllipse.X = myEllipse.Center.X + ellipseRadius * Math.Cos(angleEllipse);
                                vEllipse.Y = myEllipse.Center.Y + ellipseRadius * Math.Sin(angleEllipse);
                            }
                            else
                            {
                                vEllipse.X = myEllipse.Center.X + ellipseRadius * Math.Cos(Math.PI + angleEllipse);
                                vEllipse.Y = myEllipse.Center.Y + ellipseRadius * Math.Sin(Math.PI - angleEllipse);
                            }

                            points.Add(vEllipse);
                        }
                        break;
                }
            }

            Contour contour = new Contour(points);
            return contour;
        }

        // generate mesh

        private TriangleNet.Mesh DxfMesh(Polygon poly)
        {
            // routine to generate a mesh from the contnet of poly
            // Set quality and constraint options.
            var options = new ConstraintOptions() { ConformingDelaunay = true };
            double minAngle = 15;
            double maxMeshArea = 50;
            var quality = new QualityOptions() { MinimumAngle = minAngle, MaximumArea = maxMeshArea };

            // create the mesh
            var mesh = (TriangleNet.Mesh)poly.Triangulate(options, quality);

            // make sure there are at least 1000 elements in the mesh
            while (mesh.Triangles.Count < 1000)
            {
                maxMeshArea = maxMeshArea / 2;
                quality.MaximumArea = maxMeshArea;
                mesh = (TriangleNet.Mesh)poly.Triangulate(options, quality);
            }

            // smooth the mesh
            var smoother = new SimpleSmoother();
            smoother.Smooth(mesh);

            return mesh;
        }

        // calculate poly area
        private double GetPolyArea(Polygon poly)
        {
            TriangleNet.Mesh mesh = DxfMesh(poly);  // mesh poly

            int nt = mesh.Triangles.Count;
            double Area = 0;

            // calculate area
            for (int i = 0; i < nt; i++)
            {
                MesherTriangle myTriangle = new MesherTriangle(mesh.Triangles.ElementAt(i));
                Area += myTriangle.Area;
            }

            return Area;
        }

        // calculate poly basic property
        private PolyBasicProperty GetBasicProperty(Polygon poly)
        {
            TriangleNet.Mesh mesh = DxfMesh(poly);  // mesh poly

            int nt = mesh.Triangles.Count;
            PolyBasicProperty pbp = new PolyBasicProperty();

            // set depth and width
            double xmin = 0;
            double xmax = 0;
            double ymin = 0;
            double ymax = 0;
            GetPolyBox(poly, ref xmin, ref xmax, ref ymin, ref ymax);
            pbp.Depth = Math.Abs(ymax - ymin);
            pbp.Width = Math.Abs(xmax - xmin);

            // find centroid
            for (int i = 0; i < nt; i++)
            {
                MesherTriangle myTriangle = new MesherTriangle(mesh.Triangles.ElementAt(i));

                pbp.Area += myTriangle.Area;

                pbp.CenteroidX += myTriangle.Xc * myTriangle.Area;
                pbp.CenteroidY += myTriangle.Yc * myTriangle.Area;
            }
            pbp.CenteroidX = pbp.CenteroidX / pbp.Area;
            pbp.CenteroidY = pbp.CenteroidY / pbp.Area;

            // calculate I
            for (int i = 0; i < nt; i++)
            {
                MesherTriangle myTriangle = new MesherTriangle(mesh.Triangles.ElementAt(i));
                pbp.IxxC += myTriangle.Ixx();
                pbp.IyyC += myTriangle.Iyy();
                pbp.IxyC += myTriangle.Ixy();
            }
            pbp.IxxC = pbp.IxxC - pbp.Area * pbp.CenteroidY * pbp.CenteroidY;
            pbp.IyyC = pbp.IyyC - pbp.Area * pbp.CenteroidX * pbp.CenteroidX;
            pbp.IxyC = pbp.IxyC - pbp.Area * pbp.CenteroidX * pbp.CenteroidY;

            pbp.Rx = Math.Sqrt(pbp.IxxC / pbp.Area);
            pbp.Ry = Math.Sqrt(pbp.IyyC / pbp.Area);

            pbp.Spx = (pbp.IxxC / (ymax - pbp.CenteroidY));
            pbp.Snx = (pbp.IxxC / (pbp.CenteroidY - ymin));
            pbp.Spy = (pbp.IyyC / (xmax - pbp.CenteroidX));
            pbp.Sny = (pbp.IyyC / (pbp.CenteroidX - xmin));

            pbp.Ztop = ymax - pbp.CenteroidY;
            pbp.Zbot = pbp.CenteroidY - ymin;
            pbp.Zleft = pbp.CenteroidX - xmin;
            pbp.Zright = xmax - pbp.CenteroidX;

            return pbp;
        }

        // find mesh bounding box

        public void GetPolyBox(Polygon poly, ref double minx, ref double maxx, ref double miny, ref double maxy)
        {
            // function to find the linits of mesh

            maxx = double.NegativeInfinity;
            maxy = double.NegativeInfinity;
            minx = double.PositiveInfinity;
            miny = double.PositiveInfinity;

            foreach (var vertex in poly.Points)
            {
                if (maxx <= vertex.X)
                {
                    maxx = vertex.X;
                }
                if (minx >= vertex.X)
                {
                    minx = vertex.X;
                }
                if (maxy <= vertex.Y)
                {
                    maxy = vertex.Y;
                }
                if (miny >= vertex.Y)
                {
                    miny = vertex.Y;
                }
            }
        }

        // find the centroide of a mesh

        public Vertex MesherPolygonCentroid(List<Vertex> contour)
        {
            Vertex VC = new Vertex(0, 0);
            int np = contour.Count;
            Console.WriteLine(np.ToString());

            double area = 0;
            for (int i = 0; i < np - 1; i++)
            {
                area += contour.ElementAt(i).X * contour.ElementAt(i + 1).Y - contour.ElementAt(i).Y * contour.ElementAt(i + 1).X;
            }
            area += contour.ElementAt(np - 1).X * contour.ElementAt(0).Y - contour.ElementAt(np - 1).Y * contour.ElementAt(0).X;
            area = area / 2;

            for (int i = 0; i < np - 1; i++)
            {
                VC.X += (contour.ElementAt(i).X + contour.ElementAt(i + 1).X)
                      * (contour.ElementAt(i).X * contour.ElementAt(i + 1).Y - contour.ElementAt(i).Y * contour.ElementAt(i + 1).X);
                VC.Y += (contour.ElementAt(i).Y + contour.ElementAt(i + 1).Y)
                      * (contour.ElementAt(i).X * contour.ElementAt(i + 1).Y - contour.ElementAt(i).Y * contour.ElementAt(i + 1).X);
            }

            VC.X += (contour.ElementAt(np - 1).X + contour.ElementAt(0).X)
                  * (contour.ElementAt(np - 1).X * contour.ElementAt(0).Y - contour.ElementAt(np - 1).Y * contour.ElementAt(0).X);
            VC.Y += (contour.ElementAt(np - 1).Y + contour.ElementAt(0).Y)
                  * (contour.ElementAt(np - 1).X * contour.ElementAt(0).Y - contour.ElementAt(np - 1).Y * contour.ElementAt(0).X);

            VC.X = VC.X / (6 * area);
            VC.Y = VC.Y / (6 * area);

            return VC;
        }

    }

    public class PolyBasicProperty
    {
        // information data
        public double Depth = 0;
        public double Width = 0;
        public double Area = 0;
        public double CenteroidX = 0;
        public double CenteroidY = 0;

        public double IxxC = 0;
        public double IyyC = 0;
        public double IxyC = 0;

        public double Rx = 0;
        public double Ry = 0;

        public double Spx = 0;
        public double Snx = 0;
        public double Spy = 0;
        public double Sny = 0;

        public double Ztop = 0;
        public double Zbot = 0;
        public double Zleft = 0;
        public double Zright = 0;

        public double J = 0;
        public double Cw = 0;
    }

    class MesherTriangle
    {
        TriangleNet.Geometry.Point P0 = new TriangleNet.Geometry.Point();
        TriangleNet.Geometry.Point P1 = new TriangleNet.Geometry.Point();
        TriangleNet.Geometry.Point P2 = new TriangleNet.Geometry.Point();

        public Double Area { get; set; }
        public Double Xc { get; }
        public Double Yc { get; }


        public MesherTriangle(TriangleNet.Topology.Triangle triangle)
        {
            P0 = triangle.GetVertex(0);
            P1 = triangle.GetVertex(1);
            P2 = triangle.GetVertex(2);

            Area = ((P0.X * (P1.Y - P2.Y)) + (P1.X * (P2.Y - P0.Y)) + (P2.X * (P0.Y - P1.Y))) / 2;

            Xc = (P0.X + P1.X + P2.X) / 3;
            Yc = (P0.Y + P1.Y + P2.Y) / 3;
        }


        public double Ixx()
        {
            double result = 0;
            result = (Area / 6) * (P0.Y * P0.Y + P1.Y * P1.Y + P2.Y * P2.Y +
                               P0.Y * P1.Y + P1.Y * P2.Y + P2.Y * P0.Y);
            return result;

        }
        public double Iyy()
        {
            double result = 0;
            result = (Area / 6) * (P0.X * P0.X + P1.X * P1.X + P2.X * P2.X +
                                   P0.X * P1.X + P1.X * P2.X + P2.X * P0.X);
            return result;
        }
        public double Ixy()
        {
            double result = 0;
            result = (Area / 6) * (P0.X * P0.Y + P1.X * P1.Y + P2.X * P2.Y) +
                     (Area / 12) * (P0.X * P1.Y + P1.X * P0.Y +
                                   P0.X * P2.Y + P2.X * P0.Y +
                                   P1.X * P2.Y + P2.X * P1.Y);
            return result;
        }
    }
}

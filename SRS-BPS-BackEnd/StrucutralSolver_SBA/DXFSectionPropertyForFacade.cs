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
    public class DXFFacadeSectionProperty

    {
        List<Polygon> polygons_Al;
        alglib.sparsematrix globalStiff;
        double[] meshOmega;

        public void ReadDXF(string filename, ref FacadeSection facadeSection)
        {
            polygons_Al = new List<Polygon>();

            GeneratePolygons(filename, ref facadeSection);

            List<FacadePolyBasicProperty> pbpList = new List<FacadePolyBasicProperty>();
            for (int i = 0; i < 1; i++)
            {
                var poly = polygons_Al.ElementAt(i);

                // compute mesh basic property
                FacadePolyBasicProperty pbp = GetBasicProperty(poly);
                pbpList.Add(pbp);
            }

            // make sure pbpList[0] is for the top part, pbpList[1] is for the bottom part;
            //if (pbpList[1].CenteroidY > pbpList[0].CenteroidY)
            //{
            //    FacadePolyBasicProperty pTemp = pbpList[1];
            //    pbpList[1] = pbpList[0];
            //    pbpList[0] = pTemp;
            //}

            facadeSection.Zo = pbpList[0].Ztop;             // mm
            facadeSection.Zu = pbpList[0].Zbot;             // mm
            facadeSection.Zl = pbpList[0].Zleft;            // mm
            facadeSection.Zr = pbpList[0].Zright;            // mm
            facadeSection.A = pbpList[0].Area;              // mm^2
            facadeSection.Iyy = pbpList[0].IxxC;            // mm^4
            facadeSection.Izz = pbpList[0].IyyC;            // mm^4 
            facadeSection.J = pbpList[0].J;
            facadeSection.Width = Convert.ToInt32(pbpList[0].Zright + pbpList[0].Zleft);
            facadeSection.OutsideW = Convert.ToInt32(pbpList[0].Zright + pbpList[0].Zleft);
            facadeSection.BTDepth = Convert.ToInt32(pbpList[0].Ztop + pbpList[0].CenteroidY);

            double W = 0.0;
            if (facadeSection.Material == "Aluminum")
            {
               W  = pbpList[0].Area * 1 * 2.7 / 1e6; // weight in kg/mm
            }
            if (facadeSection.Material == "Steel")
            {
                W = pbpList[0].Area * 1 * 7.8 / 1e6; // weight in kg/mm
            }
            facadeSection.Weight = W;

            facadeSection.Wyy = Math.Min(facadeSection.Iyy / pbpList[0].Ztop, facadeSection.Iyy / pbpList[0].Zbot);
            facadeSection.Wzz = Math.Min(facadeSection.Izz / pbpList[0].Zleft, facadeSection.Izz / pbpList[0].Zright);

            facadeSection.Ry = pbpList[0].Rx;
            facadeSection.Rz = pbpList[0].Ry;
            facadeSection.Wyp = pbpList[0].Spx;
            facadeSection.Wyn = pbpList[0].Snx;
            facadeSection.Wzp = pbpList[0].Spy;
            facadeSection.Wzn = pbpList[0].Sny;
            facadeSection.Ys = pbpList[0].Xs;
            facadeSection.Zs = pbpList[0].Ys - pbpList[0].CenteroidY;
            facadeSection.Cw = pbpList[0].Cw;
            facadeSection.Beta_torsion = pbpList[0].Beta;
            facadeSection.Zy = pbpList[0].Zx;
            facadeSection.Zz = pbpList[0].Zy;
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
        public void GeneratePolygons(string filename, ref FacadeSection facadeSection)
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
                    if (ent.Layer.Name.ToString() == "0S-Alu hatch")
                    {
                        Poly = GetPolygon(ent);
                        polygons_Al.Add(Poly);
                        facadeSection.Material = "Aluminum";
                    }
                    if (ent.Layer.Name.ToString() == "0S-Steel hatch")
                    {
                        Poly = GetPolygon(ent);
                        polygons_Al.Add(Poly);
                        facadeSection.Material = "Steel";
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

                        for (int j = 0; j < myPolyline.Vertexes.Count(); j++)
                        {
                            if (myPolyline.Vertexes.Count() > 3000 && j % 3 != 0 && j != myPolyline.Vertexes.Count() - 1)
                            {
                                continue;
                            }
                            var vertix = new Vertex();
                            vertix.X = myPolyline.Vertexes.ElementAt(j).X * (-1);
                            vertix.Y = myPolyline.Vertexes.ElementAt(j).Y;
                            points.Add(vertix);
                        }

                        //foreach (var vertex in myPolyline.Vertexes)
                        //{
                        //    var vPolyline = new Vertex
                        //    {
                        //        X = vertex.X,
                        //        Y = vertex.Y
                        //    };
                        //    points.Add(vPolyline);
                        //}
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
            // var smoother = new SimpleSmoother();
            // smoother.Smooth(mesh);

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
                FacadeMesherTriangle myTriangle = new FacadeMesherTriangle(mesh.Triangles.ElementAt(i));
                Area += myTriangle.Area;
            }

            return Area;
        }

        // calculate poly basic property
        private FacadePolyBasicProperty GetBasicProperty(Polygon poly)
        {
            TriangleNet.Mesh mesh = DxfMesh(poly);  // mesh poly

            int nt = mesh.Triangles.Count;
            FacadePolyBasicProperty pbp = new FacadePolyBasicProperty();

            #region Basic Properties
            // set depth and width
            double xmin = 0;
            double xmax = 0;
            double ymin = 0;
            double ymax = 0;
            GetPolyBox(poly, ref xmin, ref xmax, ref ymin, ref ymax);
            pbp.Depth = Math.Abs(ymax - ymin);
            pbp.Width = Math.Abs(xmax - xmin);

            // find centroid and total area
            for (int i = 0; i < nt; i++)
            {
                FacadeMesherTriangle myTriangle = new FacadeMesherTriangle(mesh.Triangles.ElementAt(i));

                pbp.Area += myTriangle.Area;

                pbp.CenteroidX += myTriangle.Xc * myTriangle.Area;
                pbp.CenteroidY += myTriangle.Yc * myTriangle.Area;
            }
            pbp.CenteroidX = pbp.CenteroidX / pbp.Area;
            pbp.CenteroidY = pbp.CenteroidY / pbp.Area;

            // calculate I
            for (int i = 0; i < nt; i++)
            {
                FacadeMesherTriangle myTriangle = new FacadeMesherTriangle(mesh.Triangles.ElementAt(i));
                pbp.Ixx += myTriangle.Ixx();
                pbp.Iyy += myTriangle.Iyy();
                pbp.Ixy += myTriangle.Ixy();
            }
            pbp.IxxC = pbp.Ixx - pbp.Area * pbp.CenteroidY * pbp.CenteroidY;
            pbp.IyyC = pbp.Iyy - pbp.Area * pbp.CenteroidX * pbp.CenteroidX;
            pbp.IxyC = pbp.Ixy - pbp.Area * pbp.CenteroidX * pbp.CenteroidY;

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

            // calculate J
            //pbp.J = pbp.IxxC + pbp.IyyC;
            #endregion 

            #region Advanced Properties

            //  minimize the bandwidth
            mesh.Renumber(NodeNumbering.CuthillMcKee);

            // set up an sparse global stiffness matrix
            int nv = mesh.Vertices.Count;

            //  global stiffnes is moved tot he common (global araa) to be sheard with the shear correctiob routine
            // alglib.sparsematrix globalStiff;
            alglib.sparsecreate(nv, nv, 0, out globalStiff);

            // loop over all triangles
            foreach (var t in mesh.Triangles)
            {
                // create elemment stiffnessmatrix
                var triStiff = MesherStiff(t);

                // assemble stiffness matrix
                for (int i = 0; i < 3; i++)
                {
                    int nodei = t.GetVertex(i).ID;
                    for (int j = 0; j < 3; j++)
                    {
                        int nodej = t.GetVertex(j).ID;
                        alglib.sparseadd(globalStiff, nodei, nodej, triStiff[i, j]);
                    }
                }
            }

            // set the warping function of an arbitary node to zero to ensure stablity Dirichlet 
            // for now just add a big number to the diagonal of the last  point on the mesh
            alglib.sparseadd(globalStiff, nv - 1, nv - 1, 1000000);

            // convert the Hash-Table matrix to sKs
            alglib.sparseconverttosks(globalStiff);

            // initialize solution matrices
            double[] bVector = new double[nv];
            int[] nodeIndex = new int[nv];
            Array.Resize(ref meshOmega, nv);

            for (int i = 0; i < nv; i++)
            {
                bVector[i] = 0;
                nodeIndex[i] = 0;
                meshOmega[i] = 0;
            }

            // find the cross reffernse node array if the nodes are renumbered
            for (int i = 0; i < nv; i++) { nodeIndex[mesh.Vertices.ElementAt(i).ID] = i; }

            // loop over all of the edges and construct the bVector
            // create load vector using Neumann boundry condition
            foreach (var edge in mesh.Edges)
            {
                if (edge.Label == 1)
                {
                    int index0 = nodeIndex[edge.P0];
                    int index1 = nodeIndex[edge.P1];

                    double value = MesherNuemann(mesh, index0, index1);
                    bVector[edge.P0] = bVector[edge.P0] + value;
                    bVector[edge.P1] = bVector[edge.P1] + value;
                }
            }

            // solve for the warpping function
            alglib.sparsesolvesks(globalStiff, nv, true, bVector, out alglib.sparsesolverreport rep, out meshOmega);

            // find warping dependent section properties

            // 1. Sanit Venant Torsional Constant
            pbp.J = 0;
            for (int i = 0; i < nv; i++)
            {
                pbp.J += meshOmega[i] * bVector[i];
            }
            pbp.J = pbp.Ixx + pbp.Iyy - pbp.J;

            /* //  minimize the bandwidth
             mesh.Renumber(NodeNumbering.CuthillMcKee);

             // set up an sparse global stiffness matrix
             int nv = mesh.Vertices.Count;
             alglib.sparsematrix globalStiff;
             alglib.sparsecreate(nv, nv, 0, out globalStiff);

             // loop over all triangles
             foreach (var t in mesh.Triangles)
             {
                 // create elemment stiffnessmatrix
                 var triStiff = MesherStiff(t);

                 // assemble stiffness matrix
                 for (int i = 0; i < 3; i++)
                 {
                     int nodei = t.GetVertex(i).ID;
                     for (int j = 0; j < 3; j++)
                     {
                         int nodej = t.GetVertex(j).ID;
                         alglib.sparseadd(globalStiff, nodei, nodej, triStiff[i, j]);
                     }
                 }
             }

             // set the warping function of an arbitary node to zero to ensure stablity Dirichlet 
             // for now just add a big number to the diagonal of the last  point on the mesh
             alglib.sparseadd(globalStiff, nv - 1, nv - 1, 1000000);

             // convert the Hash-Table matrix to sKs
             alglib.sparseconverttosks(globalStiff);

             // initialize solution matrices
             double[] bVector = new double[nv];
             int[] nodeIndex = new int[nv];
             double[] Omega = new double[nv];
             Array.Resize(ref Omega, nv);

             for (int i = 0; i < nv; i++)
             {
                 bVector[i] = 0;
                 nodeIndex[i] = 0;
                 Omega[i] = 0;
             }

             // find the cross reffernse node array if the nodes are renumbered
             for (int i = 0; i < nv; i++) { nodeIndex[mesh.Vertices.ElementAt(i).ID] = i; }

             // loop over all of the edges and construct the bVector
             // create load vector using Neumann boundry condition
             foreach (var edge in mesh.Edges)
             {
                 if (edge.Label == 1)
                 {
                     int index0 = nodeIndex[edge.P0];
                     int index1 = nodeIndex[edge.P1];

                     double value = MesherNuemann(mesh, index0, index1);
                     bVector[edge.P0] = bVector[edge.P0] + value;
                     bVector[edge.P1] = bVector[edge.P1] + value;
                 }
             }

             // solve for the warpping function
             alglib.sparsesolvesks(globalStiff, nv, true, bVector, out alglib.sparsesolverreport rep, out Omega);*/

            // find warping dependent section properties

            // 1. Sanit Venant Torsional Constant
            //pbp.J = 0;
            //for (int i = 0; i < nv; i++)
            //{
            //    pbp.J += Omega[i] * bVector[i];
            //}
            //pbp.J = pbp.Ixx + pbp.Iyy - pbp.J;

            // Torsional properties
            double Ixw = 0;
            double Iyw = 0;
            double Iw = 0;
            double Qw = 0;
            double Betax = 0;

            foreach (var t in mesh.Triangles)
            {
                MesherTriangle myTriangle = new MesherTriangle(t);
                double a = myTriangle.Area;

                double w0 = meshOmega[t.GetVertex(0).ID];
                double w1 = meshOmega[t.GetVertex(1).ID];
                double w2 = meshOmega[t.GetVertex(2).ID];

                double p0x = t.GetVertex(0).X - pbp.CenteroidX;
                double p0y = t.GetVertex(0).Y - pbp.CenteroidY;
                double p1x = t.GetVertex(1).X - pbp.CenteroidX;
                double p1y = t.GetVertex(1).Y - pbp.CenteroidY;
                double p2x = t.GetVertex(2).X - pbp.CenteroidX;
                double p2y = t.GetVertex(2).Y - pbp.CenteroidY;

                Qw += a * (w0 + w1 + w2) / 3;
                Iw += a * (w0 * w0 + w1 * w1 + w2 * w2 + w0 * w1 + w1 * w2 + w2 * w0) / 6;

                Ixw += a * (p0x * w0 + p1x * w1 + p2x * w2) / 6 + a * (p0x * w1 + p1x * w0 + p0x * w2 + p2x * w0 + p1x * w2 + p2x * w1) / 12;
                Iyw += a * (p0y * w0 + p1y * w1 + p2y * w2) / 6 + a * (p0y * w1 + p1y * w0 + p0y * w2 + p2y * w0 + p1y * w2 + p2y * w1) / 12;

                //pbp.Beta += a * MesherCalculateBeta(p0x, p1x, p2x, p0y, p1y, p2y);
                Betax += MesherCalculateBeta(p0x, p1x, p2x, p0y, p1y, p2y);
            }

            //Ixw = 0;
            pbp.Xs = (+(pbp.IxyC * Ixw) - (pbp.IyyC * Iyw)) / (pbp.IxxC * pbp.IyyC - pbp.IxyC * pbp.IxyC);
            pbp.Ys = (-(pbp.IxyC * Iyw) + (pbp.IxxC * Ixw)) / (pbp.IxxC * pbp.IyyC - pbp.IxyC * pbp.IxyC);
            pbp.Cw = Iw - Math.Pow(Qw, 2) / pbp.Area - pbp.Ys * Ixw + pbp.Xs * Iyw;

            // mds.meshBeta = mds.meshBeta / (mds.meshIxxC)  ;
            pbp.Beta = Betax / (pbp.IxxC) - 2 * (pbp.Ys - pbp.CenteroidY);
            #endregion

            #region Plastic Properties
            // find the centroid of every mesh triangle and store them.

            List<int> xIndex = new List<int>();
            List<int> yIndex = new List<int>();
            List<double> trinagleXc = new List<double>();
            List<double> trinagleYc = new List<double>();

            for (int i = 0; i < nt; i++)
            {
                MesherTriangle myTriangle = new MesherTriangle(mesh.Triangles.ElementAt(i));
                trinagleXc.Add(myTriangle.Xc);
                trinagleYc.Add(myTriangle.Yc);
                xIndex.Add(i);
                yIndex.Add(i);

            }

            pbp.Xp = MesherPlasticNutralAxis(mesh, trinagleXc, xIndex, pbp.Area);
            pbp.Yp = MesherPlasticNutralAxis(mesh, trinagleYc, yIndex, pbp.Area);

            //MesherPlasticNutralAxisNew(mds);

            pbp.Zx = 0;
            pbp.Zy = 0;
            for (int i = 0; i < nt; i++)
            {
                MesherTriangle myTriangle = new MesherTriangle(mesh.Triangles.ElementAt(i));
                pbp.Zx += myTriangle.Area * Math.Abs((myTriangle.Yc - pbp.Yp));
                pbp.Zy += myTriangle.Area * Math.Abs((myTriangle.Xc - pbp.Xp));
            }
            #endregion

            return pbp;

        }

        public double MesherPlasticNutralAxis(TriangleNet.Mesh mesh, List<double> coordinate, List<int> index, double totalMeshArea)
        {
            double value = 0;

            // find the Location of Plastic Nutural Axis 

            int nt = coordinate.Count;

            // sort the lists

            var items = new List<Tuple<double, int>>();
            for (int i = 0; i < nt; i++)
            {
                items.Add(Tuple.Create(coordinate[i], index[i]));
            }

            var sorted = items.OrderBy(x => x.Item1);
            coordinate = sorted.Select(x => x.Item1).ToList();
            index = sorted.Select(x => x.Item2).ToList();

            // find location wher the area halves

            double area = 0;
            for (int i = 0; i < nt; i++)
            {
                MesherTriangle myTriangle = new MesherTriangle(mesh.Triangles.ElementAt(index[i]));
                area += myTriangle.Area;

                if (area >= totalMeshArea / 2)
                {
                    value = (coordinate[i - 1] + coordinate[i]) / 2;
                    break;
                }

            }
            return value;
        }

        public double MesherCalculateBeta(double x1, double x2, double x3, double y1,double y2, double y3)
        {
            double area, xc, yc;

            area = Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2.0);
            xc = (x1 + x2 + x3) / 3.0;
            yc = (y1 + y2 + y3) / 3.0;

            double value = area * (xc * xc + yc * yc) * yc;

            return value;

            /*double value;
            value = 0;

            value += (1.0 / 30.0) * (Math.Pow(x1, 2) * (3 * y1 + y2 + y3));
            value += (1.0 / 30.0) * (Math.Pow(x2, 2) * (y1 + 3 * y2 + y3));
            value += (1.0 / 30.0) * (Math.Pow(x3, 2) * (y1 + y2 + 3 * y3));

            value += (1.0 / 30.0) * (x1 * x2 * (2 * y1 + 2 * y2 + y3));
            value += (1.0 / 30.0) * (x1 * x3 * (2 * y1 + y2 + 2 * y3));
            value += (1.0 / 30.0) * (x2 * x3 * (y1 + 2 * y2 + 2 * y3));

            value += (1.0 / 10.0) * (Math.Pow(y1, 3) + Math.Pow(y2, 3) + Math.Pow(y3, 3));
            value += (1.0 / 10.0) * (Math.Pow(y1, 2) * (y2 + y3));
            value += (1.0 / 10.0) * (Math.Pow(y2, 2) * (y1 + y3));
            value += (1.0 / 10.0) * (Math.Pow(y3, 2) * (y1 + y2));
            value += (1.0 / 10.0) * (y1 * y2 * y3);

            return value;*/
        }


        public double MesherNuemann(TriangleNet.Mesh mesh, int index0, int index1)
        {

            // Create Nuemann boundary condition

            double value;

            // get coordinates the boundary pair node
            double x0 = mesh.Vertices.ElementAt(index0).X;
            double y0 = mesh.Vertices.ElementAt(index0).Y;
            double x1 = mesh.Vertices.ElementAt(index1).X;
            double y1 = mesh.Vertices.ElementAt(index1).Y;

            // find the length
            double dx = x1 - x0;
            double dy = y1 - y0;
            double len = Math.Sqrt(Math.Pow((dx), 2) + Math.Pow((dy), 2));

            // find midpoint
            double xavg = (x1 + x0) / 2;
            double yavg = (y1 + y0) / 2;

            // get the Neumann surface constraint
            value = dy * yavg / len + dx * xavg / len;
            value = (value * len / 2);

            return value;
        }

        public double[,] MesherStiff(TriangleNet.Topology.Triangle t)
        {
            double[,] s = new double[3, 3];
            double b0, b1, b2, c0, c1, c2;

            TriangleNet.Geometry.Point[] p = new TriangleNet.Geometry.Point[3];

            p[0] = t.GetVertex(0);
            p[1] = t.GetVertex(1);
            p[2] = t.GetVertex(2);

            b0 = p[1].Y - p[2].Y;
            b1 = p[2].Y - p[0].Y;
            b2 = p[0].Y - p[1].Y;

            c0 = -p[1].X + p[2].X;
            c1 = -p[2].X + p[0].X;
            c2 = -p[0].X + p[1].X;

            s[0, 0] = b0 * b0 + c0 * c0;
            s[0, 1] = b0 * b1 + c0 * c1;
            s[0, 2] = b0 * b2 + c0 * c2;

            s[1, 0] = s[0, 1];
            s[1, 1] = b1 * b1 + c1 * c1;
            s[1, 2] = b1 * b2 + c1 * c2;

            s[2, 0] = s[0, 2];
            s[2, 1] = s[1, 2];
            s[2, 2] = b2 * b2 + c2 * c2;

            // compute element  Area
            double area;
            area = ((p[0].X * (p[1].Y - p[2].Y)) +
                    (p[1].X * (p[2].Y - p[0].Y)) +
                    (p[2].X * (p[0].Y - p[1].Y))) / 2;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    s[i, j] = s[i, j] / (4 * area);
                }
            }
            return s;
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

    public class FacadePolyBasicProperty
    {
        // information data
        public double Depth = 0;
        public double Width = 0;
        public double Area = 0;
        public double CenteroidX = 0;
        public double CenteroidY = 0;

        public double Ixx = 0;
        public double Iyy = 0;
        public double Ixy = 0;
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
        public double Beta = 0;

        public double Xs = 0;
        public double Ys = 0;

        public double Xp = 0;
        public double Yp = 0;
        public double Zx = 0;
        public double Zy = 0;
    }

    class FacadeMesherTriangle
    {
        TriangleNet.Geometry.Point P0 = new TriangleNet.Geometry.Point();
        TriangleNet.Geometry.Point P1 = new TriangleNet.Geometry.Point();
        TriangleNet.Geometry.Point P2 = new TriangleNet.Geometry.Point();

        public Double Area { get; set; }
        public Double Xc { get; }
        public Double Yc { get; }


        public FacadeMesherTriangle(TriangleNet.Topology.Triangle triangle)
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

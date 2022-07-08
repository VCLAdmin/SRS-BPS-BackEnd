using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BpsUnifiedModelLib;

namespace StructuralSolverSBA
{
    public class ASCEWindLoadInput
    {
        // Input unit convention
        // length - ft; angle - degree; wind speed - mph;
        public double L0 { get; set; }          // Building Length
        public double B0 { get; set; }          // Building Depth
        public double h { get; set; }           // Building Height
        public double theta { get; set; }       // roof angle, default to 0
        public int RC { get; set; }             // Risk Category of building
        public int ET { get; set; }            // Enclosure classification. default to 1 for C&C
        public double V { get; set; }           // Basic wind speed corresponding to Risk Category, /mph
        public double EC { get; set; }          // Exposure Categroy; 1 - B, 2 -C, 3 - D
        public double bw { get; set; }          // Window opening width
        public double hw { get; set; }         // Window opening height
        public double Elvw { get; set; }       // Window opening elevation
        public int WindowZone { get; set; }    // window zone; 4 - center zone, 5 - edge zone
    }

    public class ASCEWindLoadCalculation
    {
        // 1.0 INITIALIZING STRUCTURAL ANALYSIS FROM MODEL INPUT
        public ASCEWindLoadCalculation(ASCEWindLoadInput wlInput)
        {
            _wlInput = wlInput;
            if(_wlInput.ET == 0) _wlInput.ET = 1;   // set ET to default value 1 for C&C
            _errorMsg = "";
            _calcCompleted = false;

            // call api to get V from ZIP
        }

        public WindLoadOutput Calculate()
        {
            double L0 = _wlInput.L0;        // Building Length
            double B0 = _wlInput.B0;        // Building Depth
            double h = _wlInput.h;          // Building Height
            double theta = _wlInput.theta;  // roof angle
            int RC = _wlInput.RC;           // Risk Category of building
            int ET = _wlInput.ET;           // Enclosure classification. default to 1 for C&C
            double V = _wlInput.V;          // mphBasic wind speed corresponding to Risk Category
            double EC = _wlInput.EC;        // Exposure Categroy
            double bw = _wlInput.bw;        // Window opening width
            double hw = _wlInput.hw;        // Window opening height
            double Elvw = _wlInput.Elvw;    // Window opening elevation
            int WindowZone = _wlInput.WindowZone; // Window Zone

            // Step 2 Determine basic wind speed V
            Elvw = h < 60 ? h : Elvw;

            // Step 3 Determine wind load parameters
            double Kd = 0.85;               // default to 0.85 for window
            double Kzt = 1.0;               // default to 1.0

            double GCpi = 0.0;
            switch (ET)
            {
                case 1:
                    GCpi = 0.18;
                    break;
                case 2:
                    GCpi = 0.55;
                    break;
                case 3:
                    GCpi = 0.0;
                    break;
                default:
                    _errorMsg = "ET number must be 1, 2 or 3";
                    return wlOutput;
            }

            // Step 4 Velocity pressure exposure coefficient
            double Alpha = 0.0;
            switch (EC)
            {
                case 1:
                    Alpha = 7.0;
                    break;
                case 2:
                    Alpha = 9.5;
                    break;
                case 3:
                    Alpha = 11.5;
                    break;
                default:
                    _errorMsg = "EC number must be 1, 2 or 3";
                    return wlOutput;
            }

            double Zg = 0.0;
            switch (EC)
            {
                case 1:
                    Zg = 1200;
                    break;
                case 2:
                    Zg = 900;
                    break;
                case 3:
                    Zg = 700;
                    break;
                default:
                    _errorMsg = "EC number must be 1, 2 or 3";
                    return wlOutput;
            }

            // Step 5 Velocity pressure
            double Kz = CalcKz(Elvw, Zg, Alpha);
            double qww = 0.00256 * Kz * Kzt * Kd * V * V;
            double Kh = CalcKz(h, Zg, Alpha);
            double qh = 0.00256 * Kh * Kzt * Kd * V * V;

            // Step 6 External pressure coefficient GCp
            // Part 1 (Envelope procedure) when h < 60
            double A = bw * hw;
            if (h < 60)
            {
                double GCp1_pl = 0.0;
                if (A <= 10)
                {
                    GCp1_pl = 1.0;
                }
                else if (A >= 500)
                {
                    GCp1_pl = 0.7;
                }
                else
                {
                    GCp1_pl = 1 - 0.3 * Math.Log10(A / 10) / Math.Log10(50);
                }
                GCp1_pl = theta < 10 ? (0.9 * GCp1_pl) : GCp1_pl;

                double GCp1_ng4 = 0.0;
                if (A <= 10)
                {
                    GCp1_ng4 = -1.1;
                }
                else if (A >= 500)
                {
                    GCp1_ng4 = -0.8;
                }
                else
                {
                    GCp1_ng4 = -1.1 + 0.3 * Math.Log10(A / 10) / Math.Log10(50);
                }
                GCp1_ng4 = theta < 10 ? (0.9 * GCp1_ng4) : GCp1_ng4;

                double GCp1_ng5 = 0.0;
                if (A <= 10)
                {
                    GCp1_ng5 = -1.4;
                }
                else if (A >= 500)
                {
                    GCp1_ng5 = -0.8;
                }
                else
                {
                    GCp1_ng5 = -1.4 + 0.6 * Math.Log10(A / 10) / Math.Log10(50);
                }
                GCp1_ng5 = theta < 10 ? (0.9 * GCp1_ng5) : GCp1_ng5;

                double P1_pl = Math.Max(qh * (GCp1_pl - GCpi), qh * (GCp1_pl + GCpi));
                double P1_ng4 = Math.Min(qh * (GCp1_ng4 - GCpi), qh * (GCp1_ng4 + GCpi));
                double P1_ng5 = Math.Min(qh * (GCp1_ng5 - GCpi), qh * (GCp1_ng5 + GCpi));

                P_pl = P1_pl;
                switch (WindowZone)
                {
                    case 4:
                        P_ng = P1_ng4;
                        break;
                    case 5:
                        P_ng = P1_ng5;
                        break;
                    default:
                        _errorMsg = "WindowZone number must be 4 or 5 for ASCE code check";
                        return wlOutput;
                }
            }
            else if (h >= 60)
            {
                // Part 3 (Directional procedure) when h > 60
                double GCp3_pl = 0.0;
                if (A <= 20)
                {
                    GCp3_pl = 0.9;
                }
                else if (A >= 500)
                {
                    GCp3_pl = 0.6;
                }
                else
                {
                    GCp3_pl = 0.9 - 0.3 * Math.Log10(A / 20) / Math.Log10(25);
                }

                double GCp3_ng4 = 0.0;
                if (A <= 20)
                {
                    GCp3_ng4 = -0.9;
                }
                else if (A >= 500)
                {
                    GCp3_ng4 = -0.7;
                }
                else
                {
                    GCp3_ng4 = -0.9 + 0.2 * Math.Log10(A / 20) / Math.Log10(25);
                }

                double GCp3_ng5 = 0.0;
                if (A <= 20)
                {
                    GCp3_ng5 = -1.8;
                }
                else if (A >= 500)
                {
                    GCp3_ng5 = -1.0;
                }
                else
                {
                    GCp3_ng5 = -1.8 + 0.8 * Math.Log10(A / 20) / Math.Log10(25);
                }

                double P3_pl_windward = Math.Max((qww * GCp3_pl - qh*GCpi), (qww * GCp3_pl + qh * GCpi));
                double P3_pl_leeward = Math.Max((qh * GCp3_pl - qh * GCpi), (qh * GCp3_pl + qh * GCpi));
                double P3_ng4_windward = Math.Min((qww * GCp3_ng4 - qh * GCpi), (qww * GCp3_ng4 + qh * GCpi));
                double P3_ng5_windward = Math.Min((qww * GCp3_ng5 - qh * GCpi), (qww * GCp3_ng5 + qh * GCpi));
                double P3_ng4_leeward = Math.Min((qh * GCp3_ng4 - qh * GCpi), (qh * GCp3_ng4 + qh * GCpi));
                double P3_ng5_leeward = Math.Min((qh * GCp3_ng5 - qh * GCpi), (qh * GCp3_ng5 + qh * GCpi));

                P_pl = Math.Max(P3_pl_windward, P3_pl_leeward);
                switch (WindowZone)
                {
                    case 4:
                        P_ng = Math.Min(P3_ng4_windward, P3_ng4_leeward);
                        break;
                    case 5:
                        P_ng = Math.Min(P3_ng5_windward, P3_ng5_leeward);
                        break;
                    default:
                        _errorMsg = "WindowZone number must be 4 or 5";
                        return wlOutput;
                }
            }
            P_maxAbs = Math.Max(P_pl, -P_ng);
            _calcCompleted = true;

            wlOutput = new WindLoadOutput
            {
                MaxPositiveWindPressure = P_pl,
                MinNegtiveWindPressure = P_ng,
                MaxAbsWindPressure = P_maxAbs,
                WLCString = OutputWLCFile(),
            };
            return wlOutput;
        }

        // 10.0 DEBUG: CREATE SA OUTPUT FILES
        public string OutputWLCFile()
        {
            // 10.5 Begin writing Output file
            StringBuilder sb = new StringBuilder();
            string strJson = JsonConvert.SerializeObject(_wlInput, Formatting.Indented);
            string strWLCOutput = "";
            using (StringWriter sw = new StringWriter(sb))
            {
                sw.WriteLine("****************************************************************************************");
                sw.WriteLine("This output file is generated on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                sw.WriteLine("****************************************************************************************");
                sw.WriteLine("ASCE Wind Load Calculation Input Echo:");
                sw.Write(strJson);
                sw.WriteLine("\n****************************************************************************************");
                if (_calcCompleted)
                {
                    sw.Write(strWLCOutput);
                    sw.WriteLine("The calculation is completed successfully on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    sw.WriteLine("The Wind Load Calculation is based on ASCE7-10 Chapter 26 & 30");
                    sw.WriteLine("Max. Positive Wind Load: " + string.Format("{0:#.######}", P_pl));
                    sw.WriteLine("Min. Negtive Wind Load: " + string.Format("{0:#.######}", P_ng));
                    sw.WriteLine("Calculated Max. Absolute Value of Wind Load: " + string.Format("{0:#.######}", P_maxAbs));
                }
                else
                {
                    sw.WriteLine("The calculation exit with error on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    sw.WriteLine("Solver Error Message:");
                    sw.WriteLine(_errorMsg);
                }
            }
            return sb.ToString();
        }


        private double CalcKz(double Z, double Zg, double Alpha)
        {
            double Kz = 0.0;
            if (Z < Zg && Z > 15)
            {
                Kz = 2.01 * Math.Pow((Z / Zg), 2.0 / Alpha);
            }
            else if (Z < 15)
            {
                Kz = 2.01 * Math.Pow((15 / Zg), 2.0 / Alpha);
            }
            return Kz;
        }

        private ASCEWindLoadInput _wlInput;
        private string _errorMsg;
        private bool _calcCompleted;
        private double P_pl, P_ng, P_maxAbs;
        public WindLoadOutput wlOutput;
    }

    // DIN EN 1991-1-4:2010-12  and DIN EN 1991-1-4/NA:2010-12
    //public class DINWindLoadInput
    //{
    //    // Input unit convention
    //    // length - ft; angle - degree; wind speed - mph;
    //    public double L0 { get; set; }          // Building Width (length of the wall where window is at)
    //    public double B0 { get; set; }          // Building Depth
    //    public double h { get; set; }           // Building Height
    //    public int WindZone { get; set; }             // Wind Zone (NA.A.1)
    //    public int TerrainCategory { get; set; }      // Terrain Category (NA.B.1)
    //    public double bw { get; set; }          // Window opening width
    //    public double hw { get; set; }         // Window opening height
    //    public double Elvw { get; set; }       // Window opening elevation
    //    public int WindowZone { get; set; }    // window zone; 1 - edge zone, 2 - center zone (EN figure7.5)
    //}

    public class DINWindLoadCalculation
    {
        // 1.0 INITIALIZING STRUCTURAL ANALYSIS FROM MODEL INPUT
        public DINWindLoadCalculation(DinWindLoadInput wlInput)
        {
            _wlInput = wlInput;
            _errorMsg = "";
            _calcCompleted = false;

            // get WindZone from ZIP

            // get WindZone from ZIP ends here
        }

        public WindLoadOutput Calculate()
        {
            double b0 = _wlInput.L0;                // Building Width (length of the wall perpendicular to window)
            double d0 = _wlInput.B0;                // Building Depth (length of the wall where window is at)
            double h = _wlInput.h;                  // Building Height
            double WZ = _wlInput.WindZone;          // Wind Zone (NA.A.1)
            int TC = _wlInput.TerrainCategory;      // Terrain Category (1~4 from NA.B.1, 5 - inland regions, mixed profile of TC II and III, 6 - coastal area, mixed profile of TC I and II)
            double ElvW = _wlInput.ElvW;            // Window opening elevation
            int WindowZone = _wlInput.WindowZone;   // Window Zone; 1 - center zone, 2 - edge zone
            bool includeCpi = _wlInput.IncludeCpi;  // Whether to include Cpi in Cp calculation

            // Section 4.2 Basic Value
            // NA.A.1
            double vb0;
            switch (WZ)
            {
                case 1:
                    vb0 = 22.5;     
                    break;
                case 2:
                    vb0 = 25;
                    break;
                case 3:
                    vb0 = 27.5;
                    break;
                case 4:
                    vb0 = 30;
                    break;
                default:
                    _errorMsg = "Wind Zone must be 1, 2, 3 or 4";
                    return wlOutput;
            }

            double c_dir = 1.0;                        // DNP. re 4.2(2)P Note2
            double c_season = 1.0;                     // DNP. re 4.2(2)P Note3
            double vb = c_dir * c_season * vb0;        // EN1-1-4 Expression 4.10

            // windload = Cpe1 * q_p
            Aw = 1;  // calculate Cpe1 using Aw=1;
            // NEGTIVE PRESSURE
            double b = b0;      //b length of the wall perpendicular to window
            double d = d0;      //d length of the wall where the window is at
            double c0 = 1.0;                            // EN1-1-4 A.3
            q_p = Calculateq_p(TC, h, c0, vb);  // Section 4.3 ~ Section 4.5 Peak velocity pressure, // Section 7.2.2, for zone A,B,C and E, Ze = h
            double a_r = h / d;
            // zone A, Aw=1
            double Cpe = CalculateCpe(1, Aw, a_r);  // Section 7.2.2NA Table 7.1 External Pressure Coefficient
            double Cpi = 0;
            if (_wlInput.IncludeCpi)
            {
                Cpi = Cpe > 0 ? _wlInput.nCpi : _wlInput.pCpi;  //default to  -0.3 : 0.2
            }
            double Cp = Cpe - Cpi;
            P_ng_A = Cp * q_p;
            // zone B, Aw=1
            Cpe = CalculateCpe(2, Aw, a_r);  // Section 7.2.2NA Table 7.1 External Pressure Coefficient
            Cpi = 0;
            if (_wlInput.IncludeCpi)
            {
                Cpi = Cpe > 0 ? _wlInput.nCpi : _wlInput.pCpi;  //default to  -0.3 : 0.2
            }
            Cp = Cpe - Cpi;
            P_ng_B = Cp * q_p;
            // zone user defined, Aw=1
            Cpe = CalculateCpe(WindowZone, Aw, a_r);  // Section 7.2.2 Table 7.1 External Pressure Coefficient
            Cpi = 0;
            if (_wlInput.IncludeCpi)
            {
                Cpi = Cpe > 0 ? _wlInput.nCpi : _wlInput.pCpi;  //default to  -0.3 : 0.2
            }
            Cp = Cpe - Cpi;
            P_ng = Math.Abs(Cp * q_p);

            // POSTIVE PRESSURE
            b = d0;     //b length of the wall where the window is at
            d = b0;     //d length of the wall that is perpendicular to window 
            //double ze = CalculateZe(h, b, ElvW);           // Reference heights for windward
            //c0 = 1.0;                               // EN1-1-4 A.3
            //q_p = Calculateq_p(TC, ze, c0, vb);     // Section 4.3 ~ Section 4.5 Peak velocity pressure
            a_r = h / d;
            Cpe = CalculateCpe(4, Aw, a_r);         // Section 7.2.2 Table 7.1 External Pressure Coefficient
            Cpi = 0;
            if (_wlInput.IncludeCpi)
            {
                Cpi = Cpe > 0 ? _wlInput.nCpi : _wlInput.pCpi;  //default to  -0.3 : 0.2
            }
            Cp = Cpe - Cpi;                        // add internal pressure
            P_pl = Math.Abs(Cp * q_p);

            // windload = q_p
            // NEGTIVE PRESSURE
            //double b = d0;
            //double d = b0;
            //double c0 = 1.0;                            // EN1-1-4 A.3
            //q_p = Calculateq_p(TC, h, c0, vb);          // Section 4.3 ~ Section 4.5 Peak velocity pressure, // Section 7.2.2, for zone A,B,C and E, Ze = h
            //P_ng = q_p;

            // POSTIVE PRESSURE
            //b = b0;
            //d = d0;
            //double ze = CalculateZe(h, b, ElvW);           // Reference heights for windward
            //c0 = 1.0;                               // EN1-1-4 A.3
            //q_p = Calculateq_p(TC, ze, c0, vb);     // Section 4.3 ~ Section 4.5 Peak velocity pressure
            //P_pl = q_p;

            P_maxAbs = Math.Max(P_pl, P_ng);

            _calcCompleted = true;

            wlOutput = new WindLoadOutput
            {
                q_p = q_p / 1000,
                MaxPositiveWindPressure = P_pl / 1000, // convert from Pa to kPa
                MinNegtiveWindPressure = P_ng / 1000,
                MaxAbsWindPressure = P_maxAbs / 1000,
                WLCString = OutputWLCFile(),
            };
            return wlOutput;
        }

        // 10.0 DEBUG: CREATE SA OUTPUT FILES
        public string OutputWLCFile()
        {
            // 10.5 Begin writing Output file
            StringBuilder sb = new StringBuilder();
            string strJson = JsonConvert.SerializeObject(_wlInput, Formatting.Indented);
            string strWLCOutput = "";
            using (StringWriter sw = new StringWriter(sb))
            {
                sw.WriteLine("****************************************************************************************");
                sw.WriteLine("This output file is generated on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                sw.WriteLine("****************************************************************************************");
                sw.WriteLine("DIN EN 1991-1-4/NA Wind Load Calculation Input Echo:");
                sw.Write(strJson);
                sw.WriteLine("\n\n****************************************************************************************");
                if (_calcCompleted)
                {
                    sw.Write(strWLCOutput);
                    sw.WriteLine("The calculation is completed successfully on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    sw.WriteLine("The Wind Load Calculation is based on DIN EN 1991-1-4 & NA (Germany)");
                    sw.WriteLine("For Structural Member Tributary Area Aw <= 1 m2");
                    sw.WriteLine("Peak velocity pressure q_p        " + string.Format("{0:0.###}", q_p/1000) + " kPa");
                    sw.WriteLine("Wind Load - windward              Windzone 4");
                    sw.WriteLine("                                  " + string.Format("{0:0.###}", P_pl / 1000) + " kPa ");
                    sw.WriteLine("Wind Load - wind from side        Windzone 1 (Edge Zone)      Windzone 2 (Center Zone)");
                    sw.WriteLine("                                  " + string.Format("{0:0.###}", P_ng_A / 1000) + " kPa                  " + string.Format("{0:0.###}", P_ng_B / 1000) + " kPa");
                    sw.WriteLine("\n****************************************************************************************");
                    string Windowzone = _wlInput.WindowZone == 2 ? "Windowzone 2 (Center Zone)" : "Windowzone 1 (Edge Zone)";
                    sw.WriteLine("The Wind Load Calculation Summary For "+ Windowzone);
                    sw.WriteLine("Max. Positive Wind Load: " + string.Format("{0:0.###}", P_pl / 1000) + " kPa");
                    sw.WriteLine("Min. Negtive Wind Load: " + string.Format("{0:0.###}", P_ng / 1000) + " kPa");
                    sw.WriteLine("Calculated Max. Absolute Value of Wind Load: " + string.Format("{0:0.###}", P_maxAbs / 1000) + " kPa");
                }
                else
                {
                    sw.WriteLine("The calculation exit with error on " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    sw.WriteLine("Solver Error Message:");
                    sw.WriteLine(_errorMsg);
                }
            }

            return sb.ToString();
        }

        private double CalculateZe(double h, double b, double Elvw)
        {
            double ze;
            double h_strip = (h - 2 * b) / 4;
            if (h <= b)
            {
                ze = h;
            }
            else if (h <= 2 * b)
            {
                ze = Elvw < b ? b : h;
            }
            else
            {
                if (Elvw <= b)
                { ze = b; }
                else if (Elvw >= h - b)
                { ze = h; }
                else
                { ze = b + h_strip * Math.Ceiling((Elvw - b) / h_strip); }
            }
            return ze;
        }

        private double Calculateq_p(int TC, double z, double c0, double vb)
        {
            double z_min = Math.Pow(2, TC);            // Table NA.B.2
            double rho = 1.25;                         // NDP re 4.5(1), Note 2
            double q_p = 0.0;
            if (z > 50 && TC >1 && TC <5) TC = TC - 1;          // NA.B.2 (2)
            // Method 1:  Calculate q_p using Table NA.B.2 / NA.B.4 and Eq. NA.B.11
            // vmf and Ivf from Table NA.B.2 and Table NA.B.4
            // 5-inland, 6-coastal
            // double vmf, Ivf, vm, Iv;
            //switch (TC)
            //{
            //    case 1:
            //        vmf = z >= z_min ? 1.18 * vb * Math.Pow((z / 10), 0.12) : 0.97 * vb;
            //        Ivf = z >= z_min ? 0.14 * Math.Pow((z / 10), -0.12) : 0.17;
            //        break;
            //    case 2:
            //        vmf = z >= z_min ? 1.0 * vb * Math.Pow((z / 10), 0.16) : 0.86 * vb;
            //        Ivf = z >= z_min ? 0.19 * Math.Pow((z / 10), -0.16) : 0.22;
            //        break;
            //    case 3:
            //        vmf = z >= z_min ? 0.77 * vb * Math.Pow((z / 10), 0.22) : 0.73 * vb;
            //        Ivf = z >= z_min ? 0.28 * Math.Pow((z / 10), -0.22) : 0.29;
            //        break;
            //    case 4:
            //        vmf = z >= z_min ? 0.56 * vb * Math.Pow((z / 10), 0.3) : 0.64 * vb;
            //        Ivf = z >= z_min ? 0.43 * Math.Pow((z / 10), -0.3) : 0.37;
            //        break;
            //    case 5:
            //        z_min = 7;
            //        if (z>=50)
            //        {
            //            vmf = 1.0 * vb * Math.Pow((z / 10), 0.16);
            //            Ivf = 0.19 * Math.Pow((z / 10), -0.16) ;
            //        }
            //        else if (z >= z_min)
            //        {
            //            vmf = 0.86 * vb * Math.Pow((z / 10), 0.25);
            //            Ivf = 0.22 * Math.Pow((z / 10), -0.25);
            //        }
            //        else
            //        {
            //            vmf = 0.79 * vb;
            //            Ivf = 0.24;
            //        }
            //        break;
            //    case 6:
            //        z_min = 4;
            //        if (z >= 50)
            //        {
            //            vmf = 1.18 * vb * Math.Pow((z / 10), 0.12);
            //            Ivf = 0.14 * Math.Pow((z / 10), -0.12);
            //        }
            //        else if (z >= z_min)
            //        {
            //            vmf = 1.10 * vb * Math.Pow((z / 10), 0.165);
            //            Ivf = 0.15 * Math.Pow((z / 10), -0.165);
            //        }
            //        else
            //        {
            //            vmf = 0.95 * vb;
            //            Ivf = 0.17;
            //        }
            //        break;
            //    default:
            //        _errorMsg = "Terrain Category must be 1, 2, 3, 4, 5 or 6";
            //        return q_p;
            //}
            //vm = c0 * vmf;
            //Iv = Ivf / c0;
            //q_p = 0.5 * rho * vm * vm * (1 + 6 * Iv / c0);

            // Method 2:  Calculate q_p using Table NA.B.2 and Eq. NA.B.1~8
            // TC: 5-inland, 6-coastal, 7-island in the North Sea
            double q_b = rho*vb*vb/2;    
            switch (TC)
            {
                case 1:
                    q_p = z >= z_min ? 2.6 * q_b * Math.Pow((z / 10), 0.19) : 1.9 * q_b;
                    break;
                case 2:
                    q_p = z >= z_min ? 2.1 * q_b * Math.Pow((z / 10), 0.24) : 1.7 * q_b;
                    break;
                case 3:
                    q_p = z >= z_min ? 1.6 * q_b * Math.Pow((z / 10), 0.31) : 1.5 * q_b;
                    break;
                case 4:
                    q_p = z >= z_min ? 1.1 * q_b * Math.Pow((z / 10), 0.4) : 1.3 * q_b;
                    break;
                case 5:
                    z_min = 7;
                    if (z <= z_min)
                    {
                        q_p = 1.5 * q_b;
                    }
                    else if (z <= 50)
                    {
                        q_p = 1.7 * q_b * Math.Pow((z / 10), 0.37);
                    }
                    else
                    {
                        q_p = 2.1 * q_b * Math.Pow((z / 10), 0.24);
                    }
                    break;
                case 6:
                    z_min = 4;
                    if (z <= z_min)
                    {
                        q_p = 1.8 * q_b;
                    }
                    else if (z <= 50)
                    {
                        q_p = 2.3 * q_b * Math.Pow((z / 10), 0.27);
                    }
                    else
                    {
                        q_p = 2.6 * q_b * Math.Pow((z / 10), 0.19);
                    }
                    break;
                case 7:
                    z_min = 2.0;
                    if (z <= z_min)
                    {
                        q_p = 1100;
                    }
                    else
                    {
                        q_p = 1500 * Math.Pow((z / 10), 0.19);
                    }
                    break;
                default:
                    _errorMsg = "Terrain Category must be 1, 2, 3, 4, 5, 6 or 7";
                    return q_p;
            }
            return q_p;
        }

        private double CalculateCpe(int WindowZone, double Aw, double a_r)
        {
            // section 7.2.2
            double Cpe = 0.0, Cpe1, Cpe10;
            switch (WindowZone)
            {
                case 1:
                    if (a_r <= 1)
                    {
                        Cpe1 = -1.4;
                        Cpe10 = -1.2;
                    }
                    else if (a_r >= 5)
                    {
                        Cpe1 = -1.7;
                        Cpe10 = -1.4;
                    }
                    else
                    {
                        Cpe1 = -1.4 - 0.3 * (a_r - 1) / 4;
                        Cpe10 = -1.2 - 0.2 * (a_r - 1) / 4;
                    }
                    break;
                case 2:
                    Cpe1 = -1.1;
                    Cpe10 = -0.8;
                    break;
                case 4:
                    Cpe1 = 1.0;
                    if (a_r <= 0.25)
                    {
                        Cpe10 = 0.7;

                    }
                    else if (a_r >= 1)
                    {
                        Cpe10 = 0.8;
                    }
                    else
                    {
                        Cpe10 = 0.7 + 0.1 * (a_r - 0.25) / 0.75;
                    }
                    break;
                default:
                    _errorMsg = "Window Zone must be 1, 2 or 4";
                    return Cpe;
            }
            if (Aw <= 1)
            {
                Cpe = Cpe1;
            }
            else if (Aw >= 10)
            {
                Cpe = Cpe10;
            }
            else
            {
                Cpe = Cpe1 - (Cpe1 - Cpe10) * Math.Log10(Aw);
            }
            return Cpe;
        }

        private DinWindLoadInput _wlInput;
        private string _errorMsg;
        private bool _calcCompleted;
        private double P_ng_A, P_ng_B, Aw, q_p;
        private double P_pl, P_ng, P_maxAbs;
        public WindLoadOutput wlOutput;
    }


    public class WindLoadOutput
    {
        public double q_p;
        public double MaxPositiveWindPressure;
        public double MinNegtiveWindPressure;
        public double MaxAbsWindPressure;
        public string WLCString;
    }

    public class WindZoneOutput
    {
        public int WindZone;
        public string State;
        public string District;
        public string Place;
        public double vb0;
        public double qb0;
    }
}

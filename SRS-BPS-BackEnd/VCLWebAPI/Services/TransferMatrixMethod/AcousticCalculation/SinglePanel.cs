using System;
using VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation
{
    public static class SinglePanel
    {
        private const double C = 340.0;

        public static double GetCriticalFreq(Plate plate)
        {
            double B = (plate.E * Math.Pow(plate.H, 3.0)) / (12.0 * (1.0 - Math.Pow(plate.V, 2.0)));
            double m = SinglePanel.ComputeSurfaceMass(plate);
            double f = Math.Pow(C, 2.0) * Math.Sqrt(m / B) / (2.0 * Math.PI);
            return f;
        }

        public static Double GetLowestResonantFrequency(Plate plate)
        {
            int mm = 3, nn = 3;

            double term1 = plate.Rho * (1 - Math.Pow(plate.V, 2.0));
            double cL = Math.Pow(plate.E / term1, 0.5);
            double term2 = Math.Pow((mm / plate.Lx), 2.0) + Math.Pow((nn / plate.Ly), 2.0);
            double fL = Math.PI * cL * plate.H * term2 / (4 * Math.Pow(3, 0.5));
            return fL;
        }

        public static double ComputeSurfaceMass(Plate plate)
        {
            double m;
            if (plate.Material == Plate.MaterialT.glass)
            {
                m = plate.Rho * plate.H;
            }
            else
            {
                m = plate.Rho * plate.H + plate.InterRho * plate.InterH;
            }
            return m;
        }

        public static Double ComputeTauL1(double f, Plate plate, Room room)
        {
            double k = 2.0 * Math.PI * f / C;
            double alphaZero = Math.PI * f * (ComputeSurfaceMass(plate)) / (room.Rho * C);

            double COS;
            double TauL1;
            if (1.0 / (k * Math.Pow(plate.Lx * plate.Ly, 0.5)) > 0.9)
            {
                COS = 0.9;
            }
            else
            {
                COS = 1.0 / (k * Math.Pow(plate.Lx * plate.Ly, 0.5));
            }

            double term1 = 1 + Math.Pow(alphaZero, 2.0);
            double term2 = 1 + Math.Pow(alphaZero, 2.0) * COS;
            TauL1 = (1 / Math.Pow(alphaZero, 2.0)) * Math.Log(term1 / term2);

            return TauL1;
        }

        public static double ComputeSinglePanelG(Plate plate, double f)
        {
            double g;
            if (f >= GetCriticalFreq(plate))
            {
                g = Math.Sqrt(1.0 - (GetCriticalFreq(plate) / f));
            }
            else { g = 0; }

            return g;
        }

        public static double ComputeSinglePanelH(Plate plate, double f)
        {
            double beta = 0.124;
            double k = 2.0 * Math.PI * f / C;
            double aeff = plate.Lx * plate.Ly / (plate.Lx + plate.Ly);
            double h = 1.0 / ((2.0 / 3.0) * Math.Sqrt(2.0 * k * aeff / Math.PI) - beta);

            return h;
        }

        public static double ComputeSinglePanelP(Plate plate, double f)
        {
            double w = 1.3;
            double k = 2.0 * Math.PI * f / C;
            double aeff = plate.Lx * plate.Ly / (plate.Lx + plate.Ly);
            double p;
            if (w * Math.Sqrt(Math.PI / (2.0 * k * aeff)) <= 1)
            {
                p = w * Math.Sqrt(Math.PI / (2.0 * k * aeff));
            }
            else { p = 1.0; }
            return p;
        }

        public static double ComputeSinglePanelQ(Plate plate, double f)
        {
            double k = 2.0 * Math.PI * f / C;
            double q = 2.0 * Math.PI / (Math.Pow(k, 2.0) * (plate.Lx * plate.Ly));

            return q;
        }

        public static double ComputeSinglePanelSigmaZero(Plate plate, double f)
        {
            double n = 2.0;
            double p = ComputeSinglePanelP(plate, f);
            double q = ComputeSinglePanelQ(plate, f);
            double h = ComputeSinglePanelH(plate, f);
            double g = ComputeSinglePanelG(plate, f);

            double alpha = h / p - 1.0;

            double sigmaZero = 0.0;
            {
                if (g <= 1 && g >= p)
                {
                    double term1 = Math.Pow((Math.Pow(g, n) + Math.Pow(q, n)), 1 / n);
                    sigmaZero = 1 / term1;
                }
                else if (p > g && g >= 0)
                {
                    double term2 = Math.Pow((Math.Pow((h - alpha * g), n) + Math.Pow(q, n)), 1 / n);
                    sigmaZero = 1 / term2;
                }
            }
            return sigmaZero;
        }

        public static double ComputeSinglePanelSigmaOne(Plate plate, double f)
        {
            double sigmaZero = ComputeSinglePanelSigmaZero(plate, f);
            double fc = GetCriticalFreq(plate);
            double sigmaZero09 = ComputeSinglePanelSigmaZero(plate, 0.9 * fc);
            double sigmaC = -0.65 * plate.InterH * 1000 + 2.25;
            double sigmaOne;
            {
                if (f >= fc)
                {
                    sigmaOne = Math.Min(sigmaC, sigmaZero);
                }
                else if (f <= 0.9 * fc)
                {
                    sigmaOne = sigmaZero;
                }
                else
                {
                    sigmaOne = sigmaZero09 + (Math.Min(sigmaC, sigmaZero) - sigmaZero09) * (f - 0.9 * fc) / (0.1 * fc);
                }
            }
            return sigmaOne;
        }

        public static Double SolveSinglePanelSTC(Plate plate, Room room, double f)
        {
            double p = ComputeSinglePanelP(plate, f);
            double q = ComputeSinglePanelQ(plate, f);
            double h = ComputeSinglePanelH(plate, f);

            double sigmaZero; // !!! legacy code, sigmaZero represents sigmaOne for lamimnated single panel
            // Non-laminated Single Panel
            if (plate.Material == Plate.MaterialT.glass)
            {
                sigmaZero = ComputeSinglePanelSigmaZero(plate, f);
            }
            // laminated Single Panel, TODO: differenciate PVB, SGP, SC
            else
            {
                sigmaZero = ComputeSinglePanelSigmaOne(plate, f);
            }

            double alpha = h / p - 1.0;

            double etaD = (plate.Rho * plate.H) / (485.0 * Math.Pow(f, 0.5)) + plate.Eta; // laminated ??
            double alphaZero = (Math.PI * f * ComputeSurfaceMass(plate)) / (room.Rho * C);
            double fL = GetLowestResonantFrequency(plate);
            double fc = GetCriticalFreq(plate);

            double betaZero = 2.0 * alphaZero * (f / fc) * (sigmaZero + alphaZero * etaD);
            double term3 = Math.Atan((2.0 * alphaZero) / (sigmaZero + alphaZero * etaD));
            double term4 = Math.Atan(2.0 * alphaZero * (1 - (f / fc)) / (sigmaZero + alphaZero * etaD));
            double taudH = (Math.Pow(sigmaZero, 2.0) / betaZero) * (term3 - term4);

            double term5 = Math.Log((1 + Math.Pow(1 + Math.Pow(q, 2.0), 0.5)) / (p + Math.Pow(Math.Pow(p, 2.0) + Math.Pow(q, 2.0), 0.5)));
            double term6 = (h + Math.Pow(Math.Pow(h, 2.0) + Math.Pow(q, 2.0), 0.5)) / (p + Math.Pow(Math.Pow(p, 2.0) + Math.Pow(q, 2.0), 0.5));
            double sumF = term5 + Math.Log(term6) / alpha;
            double taudL = 2.0 * sumF / Math.Pow(alphaZero, 2.0);

            double tauL1 = ComputeTauL1(f, plate, room);

            double tauM = taudH + taudL;

            double term7 = (tauM - ComputeTauL1((2.0 / 3.0) * fL, plate, room)) / ((1.0 / 3.0) * fL);
            double tauL2 = ComputeTauL1((2.0 / 3.0) * fL, plate, room) + term7 * (f - (2.0 / 3.0) * fL);

            double tau;

            {
                if (f <= (2.0 / 3.0) * fL)
                {
                    tau = ComputeTauL1(f, plate, room);
                }
                else if (f <= fc && f >= fL)
                {
                    tau = tauM;
                }
                else if (f < fL && f > (2.0 / 3.0) * f)
                {
                    tau = tauL2;
                }
                else
                {
                    tau = taudH;
                }

                return tau;
            }
        }
    }
}
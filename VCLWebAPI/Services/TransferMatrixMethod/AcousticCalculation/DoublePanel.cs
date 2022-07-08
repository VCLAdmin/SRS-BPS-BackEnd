using MathNet.Numerics.Integration;
using System;
using System.Numerics;
using VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation
{
    public static class DoublePanel
    {
        private const double c = 340.0;
        private const double betaBrun = 0.956;
        private const int INTEGRATE_ORDER = 1024;

        public static double ComputeSigmaf(Plate plate, double theta, double f)
        {
            Complex k = 2.0 * Math.PI * f / c;
            Complex aeff = 2.0 * plate.Lx * plate.Ly / (2.0 * plate.Lx + 2.0 * plate.Ly);

            Complex term1 = Math.Pow(Math.Cos(theta), 2.0) - (4.0 * Complex.ImaginaryOne * betaBrun * Math.Sin(theta)) / (k * aeff);
            Complex term2 = 2.0 * betaBrun / (k * aeff);
            Complex Zfbar = 1.0 / (Complex.Sqrt(term1) + Complex.Pow(term2, 2.0));
            double sigmaf = Zfbar.Real;
            return sigmaf;
        }

        public static double ComputeDoublePanelDamingLossFactorB(Plate plate, Room room, double f)
        {
            double m = SinglePanel.ComputeSurfaceMass(plate);
            double eta = m / (485 * Math.Sqrt(f)) + plate.Eta;
            return eta;
        }

        public static double ComputeDoublePanelResonantFrequency(Plate plate1, Plate plate2, Room room)
        {
            double m1 = SinglePanel.ComputeSurfaceMass(plate1);
            double m2 = SinglePanel.ComputeSurfaceMass(plate2);
            double term1 = 1.0 / (m1) + 1.0 / (m2);
            double f = c * Math.Sqrt(room.Rho * term1 / room.Lz) / (2.0 * Math.PI);
            return f;
        }

        public static double ComputeDoublePanelStudBorneSTC(FacadeComponent component, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;
            double k = 2.0 * Math.PI * f / c;
            double omega = 2.0 * Math.PI * f;
            double m1 = SinglePanel.ComputeSurfaceMass(plates[0]);
            double m2 = SinglePanel.ComputeSurfaceMass(plates[1]);

            double g = m1 * Math.Sqrt(2.0 * Math.PI * SinglePanel.GetCriticalFreq(plates[1])) + m2 * Math.Sqrt(2.0 * Math.PI * SinglePanel.GetCriticalFreq(plates[0]));
            double D = 2.0;
            double b0 = plates[0].Ly / 3.0;
            double cM = 5.1 * 1e-8; //todo
            double K2010 = 1.5;
            double term1 = 1.0 - 4.0 * Math.Pow(omega, 1.5) * m1 * m2 * c * cM / g;
            double J1 = 2.0 / (1.0 + Math.Pow(term1, 2.0));
            double J2;
            double J;
            {
                if (J1 <= K2010)
                {
                    J2 = K2010;
                }
                else
                {
                    J2 = J1;
                }
            }
            if (plates[0].H == plates[1].H)
            {
                J = J1;
            }
            else
            {
                J = J2;
            }

            double tauSD = 32.0 * Math.Pow(rooms[0].Rho, 2.0) * Math.Pow(c, 3.0) * D * J / (Math.Pow(g, 2.0) * b0 * Math.Pow(omega, 2.0));
            return tauSD;
        }

        public static double ComputeDoublePanelLowFreqSTC(FacadeComponent component, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;
            double m1 = SinglePanel.ComputeSurfaceMass(plates[0]);
            double m2 = SinglePanel.ComputeSurfaceMass(plates[1]);
            double k = 2.0 * Math.PI * f / c;

            double ab = 2.0 * Math.PI * f * (m1 + m2) / (2.0 * rooms[0].Rho * c);
            double COSb;
            {
                if (1.0 / (k * Math.Sqrt(plates[0].Lx * plates[0].Ly)) > 0.9)
                {
                    COSb = 0.9;
                }
                else
                {
                    COSb = 1.0 / (k * Math.Sqrt(plates[0].Lx * plates[0].Ly));
                }
            }
            double tauLOW = Math.Log((1.0 + Math.Pow(ab, 2.0)) / (1.0 + Math.Pow(ab, 2.0) * COSb)) / Math.Pow(ab, 2.0);
            return tauLOW;
        }

        public static double ComputeDoublePanelsigmaZeroZero(FacadeComponent component, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;
            double sigmaZeroZero;
            if (plates[0].Material == plates[1].Material && plates[0].Material == Plate.MaterialT.glass)
            {
                if (rooms[0].Lz <= 0.0127)
                {
                    sigmaZeroZero = 0.4 * Math.Abs((plates[0].H - plates[1].H)) * 1000 + 0.04 * rooms[0].Lz * 1000 + 0.52;
                }
                else
                {
                    sigmaZeroZero = 0.4 * Math.Abs((plates[0].H - plates[1].H)) * 1000 + 0.025 * rooms[0].Lz * 1000 + 0.52;
                }
            }
            else
            {
                if (rooms[0].Lz <= 0.0127)
                {
                    sigmaZeroZero = 0.2 * Math.Abs((plates[0].H - plates[1].H)) * 1000 + 0.04 * rooms[0].Lz * 1000 + 0.52;
                }
                else
                {
                    sigmaZeroZero = 0.2 * Math.Abs((plates[0].H - plates[1].H)) * 1000 + 0.025 * rooms[0].Lz * 1000 + 0.52;
                }
            }

            return sigmaZeroZero;
        }

        public static double ComputeDoublePanelSigma(FacadeComponent component, Plate plate, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;
            double sigmaZeroZero = ComputeDoublePanelsigmaZeroZero(component, f);
            double sigsigma = SinglePanel.ComputeSinglePanelSigmaZero(plate, f);
            double sigsigma09 = SinglePanel.ComputeSinglePanelSigmaZero(plate, 0.9 * f);
            double fc = SinglePanel.GetCriticalFreq(plate);

            double sigma;
            if (f >= fc)
            {
                sigma = Math.Min(sigmaZeroZero, sigsigma);
            }
            else if (f <= 0.9 * fc)
            {
                sigma = sigsigma;
            }
            else
            {
                sigma = sigsigma09 + (f - 0.9 * fc) * (Math.Min(sigmaZeroZero, sigsigma) - sigsigma09) / (0.1 * fc);
            }
            return sigma;
        }

        public static double ComputeDoublePanelHighFreqSTC(FacadeComponent component, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;
            double xi1 = f / SinglePanel.GetCriticalFreq(plates[0]);
            double xi2 = f / SinglePanel.GetCriticalFreq(plates[1]);
            double m1 = SinglePanel.ComputeSurfaceMass(plates[0]);
            double m2 = SinglePanel.ComputeSurfaceMass(plates[1]);

            double p1 = 1.0 - 1.0 / xi1;
            double p2 = 1.0 - 1.0 / xi2;
            double aa1 = Math.PI * f * (m1) / (rooms[0].Rho * c);
            double aa2 = Math.PI * f * (m2) / (rooms[1].Rho * c);
            double aeff = plates[0].Lx * plates[0].Ly / (plates[0].Lx + plates[0].Ly);

            double sigma1 = ComputeDoublePanelSigma(component, plates[0], f);
            double sigma2 = ComputeDoublePanelSigma(component, plates[1], f);
            double s1 = 2.0 * aa1 * xi1 / sigma1;
            double s2 = 2.0 * aa2 * xi2 / sigma2;

            double q1 = (1.0 + aa1 * ComputeDoublePanelDamingLossFactorB(plates[0], rooms[0], f) / sigma1) / s1;
            double q2 = (1.0 + aa2 * ComputeDoublePanelDamingLossFactorB(plates[1], rooms[0], f) / sigma2) / s2;

            double It1 = GaussLegendreRule.Integrate((x) =>
            {
                double term1 = Math.Pow(q1, 2.0) + Math.Pow((x - p1), 2.0);
                double term2 = Math.Pow(q2, 2.0) + Math.Pow((x - p2), 2.0);
                return 1.0 / (term1 * term2);
            }, 0.0, 1.0, INTEGRATE_ORDER);
            double alpha;
            if (plates[0].Material == plates[1].Material && plates[0].Material == Plate.MaterialT.glass)
            {
                alpha = 0.0172 - 1.5 * (plates[0].H + plates[1].H) * 0.5 + 2.2 * rooms[0].Lz;
            }
            else
            {
                alpha = 0.0172 - 1.5 * (plates[0].H + plates[1].H) * 0.5 + 3.6 * rooms[0].Lz;
            }

            double tauH = It1 / Math.Pow(s1 * s2 * alpha, 2.0);
            return tauH;
        }

        public static double ComputeDoublePanelMidFreqSTC(FacadeComponent component, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;
            double omega = 2.0 * Math.PI * f;
            double m1 = SinglePanel.ComputeSurfaceMass(plates[0]);
            double m2 = SinglePanel.ComputeSurfaceMass(plates[1]);
            double k = 2.0 * Math.PI * f / c;
            double aeff = plates[0].Lx * plates[0].Ly / (plates[0].Lx + plates[0].Ly);

            double alpha;
            if (plates[0].Material == plates[1].Material && plates[0].Material == Plate.MaterialT.glass)
            {
                alpha = 0.0172 - 1.5 * (plates[0].H + plates[1].H) * 0.5 + 2.2 * rooms[0].Lz;
            }
            else // todo
            {
                alpha = 0.0172 - 1.5 * (plates[0].H + plates[1].H) * 0.5 + 3.6 * rooms[0].Lz;
            }

            double ab10 = Math.PI * f * (m1) / (rooms[0].Rho * c);
            double ab20 = Math.PI * f * (m2) / (rooms[1].Rho * c);

            double ab1 = omega * m1 * (1.0 - Math.Pow(omega / (2.0 * Math.PI * SinglePanel.GetCriticalFreq(plates[0])), 2.0)) / (2.0 * rooms[0].Rho * c);
            double ab2 = omega * m2 * (1.0 - Math.Pow(omega / (2.0 * Math.PI * SinglePanel.GetCriticalFreq(plates[1])), 2.0)) / (2.0 * rooms[0].Rho * c);

            double P = ab1 * ab2 * alpha;
            double Q = 0.5 * (ab2 / ab1 + ab1 / ab2);

            double COSmidR;
            {
                if (1.0 / (k * 2.0 * aeff) > 0.9)
                {
                    COSmidR = 0.9;
                }
                else if (1.0 / (k * 2.0 * aeff) <= 0.9 && 1.0 / (k * 2.0 * aeff) >= Math.Pow(Math.Cos(61 * Math.PI / 180.0), 2.0)) // todo
                {
                    COSmidR = 1.0 / (k * 2.0 * aeff);
                }
                else
                {
                    COSmidR = Math.Pow(Math.Cos(61 * Math.PI / 180.0), 2.0);
                }
            }

            double taum = (1.0 - COSmidR) / ((Q + P * COSmidR) * (Q + P));
            double tauH = ComputeDoublePanelHighFreqSTC(component, f);
            double tauM = taum + tauH;
            return tauM;
        }

        public static double ComputeDoublePanelL2FreqSTC(FacadeComponent component, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;

            double f0 = ComputeDoublePanelResonantFrequency(plates[0], plates[1], rooms[0]);
            double ks3 = (ComputeDoublePanelMidFreqSTC(component, f0) - ComputeDoublePanelLowFreqSTC(component, (2.0 / 3.0) * f0))
                            / (f0 / 3.0);
            double tauL2 = ComputeDoublePanelLowFreqSTC(component, (2.0 / 3.0) * f0) + ks3 * (f - (2.0 / 3.0) * f0);

            return tauL2;
        }

        public static double ComputeDoublePanelMHFreqSTC(FacadeComponent component, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;
            double k0 = 0.8;

            double fmax = Math.Max(SinglePanel.GetCriticalFreq(plates[0]), SinglePanel.GetCriticalFreq(plates[1]));
            double fmin = Math.Min(SinglePanel.GetCriticalFreq(plates[0]), SinglePanel.GetCriticalFreq(plates[1]));
            double term1 = ComputeDoublePanelHighFreqSTC(component, fmax)
                        - ComputeDoublePanelMidFreqSTC(component, k0 * fmin);
            double ks4 = term1 / (fmax - k0 * fmin);
            double tauMH = ComputeDoublePanelMidFreqSTC(component, k0 * fmin) + ks4 * (f - k0 * fmin);
            return tauMH;
        }

        public static double SolveDoublePanelSTC(FacadeComponent component, double f)
        {
            var plates = component.Plates;
            var rooms = component.Rooms;
            double k0 = 0.8;

            double f0 = ComputeDoublePanelResonantFrequency(plates[0], plates[1], rooms[0]);
            double fmax = Math.Max(SinglePanel.GetCriticalFreq(plates[0]), SinglePanel.GetCriticalFreq(plates[1]));
            double fmin = Math.Min(SinglePanel.GetCriticalFreq(plates[0]), SinglePanel.GetCriticalFreq(plates[1]));

            double tau;
            {
                if (f < (2.0 / 3.0) * f0)
                {
                    tau = ComputeDoublePanelLowFreqSTC(component, f);
                }
                else if (f < f0 && f > (2.0 / 3.0) * f0)
                {
                    tau = ComputeDoublePanelL2FreqSTC(component, f);
                }
                else if (f < k0 * fmin && f >= f0)
                {
                    tau = ComputeDoublePanelMidFreqSTC(component, f) + ComputeDoublePanelStudBorneSTC(component, f);
                }
                else if (f <= fmax && f > k0 * fmin)
                {
                    tau = ComputeDoublePanelMHFreqSTC(component, f) + ComputeDoublePanelStudBorneSTC(component, f);
                }
                else
                {
                    tau = ComputeDoublePanelHighFreqSTC(component, f) + ComputeDoublePanelStudBorneSTC(component, f);
                }
            }
            return tau;
        }
    }
}
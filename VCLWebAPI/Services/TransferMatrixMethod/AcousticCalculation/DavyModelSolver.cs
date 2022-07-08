using System;
using System.Collections.Generic;
using System.Diagnostics;
using VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation
{
    public static class DavyModelSolver
    {
        private const int c = 340;
        private const int NUM_POINTS_FOR_AVERAGE_AT_CRITICAL = 100;

        public readonly static double[] FREQUENCIES = GetOneThirdOctaveFrequencies();
        public readonly static int[] STCWeight = GetSTCWeight();
        public readonly static double[] AWS = GetAWS();
        public readonly static double[] RwWeight = GetRwWeight();
        public readonly static double[] L1 = GetL1();
        public readonly static double[] L2 = GetL2();

        private static double[] GetOneThirdOctaveFrequencies()
        {
            return new double[] { 50.0, 63.0, 80.0, 100.0, 125.0, 160.0, 200.0,
                                  250.0, 315.0, 400.0, 500.0, 630.0, 800.0, 1000.0,
                                  1250.0, 1600.0, 2000.0, 2500.0, 3150.0, 4000.0, 5000.0};
        }

        private static double[] GetRequiredFrequencies()
        {
            double minF = 1278;
            double maxF = 1278;
            int numOfFrequencies = 1;
            double[] frequencies = new double[numOfFrequencies + 1];
            for (int i = 0; i <= numOfFrequencies; i++)
            {
                frequencies[i] = minF + i * (maxF - minF) / numOfFrequencies;
            }
            return frequencies;
        }

        private static int[] GetSTCWeight()
        {
            return new int[] { 0, 0, 0, 0, -16, -13, -10, -7, -4, -1,
                                  0, 1, 2, 3, 4,
                                  4, 4, 4, 4, 4, 0 };
        }

        private static double[] GetRwWeight()
        {
            return new double[] { 0, 0, 0, -19.0, -16.0, -13.0, -10.0, -7.0, -4.0, -1.0,
                                  0.0, 1.0, 2.0, 3.0, 4.0,
                                  4.0, 4.0, 4.0, 4.0, 0.0, 0.0 };
        }

        private static double[] GetAWS()
        {
            return new double[] { 0, 0, 80.5, 82.9, 84.9, 84.6, 86.1,
                                  86.4, 87.4, 88.2, 89.8, 89.1,
                                  89.2, 89, 89.6, 89, 89.2, 88.3, 86.2, 85, 0.0 };
        }

        private static double[] GetL1()
        {
            return new double[] { 0, 0, 0, -29.0, -26.0, -23.0, -21.0, -19.0,
                                  -17.0, -15.0, -13.0, -12.0, -11.0,
                                  -10.0, -9.0, -9.0, -9.0, -9.0, -9.0, 0.0, 0.0 };
        }

        private static double[] GetL2()
        {
            return new double[] { 0, 0, 0, -20.0, -20.0, -18.0, -16.0, -15.0,
                                  -14.0, -13.0, -12.0, -11.0, -9.0,
                                  -8.0, -9.0, -10.0, -11.0, -13.0, -15.0, 0.0, 0.0 };
        }

        private static double GetCriticalFreq(Plate plate)
        {
            double t = (12.0 * plate.Rho * (1.0 - Math.Pow(plate.V, 2.0))) / (plate.E * Math.Pow(plate.H, 2.0));
            return (Math.Pow(340, 2.0) / (2 * Math.PI)) * Math.Sqrt(t);
        }

        private static int FindNearestFreqIndex(double freq)
        {
            int nearestFreqIndex = 0;
            for (int i = 1; i < FREQUENCIES.Length; i++)
            {
                if (Math.Abs(FREQUENCIES[i] - freq) < Math.Abs(FREQUENCIES[nearestFreqIndex] - freq))
                {
                    nearestFreqIndex = i;
                }
            }
            return nearestFreqIndex;
        }

        private static double AverageLowerBound(double freq)
        {
            return freq * Math.Pow(2.0, -1.0 / 6.0);
        }

        private static double AverageUpperBound(double freq)
        {
            return freq * Math.Pow(2.0, 1.0 / 6.0);
        }

        public static LossDistributionPoint[] ComputeSTLDistributionOnePlateWithAverage(FacadeComponent component)
        {
            double count = component.Plates.Count;
            int nearestFreqIndex;
            if (count > 2)
            {
                throw new Exception("Only support single panel and double panel glass");
            }
            LossDistributionPoint[] distribution = new LossDistributionPoint[FREQUENCIES.Length];
            if (count == 1)
            {
                Plate plate1 = component.Plates[0]; //TODO
                double criticalFreq = GetCriticalFreq(plate1);
                nearestFreqIndex = FindNearestFreqIndex(criticalFreq);
            }
            else
            {
                Plate plate1 = component.Plates[0]; //TODO
                Plate plate2 = component.Plates[1]; //TODO
                double criticalFreq1 = SinglePanel.GetCriticalFreq(plate1);
                double criticalFreq2 = SinglePanel.GetCriticalFreq(plate2);
                //nearestFreqIndex = Math.Max(FindNearestFreqIndex(criticalFreq1),FindNearestFreqIndex(criticalFreq1));
                nearestFreqIndex = FindNearestFreqIndex(Math.Max(criticalFreq1, criticalFreq2));
            }
            //LossDistributionPoint[] distribution = new LossDistributionPoint[FREQUENCIES.Length];
            double criticalFreqLower = AverageLowerBound(FREQUENCIES[nearestFreqIndex]);
            double criticalFreqUpper = AverageUpperBound(FREQUENCIES[nearestFreqIndex]);

            for (int i = 0; i < FREQUENCIES.Length; i++)
            {
                double tau = 0.0;
                int j = 0;
                if (i == nearestFreqIndex && FREQUENCIES.Length > 2)
                {
                    //Debug.WriteLine("critical frequency is" + FREQUENCIES[i]);
                    for (double freq = criticalFreqLower; j < NUM_POINTS_FOR_AVERAGE_AT_CRITICAL + 1;
                        freq += (criticalFreqUpper - criticalFreqLower) / NUM_POINTS_FOR_AVERAGE_AT_CRITICAL)
                    {
                        //Debug.WriteLine("Current frequency is " + freq);
                        if (count == 1)
                        {
                            //Debug.WriteLine("INDEX j is: "+ (j) + " Tau at freq " + freq + " is " + SinglePanel.SolveSinglePanelSTC(component.Plates[0], component.Rooms[0], freq));
                            tau += SinglePanel.SolveSinglePanelSTC(component.Plates[0], component.Rooms[0], freq);
                        }
                        else
                        {
                            //Debug.WriteLine("INDEX j is: "+ (j) + " Tau at freq " + freq + " is " + SinglePanel.SolveSinglePanelSTC(component.Plates[0], component.Rooms[0], freq));
                            tau += DoublePanel.SolveDoublePanelSTC(component, freq);
                        }
                        j++;
                    }
                    tau /= (NUM_POINTS_FOR_AVERAGE_AT_CRITICAL + 1);
                }
                else
                {
                    double freq = FREQUENCIES[i];
                    if (count == 1)
                    {
                        tau = SinglePanel.SolveSinglePanelSTC(component.Plates[0], component.Rooms[0], freq);
                    }
                    else // todo check exception
                    {
                        tau = DoublePanel.SolveDoublePanelSTC(component, freq);
                    }
                }
                distribution[i] = new LossDistributionPoint
                {
                    Frequency = FREQUENCIES[i],
                    Tau = tau,
                    STL = ComputeSTL(tau)
                };
                Debug.WriteLine("The Glass STL is: {0} dB at {1} Hz", distribution[i].STL, FREQUENCIES[i]);
            }
            return distribution;
        }

        public static double ComputeSTL(double tau)
        {
            return 10.0 * Math.Log10(1 / tau);
        }

        public static double ComputeTau(double STL)
        {
            return 1.0 / Math.Pow(10.0, STL / 10.0);
        }

        public static LossDistributionPoint[] ComputeAverageSTLDistribution(List<FacadeComponent> components, Frame frame)
        {
            LossDistributionPoint[] res = new LossDistributionPoint[FREQUENCIES.Length];
            // For each component, we compute the distribution
            List<LossDistributionPoint[]> distributions = new List<LossDistributionPoint[]>();
            for (int i = 0; i < components.Count; i++)
            {
                distributions.Add(ComputeSTLDistributionOnePlateWithAverage(components[i]));
            }

            for (int fIndex = 0; fIndex < FREQUENCIES.Length; fIndex++)
            {
                double totalArea = 0.0, totalTau = 0.0;
                for (int i = 0; i < components.Count; i++)
                {
                    Plate plate = components[i].Plates[0];
                    double area = plate.Lx * plate.Ly;
                    totalArea += area;
                    LossDistributionPoint[] distribution = distributions[i];
                    double tau = distribution[fIndex].Tau;
                    totalTau += tau * area;
                }
                totalTau += ((frame.Lx * frame.Ly) - totalArea) * frame.GetDistribution()[fIndex].Tau;
                res[fIndex] = new LossDistributionPoint
                {
                    Frequency = FREQUENCIES[fIndex],
                    Tau = totalTau / (frame.Lx * frame.Ly),
                    STL = ComputeSTL(totalTau / (frame.Lx * frame.Ly)),
                };
                //Debug.WriteLine("Composite STL at {0} Hz is {1} dB", FREQUENCIES[fIndex], res[fIndex].STL);
            }
            return res;
        }

        public static LossDistributionPoint[] ComputeOpenSTLDistribution(Input input, LossDistributionPoint[] glassdist,
                                                                         LossDistributionPoint[] frameDist,
                                                                        double openPercentage)
        {
            if (openPercentage > 1.0 || openPercentage < 0.0)
            {
                throw new Exception("openPercentage must within [0.0, 1.0].");
            }
            LossDistributionPoint[] openDistribution = new LossDistributionPoint[glassdist.Length];
            for (int i = 0; i < openDistribution.Length; i++)
            {
                if (input.Components.Count == 1)
                {
                    double glassArea = input.Components[0].Plates[0].Lx * input.Components[0].Plates[0].Ly;
                    double frameArea = input.Frame.Lx * input.Frame.Ly - glassArea;
                    openDistribution[i] = new LossDistributionPoint
                    {
                        Frequency = FREQUENCIES[i],
                        Tau = (glassdist[i].Tau * glassArea * (1 - openPercentage) + frameDist[i].Tau * frameArea + 1 * glassArea * openPercentage)
                            / (input.Frame.Lx * input.Frame.Ly),
                        STL = ComputeSTL((glassdist[i].Tau * glassArea * (1 - openPercentage) + frameDist[i].Tau * frameArea + 1 * glassArea * openPercentage)
                            / (input.Frame.Lx * input.Frame.Ly))
                    };
                }
                else
                {
                    double glassAreaOne = input.Components[0].Plates[0].Lx * input.Components[0].Plates[0].Ly;
                    double frameArea = input.Frame.Lx * input.Frame.Ly - glassAreaOne * 2;
                    double frameSTL = frameDist[i].STL;
                    double glassSTL = glassdist[i].STL;
                    openDistribution[i] = new LossDistributionPoint
                    {
                        Frequency = FREQUENCIES[i],
                        Tau = (glassdist[i].Tau * glassAreaOne * (2 - openPercentage) + frameDist[i].Tau * frameArea + 1 * glassAreaOne * openPercentage)
                            / (input.Frame.Lx * input.Frame.Ly),
                        STL = ComputeSTL((glassdist[i].Tau * glassAreaOne * (2 - openPercentage) + frameDist[i].Tau * frameArea + 1 * glassAreaOne * openPercentage)
                            / (input.Frame.Lx * input.Frame.Ly))
                    };
                }

                Debug.WriteLine("{0}% open, f = {1} Hz, STC = {2} dB", openPercentage * 100, FREQUENCIES[i], openDistribution[i].STL);
            }
            return openDistribution;
        }

        // Classifications :  STC, OITC, Rw, C, Ctr, NC Critiria, Deficiencies
        // Rw follow standards ISO 717-1
        // STC takes integer as input. Rw takes one digit number as input

        public static Boolean AssessSTCRating(int targetSTC, LossDistributionPoint[] distribution)
        {
            int[] STCConturcurve = new int[FREQUENCIES.Length];
            int totalDeficiencies = 0;
            int deficiencies = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Contrary to Rw rating, STC rating takes integer numbers for TL input
            for (int i = 4; i < FREQUENCIES.Length - 1; i++)
            {
                STCConturcurve[i] = targetSTC + STCWeight[i];
                deficiencies = STCConturcurve[i] - Convert.ToInt32(distribution[i].STL);

                if (deficiencies > 0 && deficiencies <= 8)
                {
                    totalDeficiencies += deficiencies;
                }
                else if (deficiencies > 8)
                {
                    return false;
                }
            }
            sw.Stop();
            //Debug.WriteLine("Time to compute one targetSTC is {0} ms", sw.ElapsedMilliseconds);

            if (totalDeficiencies <= 32) return true;
            else return false;
        }

        public static int CalculateSTC(LossDistributionPoint[] distribution)
        {
            int STC = 100;

            while (STC > 0 && AssessSTCRating(STC, distribution) == false)
            {
                STC--;
            }

            return STC;
        }

        public static Boolean AssessRwRating(int targetRw, LossDistributionPoint[] distribution)
        {
            double[] RWConturcurve = new double[FREQUENCIES.Length];
            double totalDeficiencies = 0;
            double deficiencies = 0;
            double[] digitTL = new double[21];

            // Convert TL numbers to one decimal digit
            for (int i = 0; i < FREQUENCIES.Length; i++)
            {
                digitTL[i] = Convert.ToInt32(distribution[i].STL * 10 + 0.5) / 10.0;
            }

            for (int i = 3; i < FREQUENCIES.Length - 2; i++)
            {
                RWConturcurve[i] = targetRw + RwWeight[i];
                deficiencies = RWConturcurve[i] - digitTL[i];

                if (deficiencies > 0)
                {
                    totalDeficiencies += deficiencies;
                }
            }

            if (totalDeficiencies <= 32) return true;
            else return false;
        }

        public static int CalculateRw(LossDistributionPoint[] distribution)
        {
            int Rw = 100;

            while (Rw > 0 && AssessRwRating(Rw, distribution) == false)
            {
                Rw--;
            }

            return Rw;
        }

        public static int CalculateOITC(LossDistributionPoint[] distribution)
        {
            int OITC = 0;
            double sum = 0;
            for (int i = 2; i < FREQUENCIES.Length - 1; i++)
            {
                sum += Math.Pow(10, (AWS[i] - distribution[i].STL) / 10);
            }
            OITC = Convert.ToInt32(100.13 - 10 * Math.Log10(sum));
            return OITC;
        }

        public static int CalculateC1(LossDistributionPoint[] distribution)
        {
            int C1 = 0;
            double sum = 0;
            int Rw = CalculateRw(distribution);

            for (int i = 3; i < FREQUENCIES.Length - 2; i++)
            {
                sum += Math.Pow(10, (L1[i] - distribution[i].STL) / 10);
            }
            C1 = Convert.ToInt32(-10 * Math.Log10(sum) - Rw);
            return C1;
        }

        public static int CalculateC2(LossDistributionPoint[] distribution)
        {
            int C2 = 0;
            double sum = 0;
            int Rw = CalculateRw(distribution);

            for (int i = 3; i < FREQUENCIES.Length - 2; i++)
            {
                sum += Math.Pow(10, (L2[i] - distribution[i].STL) / 10);
            }
            C2 = Convert.ToInt32(-10 * Math.Log10(sum) - Rw);
            return C2;
        }

        public static int[] CalculateNC(LossDistributionPoint[] distribution)
        {
            int[] res = new int[16];
            int STC = CalculateSTC(distribution);

            for (int i = 0; i < 16; i++)
            {
                res[i] = STC + STCWeight[i + 4]; // NC starts on 125Hz, ends on 4000Hz
            }
            return res;
        }

        public static int[] CalculateDeficiencies(LossDistributionPoint[] distribution)
        {
            int[] res = new int[16];
            int STC = CalculateSTC(distribution);
            int[] NC = CalculateNC(distribution);

            for (int i = 0; i < 16; i++)
            {
                if (NC[i] <= distribution[i + 4].STL) res[i] = 0;
                else res[i] = Convert.ToInt32(NC[i] - distribution[i + 4].STL);
            }
            return res;
        }

        public static Classification ComputeClassification(LossDistributionPoint[] distribution)
        {
            // TODO

            int STC = CalculateSTC(distribution);
            int OITC = CalculateOITC(distribution);
            int Rw = CalculateRw(distribution);
            int C1 = CalculateC1(distribution);
            int C2 = CalculateC2(distribution);
            int[] NC = CalculateNC(distribution);
            int[] Deficiencies = CalculateDeficiencies(distribution);

            return new Classification
            {
                STC = STC,
                OITC = OITC,
                Rw = Rw,
                C = C1,
                Ctr = C2,
                Deficiencies = Deficiencies,
                NC = NC
            };
        }

        public static double GetFrequency(int index)
        {
            return FREQUENCIES[index];
        }
    }
}
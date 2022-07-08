using System.Collections.Generic;
using VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation
{
    public class Frame
    {
        public enum FrameT { AWS70_HI, AWS70BS_HI, AWS75_SI_PlUS, AWS75BS_HI, FWS_50_SI, FWS35_PD }; // TODO

        public double Lx { get; set; }
        public double Ly { get; set; }
        public FrameT FrameType { get; set; }

        private double[] AWS70_HI = { 18, 18, 22, 27, 38, 36, 37, 39, 39, 39, 44, 40, 42, 35, 38, 40, 39, 40, 40, 48, 46 };
        private double[] AWS70BS_HI = { 18, 18, 22, 23, 27, 24, 28, 32, 37, 33, 39, 42, 37, 38, 39, 42, 43, 41, 38, 43, 46 };
        private double[] AWS75_SI_PlUS = { 18, 18, 26, 25, 35, 40, 37, 41, 46, 43, 47, 46, 44, 41, 41, 45, 48, 49, 50, 50, 48 };
        private double[] AWS75BS_HI = { 18, 18, 22, 26, 33, 29, 33, 35, 37, 40, 36, 42, 40, 40, 40, 40, 41, 41, 40, 44, 47 };
        private double[] FWS_50_SI = { 24, 33, 28, 21, 22, 15, 21, 21, 22, 28, 28, 33, 36, 37, 40, 37, 36, 38, 40, 51, 54 };
        private double[] FWS35_PD = { 27, 34, 27, 26, 28, 15, 21, 22, 31, 36, 37, 38, 39, 42, 53, 42, 38, 39, 40, 45, 48 };

        public LossDistributionPoint[] GetDistribution()
        {
            List<double[]> frameDist = new List<double[]> { AWS70_HI,
                                                            AWS70BS_HI,
                                                            AWS75_SI_PlUS,
                                                            AWS75BS_HI,
                                                            FWS_50_SI,
                                                            FWS35_PD
            };
            List<FrameT> frameNames = new List<FrameT> { FrameT.AWS70_HI,
                                                         FrameT.AWS70BS_HI,
                                                         FrameT.AWS75_SI_PlUS,
                                                         FrameT.AWS75BS_HI,
                                                         FrameT.FWS_50_SI,
                                                         FrameT.FWS35_PD
            };

            int index = frameNames.IndexOf(FrameType);
            var distribution = new LossDistributionPoint[DavyModelSolver.FREQUENCIES.Length];
            for (int i = 0; i < DavyModelSolver.FREQUENCIES.Length; i++)
            {
                distribution[i] = new LossDistributionPoint
                {
                    Frequency = DavyModelSolver.FREQUENCIES[i],
                    Tau = DavyModelSolver.ComputeTau(frameDist[index][i]),
                    STL = frameDist[index][i]
                };
            }
            return distribution;
        }
    }
}
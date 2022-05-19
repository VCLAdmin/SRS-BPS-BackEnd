using VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation
{
    internal class PredefinedFilter
    {
        public readonly static double[] FREQUENCIES = GetOneThirdOctaveFrequencies();

        private static double[] GetOneThirdOctaveFrequencies()
        {
            return new double[] { 50.0, 63.0, 80.0, 100.0, 125.0, 160.0, 200.0,
                                  250.0, 315.0, 400.0, 500.0, 630.0, 800.0, 1000.0,
                                  1250.0, 1600.0, 2000.0, 2500.0, 3150.0, 4000.0, 5000.0};
        }

        public static LossDistributionPoint[] ComputeLossDistributionPoint(double[] predefinedFilter)
        {
            LossDistributionPoint[] res = new LossDistributionPoint[predefinedFilter.Length];

            for (int i = 0; i < predefinedFilter.Length; i++)
            {
                res[i] = new LossDistributionPoint
                {
                    Frequency = FREQUENCIES[i],
                    Tau = DavyModelSolver.ComputeTau(predefinedFilter[i]),
                    STL = predefinedFilter[i],
                };
            }

            return res;
        }
    }
}
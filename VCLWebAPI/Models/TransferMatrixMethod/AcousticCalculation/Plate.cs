using VCLWebAPI.Services.TransferMatrixMethod.Utility;

namespace VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation
{
    public class Plate
    {
        public enum MaterialT { glass, lamiSGP, lamiPVB, lamiSC };

        public MaterialT Material { get; set; }
        public double Rho { get; set; } = 2500; // density
        public double H { get; set; } // glass thickness
        public double Lx { get; set; } // plate width, only define plate size for the first plate
        public double Ly { get; set; } // plate height, only define plate size for the first plate
        public double E { get; set; } = 65 * 1e9; // Young's Modulus,
        public double Eta { get; set; } = 0.05; // Damping loss factor
        public double V { get; set; } = 0.22; // possion ratio
        public double InterH { get; set; }  // interlayer thickness
        public double InterRho { get; set; } // interlayer density
        public double InterG { get; set; } // interlayer G
        public double InterEta { get; set; } // interlayer damping loss factor

        public override string ToString()
        {
            return Utility.ToString<Plate>(this);
        }
    }
}
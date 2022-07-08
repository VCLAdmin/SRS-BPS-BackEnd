using VCLWebAPI.Services.TransferMatrixMethod.Utility;

namespace VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation
{
    public class Room
    {
        public bool IsSource { get; set; } = false;
        public bool IsReceiving { get; set; } = false;

        public enum CavityType { Air, Argon };

        public CavityType CavityT { get; set; }
        public double Lz { get; set; }
        public double Rho { get; set; }
        //public double Alpha { get; set; } = 0.025; // attenuation constant, refer to dissertation Equation 5 - 8
        //public double T { get; set; } = 1.5;  // chamber reverberation time

        public override string ToString()
        {
            return Utility.ToString<Room>(this);
        }
    }
}
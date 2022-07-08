namespace VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation
{
    public class Classification
    {
        public int STC { get; set; }
        public int OITC { get; set; }
        public int Rw { get; set; }
        public int C { get; set; }
        public int Ctr { get; set; }
        public int[] NC { get; set; }
        public int[] Deficiencies { get; set; }

        public override string ToString()
        {
            return Utility.Utility.ToString<Classification>(this);
        }
    }
}
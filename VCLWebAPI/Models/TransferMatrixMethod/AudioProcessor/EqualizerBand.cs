namespace VCLWebAPI.Models.TransferMatrixMethod.AudioProcessor
{
    public class EqualizerBand
    {
        public float Frequency { get; set; }
        public float Bandwidth { get; set; }
        public float Gain { get; set; } = 0.0f;
    }
}
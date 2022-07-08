namespace VCLWebAPI.Models.Acoustics
{
    public class FieldTestResultApiModel
    {
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductCode { get; set; }
        public string OpeningType { get; set; }
        public string GlassLite1 { get; set; }
        public string GlassAirSpaceOne { get; set; }
        public string GlassLite2 { get; set; }
        public string GlassAirSpaceTwo { get; set; }
        public string GlassLite3 { get; set; }

        public int STC { get; set; }
        public int OITC { get; set; }
        public int Rw { get; set; }
        public int C { get; set; }
        public int Ctr { get; set; }

        public string FileName { get; set; }
        public string ProductImageUrl { get; set; }
        public string TestResultUrl { get; set; }
    }
}
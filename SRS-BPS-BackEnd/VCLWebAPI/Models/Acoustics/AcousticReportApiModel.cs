using System.Web.Mvc;

namespace VCLWebAPI.Models.Acoustics
{
    public class AcousticReportApiModel
    {
        [AllowHtml]
        public string[] HtmlStrings { get; set; }

        public string ProductName { get; set; }
        public string ProjectName { get; set; }
        public string ProductType { get; set; }
        public string ProductThicknessUnit { get; set; }
        public string Glass { get; set; }
        public string RwCtr { get; set; }
        public string WindowWidthUnit { get; set; }
        public string WindowHeightUnit { get; set; }
        public float ProductThickness { get; set; }
        public float Stc { get; set; }
        public float Oitc { get; set; }
        public float WindowWidth { get; set; }
        public float WindowHeight { get; set; }

        public AcousticReportApiModel()
        {
            ProductName = string.Empty;
            ProjectName = "AcousticReport";
            ProductType = string.Empty;
            ProductThicknessUnit = string.Empty;
            Glass = string.Empty;
            RwCtr = string.Empty;
            WindowWidthUnit = string.Empty;
            WindowHeightUnit = string.Empty;

            ProductThickness = 0;
            Stc = 0;
            Oitc = 0;
            WindowWidth = 0;
            WindowHeight = 0;
        }
    }
}
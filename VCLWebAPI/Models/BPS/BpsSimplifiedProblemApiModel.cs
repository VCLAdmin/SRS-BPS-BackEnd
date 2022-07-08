using System;

namespace VCLWebAPI.Models.BPS
{
    public class BpsSimplifiedProblemApiModel
    {
        public Guid ProblemGuid { get; set; }
        public string ProblemName { get; set; }
        public bool updateStructuralReport { get; set; }
        public bool updateAcousticReport { get; set; }
        public bool updateThermalReport { get; set; }

        public BpsSimplifiedProblemApiModel()
        {
            ProblemGuid = Guid.Empty;
            ProblemName = String.Empty;
            updateStructuralReport = false;
            updateAcousticReport = false;
            updateThermalReport = false;
        }
    }
}
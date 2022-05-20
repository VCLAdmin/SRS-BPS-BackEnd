using System.Collections.Generic;

namespace VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation
{
    public class Input
    {
        // DATA
        public List<FacadeComponent> Components { get; set; }

        public Frame Frame { get; set; }
        public List<double> OpenPercentages { get; set; }
    }
}
using BpsUnifiedModelLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.BPS
{
    public class BpsUnifiedProblemApiModelLite
    {
        public int ProblemId { get; set; }
        public System.Guid ProblemGuid { get; set; }
        public string ProblemName { get; set; }
        public int ProjectId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public string UnifiedModel { get; set; }
        public bool OrderPlaced { get; set; }
        public Nullable<System.DateTime> OrderPlacedCreatedOn { get; set; }
        public string OrderStatus { get; set; }
        public bool AcousticResult { get; set; }
        public bool StructuralResult { get; set; }
        public bool ThermalResult { get; set; }
        public bool EnableAcoustic { get; set; }
        public bool EnableStructural { get; set; }
        public bool EnableThermal { get; set; }
        public BpsUnifiedProblemApiModelLite(BpsUnifiedProblem dbModel)
        {
            ProblemId = dbModel.ProblemId;
            ProblemGuid = dbModel.ProblemGuid;
            ProblemName = dbModel.ProblemName;
            ProjectId = dbModel.ProjectId;
            CreatedOn = dbModel.CreatedOn;
            ModifiedOn = dbModel.ModifiedOn;
            var bpsUM = JsonConvert.DeserializeObject<BpsUnifiedModel>(dbModel.UnifiedModel);

            if (bpsUM != null && bpsUM.AnalysisResult != null)
            {
                AcousticResult = bpsUM.AnalysisResult.AcousticResult != null ? true : false;
                StructuralResult = ((bpsUM.AnalysisResult.StructuralResult != null ? true : false) || (bpsUM.AnalysisResult.FacadeStructuralResult != null ? true : false));
                ThermalResult = bpsUM.AnalysisResult.ThermalResult != null ? true : false;
            }
            else
            {
                AcousticResult = false;
                StructuralResult = false;
                ThermalResult = false;
            }

            if (bpsUM != null && bpsUM.AnalysisResult != null)
            {
                EnableAcoustic = !(bpsUM.AnalysisResult.AcousticResult != null ? true : false);
                EnableStructural = !(((bpsUM.AnalysisResult.StructuralResult != null ? true : false) || (bpsUM.AnalysisResult.FacadeStructuralResult != null ? true : false)));
                EnableThermal = !(bpsUM.AnalysisResult.ThermalResult != null ? true : false);
            }
            else if (bpsUM != null && bpsUM.ProblemSetting != null)
            {
                EnableAcoustic = !bpsUM.ProblemSetting.EnableAcoustic;
                EnableStructural = !bpsUM.ProblemSetting.EnableStructural;
                EnableThermal = !bpsUM.ProblemSetting.EnableThermal;
            }
            else
            {
                EnableAcoustic = true;
                EnableStructural = true;
                EnableThermal = true;
            }
            OrderPlaced = false;
            OrderPlacedCreatedOn = null;
            OrderStatus = "";
            if (dbModel.OrderDetails != null && dbModel.OrderDetails.Count > 0)
            {
                OrderPlacedCreatedOn = dbModel.OrderDetails.First().Order.CreatedOn;
                OrderPlaced = true;
            }
        }
    }
}
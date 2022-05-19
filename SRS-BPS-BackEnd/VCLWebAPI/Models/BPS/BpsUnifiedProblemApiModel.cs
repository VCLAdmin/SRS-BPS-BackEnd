using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.BPS
{
    public class BpsUnifiedProblemApiModel
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
        public BpsUnifiedProblemApiModel(BpsUnifiedProblem dbModel)
        {
            ProblemId = dbModel.ProblemId;
            ProblemGuid = dbModel.ProblemGuid;
            ProblemName = dbModel.ProblemName;
            ProjectId = dbModel.ProjectId;
            CreatedOn = dbModel.CreatedOn;
            ModifiedOn = dbModel.ModifiedOn;
            UnifiedModel = dbModel.UnifiedModel;
            OrderPlaced = false;
            OrderPlacedCreatedOn = null;
            OrderStatus = "";
        }
    }
}
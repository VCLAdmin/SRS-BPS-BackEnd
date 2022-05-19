using System;

namespace VCLWebAPI.Models.BPS
{
    public class BpsBoundaryConditionApiModel
    {
        public Guid BoundaryConditionGuid { get; set; }
        public Guid ProblemGuid { get; set; }
        public byte[] BoundaryConditionNode { get; set; }

        public BpsBoundaryConditionApiModel()
        {
            BoundaryConditionGuid = Guid.Empty;
            ProblemGuid = Guid.Empty;
            BoundaryConditionNode = new byte[0];
        }
    }
}
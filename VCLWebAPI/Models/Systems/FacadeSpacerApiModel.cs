using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class FacadeSpacerApiModel
    {
        public FacadeSpacerApiModel(FacadeSpacer facadeSpacer)
        {
            if (facadeSpacer == null)
                return;

            FacadeSpacerId = facadeSpacer.FacadeSpacerId;
            FacadeSpacerGuid = facadeSpacer.FacadeSpacerGuid;
            System = facadeSpacer.System;
            InsulationZone = facadeSpacer.InsulationZone;
            GlazingLayer = facadeSpacer.GlazingLayer;
            Depth = facadeSpacer.Depth;
            FacadeSpacer_1 = facadeSpacer.FacadeSpacer_1;
            FacadeSpacer_2 = facadeSpacer.FacadeSpacer_2;
            FacadeSpacer_3 = facadeSpacer.FacadeSpacer_3;
            FacadeSpacer_83 = facadeSpacer.FacadeSpacer_83;
            FacadeSpacer_51 = facadeSpacer.FacadeSpacer_51;
            FacadeSpacer_22 = facadeSpacer.FacadeSpacer_22;
            FacadeSpacer_31 = facadeSpacer.FacadeSpacer_31;
            FacadeSpacer_71 = facadeSpacer.FacadeSpacer_71;
            FacadeSpacer_52 = facadeSpacer.FacadeSpacer_52;
            FacadeSpacer_85 = facadeSpacer.FacadeSpacer_85;
            FacadeSpacer_92 = facadeSpacer.FacadeSpacer_92;
            FacadeSpacer_101 = facadeSpacer.FacadeSpacer_101;
            FacadeSpacer_93 = facadeSpacer.FacadeSpacer_93;
            FacadeSpacer_21 = facadeSpacer.FacadeSpacer_21;
            FacadeSpacer_111 = facadeSpacer.FacadeSpacer_111;
            FacadeSpacer_61 = facadeSpacer.FacadeSpacer_61;
            FacadeSpacer_72 = facadeSpacer.FacadeSpacer_72;
            FacadeSpacer_81 = facadeSpacer.FacadeSpacer_81;
            FacadeSpacer_84 = facadeSpacer.FacadeSpacer_84;
            FacadeSpacer_11 = facadeSpacer.FacadeSpacer_11;
            FacadeSpacer_91 = facadeSpacer.FacadeSpacer_91;
            FacadeSpacer_23 = facadeSpacer.FacadeSpacer_23;
            FacadeSpacer_41 = facadeSpacer.FacadeSpacer_41;
            FacadeSpacer_82 = facadeSpacer.FacadeSpacer_82;
        }

        public int FacadeSpacerId { get; set; }
        public Nullable<System.Guid> FacadeSpacerGuid { get; set; }
        public string System { get; set; }
        public string InsulationZone { get; set; }
        public int GlazingLayer { get; set; }
        public int Depth { get; set; }
        public double FacadeSpacer_1 { get; set; }
        public double FacadeSpacer_2 { get; set; }
        public double FacadeSpacer_3 { get; set; }
        public double FacadeSpacer_83 { get; set; }
        public double FacadeSpacer_51 { get; set; }
        public double FacadeSpacer_22 { get; set; }
        public double FacadeSpacer_31 { get; set; }
        public double FacadeSpacer_71 { get; set; }
        public double FacadeSpacer_52 { get; set; }
        public double FacadeSpacer_85 { get; set; }
        public double FacadeSpacer_92 { get; set; }
        public double FacadeSpacer_101 { get; set; }
        public double FacadeSpacer_93 { get; set; }
        public double FacadeSpacer_21 { get; set; }
        public double FacadeSpacer_111 { get; set; }
        public double FacadeSpacer_61 { get; set; }
        public double FacadeSpacer_72 { get; set; }
        public double FacadeSpacer_81 { get; set; }
        public double FacadeSpacer_84 { get; set; }
        public double FacadeSpacer_11 { get; set; }
        public double FacadeSpacer_91 { get; set; }
        public double FacadeSpacer_23 { get; set; }
        public double FacadeSpacer_41 { get; set; }
        public double FacadeSpacer_82 { get; set; }
    }
}
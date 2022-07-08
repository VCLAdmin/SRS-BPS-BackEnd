using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class FacadeProfileApiModel
    {
        public FacadeProfileApiModel()
        {
        }

        public FacadeProfileApiModel(FacadeProfile facadeProfile)
        {
            if (facadeProfile == null)
                return;

            FacadeProfileId = facadeProfile.FacadeProfileId;
            FacadeProfileGuid = facadeProfile.FacadeProfileGuid;
            System = facadeProfile.System;
            InsulationZone = facadeProfile.InsulationZone;
            ProfileType = facadeProfile.ProfileType;
            GlassThicknessMin = facadeProfile.GlassThicknessMin;
            GlassThicknessMax = facadeProfile.GlassThicknessMax;
            FacadeProfileTable_k = facadeProfile.FacadeProfileTable_k;
            FacadeProfileTable_l = facadeProfile.FacadeProfileTable_l;
        }

        public int FacadeProfileId { get; set; }
        public Nullable<System.Guid> FacadeProfileGuid { get; set; }
        public string System { get; set; }
        public string InsulationZone { get; set; }
        public string ProfileType { get; set; }
        public int GlassThicknessMin { get; set; }
        public int GlassThicknessMax { get; set; }
        public double FacadeProfileTable_k { get; set; }
        public double FacadeProfileTable_l { get; set; }
    }
}
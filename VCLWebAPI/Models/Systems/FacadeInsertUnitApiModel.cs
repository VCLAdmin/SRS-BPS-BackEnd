using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class FacadeInsertUnitApiModel
    {
        public FacadeInsertUnitApiModel(FacadeInsertUnit facadeInsertUnit)
        {
            if (facadeInsertUnit == null)
                return;
            FacadeInsertUnitId = facadeInsertUnit.FacadeInsertUnitId;
            FacadeInsertUnitGuid = facadeInsertUnit.FacadeInsertUnitGuid;
            System = facadeInsertUnit.System;
            OpeningType = facadeInsertUnit.OpeningType;
            Material = facadeInsertUnit.Material;
            GlassThicknessMin = facadeInsertUnit.GlassThicknessMin;
            GlassThicknessMax = facadeInsertUnit.GlassThicknessMax;
            Uf = facadeInsertUnit.Uf;
        }

        public int FacadeInsertUnitId { get; set; }
        public Nullable<System.Guid> FacadeInsertUnitGuid { get; set; }
        public string System { get; set; }
        public string OpeningType { get; set; }
        public string Material { get; set; }
        public int GlassThicknessMin { get; set; }
        public int GlassThicknessMax { get; set; }
        public double Uf { get; set; }
    }
}
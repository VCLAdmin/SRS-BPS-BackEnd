using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class GlassBPSApiModel
    {
        public GlassBPSApiModel(GlassBPS glassBPS)
        {
            if (glassBPS == null)
                return;
            GlassTypeID = glassBPS.GlassTypeID;
            GlassType = glassBPS.GlassType;
            Composition = glassBPS.Composition;
            Type = glassBPS.Type;
            Total_Thickness = glassBPS.Total_Thickness;
            U_value = glassBPS.U_value;
            Rw = glassBPS.Rw;
            Spacer = glassBPS.Spacer;
        }

        public int GlassTypeID { get; set; }
        public string GlassType { get; set; }
        public string Composition { get; set; }
        public string Type { get; set; }
        public Nullable<double> Total_Thickness { get; set; }
        public Nullable<double> U_value { get; set; }
        public Nullable<int> Rw { get; set; }
        public Nullable<int> Spacer { get; set; }
    }
}
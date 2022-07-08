using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class GlassSRSApiModel
    {
        public GlassSRSApiModel(GlassSRS glassSRS)
        {
            if (glassSRS == null)
                return;
            GlassTypeID = glassSRS.GlassTypeID;
            GlassType = glassSRS.GlassType;
            Composition = glassSRS.Composition;
            Type = glassSRS.Type;
            Total_Thickness = glassSRS.Total_Thickness;
            U_value = glassSRS.U_value;
            Rw = glassSRS.Rw;
            Spacer = glassSRS.Spacer;
            shgc = glassSRS.shgc;
            vt = glassSRS.vt;
            rwc = glassSRS.rwc;
            rwctr = glassSRS.rwctr;
            stc = glassSRS.stc;
            oitc = glassSRS.oitc;
        }

        public int GlassTypeID { get; set; }
        public string GlassType { get; set; }
        public string Composition { get; set; }
        public string Type { get; set; }
        public Nullable<double> Total_Thickness { get; set; }
        public Nullable<double> U_value { get; set; }
        public Nullable<int> Rw { get; set; }
        public Nullable<int> Spacer { get; set; }
        public Nullable<double> shgc { get; set; }
        public Nullable<double> vt { get; set; }
        public Nullable<int> rwc { get; set; }
        public Nullable<int> rwctr { get; set; }
        public Nullable<int> stc { get; set; }
        public Nullable<int> oitc { get; set; }
    }
}
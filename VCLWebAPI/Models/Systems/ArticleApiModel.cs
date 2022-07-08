using System;

namespace VCLWebAPI.Models.Systems
{
    public class ArticleApiModel
    {
        public int ArticleId { get; set; }
        public Nullable<System.Guid> ArticleGuid { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public int ArticleTypeId { get; set; }
        public string CrossSectionUrl { get; set; }
        public string Description { get; set; }
        public Nullable<double> InsideDimension { get; set; }
        public Nullable<double> OutsideDimension { get; set; }
        public Nullable<double> Dimension { get; set; }
        public Nullable<double> LeftRebate { get; set; }
        public Nullable<double> RightRebate { get; set; }
        public Nullable<double> DistBetweenIsoBars { get; set; }

        //this is for internal use only in Angular UI
        public Nullable<double> Depth { get; set; }
    }
}
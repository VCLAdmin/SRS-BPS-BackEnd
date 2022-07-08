using System;
using VCLWebAPI.Models.Edmx;


namespace VCLWebAPI.Models.Systems
{
    public class DoorHandleHingeApiModel
    {
        public DoorHandleHingeApiModel(DoorHandleHinge dhhArticle)
        {
            if (dhhArticle == null)
                return;
            ArticleSetID = dhhArticle.ArticleSetID;
            ArticleName = dhhArticle.ArticleName;
            Color = dhhArticle.Color;
            ColorCode = dhhArticle.Color_Code;
            Type = dhhArticle.Type;
            Description = dhhArticle.Description;
        }

        public int ArticleSetID { get; set; }
        public string ArticleName { get; set; }
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
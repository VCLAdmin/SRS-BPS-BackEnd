using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class UDCArticleApiModel
    {
        public UDCArticleApiModel(UDCArticle udcArticle)
        {
            if (udcArticle == null)
                return;
            ArticleSetID = udcArticle.ArticleSetID;
            ArticleName = udcArticle.ArticleName;
            Type = udcArticle.Type;
            AB = udcArticle.AB;
            BT = udcArticle.BT;
            BottomFraming = udcArticle.BottomFraming;
        }

        public int ArticleSetID { get; set; }
        public string ArticleName { get; set; }
        public string Type { get; set; }
        public Nullable<int> AB { get; set; }
        public Nullable<int> BT { get; set; }
        public string BottomFraming { get; set; }
    }
}
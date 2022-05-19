using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class ADSArticleApiModel
    {
        public ADSArticleApiModel(ADSArticle adsArticle)
        {
            if (adsArticle == null)
                return;
            ArticleSetID = adsArticle.ArticleSetID;
            ArticleName = adsArticle.ArticleName;
            System = adsArticle.System;
            Type = adsArticle.Type;
            InsideW = adsArticle.InsideW;
            OutsideW = adsArticle.OutsideW;
        }

        public int ArticleSetID { get; set; }
        public string ArticleName { get; set; }
        public string System { get; set; }
        public string Type { get; set; }
        public Nullable<int> InsideW { get; set; }
        public Nullable<int> OutsideW { get; set; }
    }
}
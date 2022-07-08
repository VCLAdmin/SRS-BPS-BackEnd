using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class ASEArticleApiModel
    {
        public ASEArticleApiModel(ASEArticle aseArticle)
        {
            if (aseArticle == null)
                return;
            ArticleSetID = aseArticle.ArticleSetID;
            ArticleName = aseArticle.ArticleName;
            System = aseArticle.System;
            SRS = aseArticle.SRS;
            Category = aseArticle.Category;
            ProfileType = aseArticle.ProfileType;
            InsideW = aseArticle.InsideW;
            OutsideW = aseArticle.OutsideW;
            Depth = aseArticle.Depth;
            IsolatorType = aseArticle.IsolatorType;
            ExtrusionLength = aseArticle.ExtrusionLength;
        }

        public int ArticleSetID { get; set; }
        public string ArticleName { get; set; }
        public string System { get; set; }
        public Nullable<int> SRS { get; set; }
        public string Category { get; set; }
        public string ProfileType { get; set; }
        public Nullable<double> InsideW { get; set; }
        public Nullable<double> OutsideW { get; set; }
        public Nullable<double> Depth { get; set; }
        public string IsolatorType { get; set; }
        public Nullable<double> ExtrusionLength { get; set; }
    }
}
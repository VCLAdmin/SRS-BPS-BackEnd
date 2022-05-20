using System;
using VCLWebAPI.Models.Edmx;

namespace VCLWebAPI.Models.Systems
{
    public class FacadeArticleApiModel
    {
        public FacadeArticleApiModel(FacadeArticle facadeArticle)
        {
            if (facadeArticle == null)
                return;
            ArticleSetID = facadeArticle.ArticleSetID;
            System = facadeArticle.System;
            Mullion = facadeArticle.Mullion;
            Mullion_Depth = facadeArticle.Mullion_Depth;
            Mullion_Reinforcement_Type1 = facadeArticle.Mullion_Reinforcement_Type1;
            Mullion_Reinforcement_Type1_Material = facadeArticle.Mullion_Reinforcement_Type1_Material;
            Mullion_Reinforcement_Type2 = facadeArticle.Mullion_Reinforcement_Type2;
            Mullion_Reinforcement_Type2_Material = facadeArticle.Mullion_Reinforcement_Type2_Material;
            Transom = facadeArticle.Transom;
            Transom_Depth = facadeArticle.Transom_Depth;
            Level_2_Transom = facadeArticle.Level_2_Transom;
            Level_2_Transom_Depth = facadeArticle.Level_2_Transom_Depth;
            Transom_Reinforcement = facadeArticle.Transom_Reinforcement;
            Transom_Reinforcement_Material = facadeArticle.Transom_Reinforcement_Material;
        }

        public int ArticleSetID { get; set; }
        public string System { get; set; }
        public string Mullion { get; set; }
        public Nullable<int> Mullion_Depth { get; set; }
        public string Mullion_Reinforcement_Type1 { get; set; }
        public string Mullion_Reinforcement_Type1_Material { get; set; }
        public string Mullion_Reinforcement_Type2 { get; set; }
        public string Mullion_Reinforcement_Type2_Material { get; set; }
        public string Transom { get; set; }
        public Nullable<int> Transom_Depth { get; set; }
        public string Level_2_Transom { get; set; }
        public Nullable<int> Level_2_Transom_Depth { get; set; }
        public string Transom_Reinforcement { get; set; }
        public string Transom_Reinforcement_Material { get; set; }
    }
}
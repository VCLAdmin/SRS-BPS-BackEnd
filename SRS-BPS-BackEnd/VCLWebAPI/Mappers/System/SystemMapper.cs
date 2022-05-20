using System.Collections.Generic;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.Systems;

namespace VCLWebAPI.Mappers.System
{
    /// <summary>
    /// Defines the <see cref="SystemMapper" />.
    /// </summary>
    public class SystemMapper
    {
        /// <summary>
        /// The ArticleDbToApiModel.
        /// </summary>
        /// <param name="article">The article<see cref="Article"/>.</param>
        /// <returns>The <see cref="ArticleApiModel"/>.</returns>
        public ArticleApiModel ArticleDbToApiModel(Article article)
        {
            var articleApiModel = new ArticleApiModel();
            if (article != null)
            {
                articleApiModel.ArticleId = article.ArticleId;
                articleApiModel.ArticleGuid = article.ArticleGuid;
                articleApiModel.Name = article.Name;
                articleApiModel.Unit = article.Unit;
                articleApiModel.ArticleTypeId = article.ArticleTypeId;
                articleApiModel.CrossSectionUrl = article.CrossSectionUrl;
                articleApiModel.Description = article.Description;
                articleApiModel.InsideDimension = article.InsideDimension;
                articleApiModel.OutsideDimension = article.OutsideDimension;
                articleApiModel.Dimension = article.Dimension;
                articleApiModel.LeftRebate = article.LeftRebate;
                articleApiModel.RightRebate = article.RightRebate;
                articleApiModel.DistBetweenIsoBars = article.DistBetweenIsoBars;
                articleApiModel.Depth = article.Depth;
            }

            return articleApiModel;
        }

        /// <summary>
        /// The ProductDbToApiModel.
        /// </summary>
        /// <param name="product">The product<see cref="Product"/>.</param>
        /// <returns>The <see cref="ProductApiModel"/>.</returns>
        public ProductApiModel ProductDbToApiModel(Product product)
        {
            var productApiModel = new ProductApiModel
            {
                ProductId = product.ProductId,
                ProductGuid = product.ProductGuid,
                Name = product.Name,
                PrettyName = product.PrettyName
            };

            List<ArticleApiModel> articles = new List<ArticleApiModel>();

            foreach (Article article in product.Article)
            {
                articles.Add(ArticleDbToApiModel(article));
            }

            productApiModel.Article = articles;

            return productApiModel;
        }
    }
}

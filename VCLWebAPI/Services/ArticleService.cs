using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VCLWebAPI.Mappers.System;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.Systems;
using Newtonsoft.Json;

namespace VCLWebAPI.Services
{
    public class ArticleService : IDisposable
    {
        private VCLDesignDBEntities _db;

        public ArticleService()
        {
            _db = new VCLDesignDBEntities();
        }

        public string GetArticlesForSystem(string systemName)
        {
            string systemType = systemName.Substring(0, 3);
            string response = "";
            // for aws system
            if (String.Equals(systemType,"AWS", StringComparison.OrdinalIgnoreCase))
            {
                var systemMapper = new SystemMapper();
                Product product = _db.Product.Where(x => x.Name == systemName).SingleOrDefault();
                if (product == null)
                {
                    throw new InvalidDataException();
                }
                List<ArticleApiModel> articles = systemMapper.ProductDbToApiModel(product).Article;
                if (articles == null)
                {
                    throw new InvalidDataException();
                }
                response = JsonConvert.SerializeObject(articles);
            }
            else if (String.Equals(systemType, "FWS", StringComparison.OrdinalIgnoreCase))
            {
                List<FacadeArticle> facadeArticles = new List<FacadeArticle>();
                foreach(FacadeArticle facadeArticle in _db.FacadeArticle)
                {
                    if (String.Equals(facadeArticle.System, systemName, StringComparison.OrdinalIgnoreCase))
                    {
                        facadeArticles.Add(facadeArticle);
                    }
                }
                if (facadeArticles == null)
                {
                    throw new InvalidDataException();
                }
                List<FacadeArticleApiModel> articles = facadeArticles.Select(x => new FacadeArticleApiModel(x)).ToList();
                response = JsonConvert.SerializeObject(articles);
            }
            else if (String.Equals(systemType, "UDC", StringComparison.OrdinalIgnoreCase))
            {
                List<UDCArticle> udcArticles = _db.UDCArticle.ToList();
                if (udcArticles == null)
                {
                    throw new InvalidDataException();
                }
                List<UDCArticleApiModel> articles = udcArticles.Select(x => new UDCArticleApiModel(x)).ToList();
                response = JsonConvert.SerializeObject(articles);
            }
            else if (String.Equals(systemType, "ADS", StringComparison.OrdinalIgnoreCase))
            {
                List<ADSArticle> adsArticles = _db.ADSArticle.ToList();
                if (adsArticles == null)
                {
                    throw new InvalidDataException();
                }
                List<ADSArticleApiModel> articles = adsArticles.Select(x => new ADSArticleApiModel(x)).ToList();
                response = JsonConvert.SerializeObject(articles);
            }
            else if (String.Equals(systemType, "ASE", StringComparison.OrdinalIgnoreCase))
            {
                List<ASEArticle> aseArticle = _db.ASEArticle.ToList();
                if (aseArticle == null)
                {
                    throw new InvalidDataException();
                }
                List<ASEArticleApiModel> articles = aseArticle.Select(x => new ASEArticleApiModel(x)).ToList();
                response = JsonConvert.SerializeObject(articles);
            }
            return response;
        }

        public List<ArticleApiModel> GetOuterFramesForSystem(string systemName)
        {
            var systemMapper = new SystemMapper();
            Product product = _db.Product.Where(x => x.Name == systemName).SingleOrDefault();
            if (product == null)
            {
                throw new InvalidDataException();
            }
            ArticleType articleType = _db.ArticleType.Where(x => x.Name == "outer__frame").SingleOrDefault();
            List<Article> outerFrames = product.Article.Where(x => x.ArticleTypeId == articleType.ArticleTypeId).ToList();
            List<ArticleApiModel> articles = new List<ArticleApiModel>();
            foreach (Article outerFrame in outerFrames)
            {
                articles.Add(systemMapper.ArticleDbToApiModel(outerFrame));
            }
            return articles;
        }

        public List<ArticleApiModel> GetVentFramesForSystem(string systemName)
        {
            var systemMapper = new SystemMapper();
            Product product = _db.Product.Where(x => x.Name == systemName).SingleOrDefault();
            if (product == null)
            {
                throw new InvalidDataException();
            }
            ArticleType articleType = _db.ArticleType.Where(x => x.Name == "vent__frame").SingleOrDefault();
            List<Article> ventFrames = product.Article.Where(x => x.ArticleTypeId == articleType.ArticleTypeId).ToList();
            List<ArticleApiModel> articles = new List<ArticleApiModel>();
            foreach (Article ventFrame in ventFrames)
            {
                articles.Add(systemMapper.ArticleDbToApiModel(ventFrame));
            }
            return articles;
        }

        public List<FacadeInsertUnitApiModel> GetFacadeInsertUnit()
        {
            return _db.FacadeInsertUnit.ToList().Select(s => new FacadeInsertUnitApiModel(s)).ToList();
        }

        public List<FacadeProfileApiModel> GetFacadeProfile()
        {
            return _db.FacadeProfile.ToList().Select(s => new FacadeProfileApiModel(s)).ToList();
        }

        public List<FacadeSpacerApiModel> GetFacadeSpacer()
        {
            return _db.FacadeSpacer.ToList().Select(s => new FacadeSpacerApiModel(s)).ToList();
        }

        public List<ArticleApiModel> GetMullionTransomForSystem(string systemName)
        {
            var systemMapper = new SystemMapper();
            Product product = _db.Product.Where(x => x.Name == systemName).SingleOrDefault();
            if (product == null)
            {
                throw new InvalidDataException();
            }
            List<ArticleApiModel> articles = new List<ArticleApiModel>();
            ArticleType mullionArticleType = _db.ArticleType.Where(x => x.Name == "mullion").SingleOrDefault();
            if (mullionArticleType != null)
            {
                List<Article> mullions = product.Article.Where(x => x.ArticleTypeId == mullionArticleType.ArticleTypeId).ToList();
                foreach (Article mullion in mullions)
                {
                    articles.Add(systemMapper.ArticleDbToApiModel(mullion));
                }
            }
            ArticleType transomArticleType = _db.ArticleType.Where(x => x.Name == "transom").SingleOrDefault();
            List<Article> transoms = product.Article.Where(x => x.ArticleTypeId == transomArticleType.ArticleTypeId).ToList();
            foreach (Article transom in transoms)
            {
                articles.Add(systemMapper.ArticleDbToApiModel(transom));
            }
            return articles;
        }

        public ArticleApiModel GetArticleByName(string name)
        {
            Article article = _db.Article.Where(x => x.Name == name).SingleOrDefault();
            if (article == null)
            {
                throw new InvalidDataException();
            }
            var systemMapper = new SystemMapper();
            ArticleApiModel articleApiModel = systemMapper.ArticleDbToApiModel(article);
            return articleApiModel;
        }

        public List<InsulatingBar> GetInsulatingBarsForArticle()
        {
            List<InsulatingBar> InsulatingBarList = new List<InsulatingBar>();
            //foreach (InsulatingBar insulatingBar in _db.InsulatingBar)
            //{
            //  InsulatingBarList.Add(insulatingBar);
            //}
            return InsulatingBarList;
        }

        public List<DoorHandleHingeApiModel> GetDoorHandleHingeForSystem()
        {
            List<DoorHandleHinge> dhhArticles = _db.DoorHandleHinge.ToList();
            if (dhhArticles == null)
            {
                throw new InvalidDataException();
            }
            List<DoorHandleHingeApiModel> articles = dhhArticles.Select(x => new DoorHandleHingeApiModel(x)).ToList();
            //string response = JsonConvert.SerializeObject(articles);
            return articles;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ArticleService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
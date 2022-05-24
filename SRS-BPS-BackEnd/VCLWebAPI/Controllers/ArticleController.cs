using Newtonsoft.Json;
using System.IO;
//using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VCLWebAPI.Services;

namespace VCLWebAPI.Controllers
{

    /// <summary>
    /// Defines the <see cref="ArticleController" />.
    /// </summary>
    [Authorize]
    public class ArticleController : BaseController
    {
        /// <summary>
        /// Defines the _articleService.
        /// </summary>
        internal ArticleService _articleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleController"/> class.
        /// </summary>
        public ArticleController()
        {
            _articleService = new ArticleService();
        }

        /// <summary>
        /// The GetArticleByName.
        /// </summary>
        /// <param name="name">The name<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("api/Article/GetArticleByName/{name}")]
        public string GetArticleByName(string name)
        {
            var article = _articleService.GetArticleByName(name);
            if (article == null)
            {
                throw new InvalidDataException();
            }
            var response = JsonConvert.SerializeObject(article);
            return response;
        }

        /// <summary>
        /// The GetArticlesForSystem.
        /// </summary>
        /// <param name="systemName">The systemName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("api/Article/GetArticlesForSystem/{systemName}")]
        public string GetArticlesForSystem(string systemName)
        {
            string response = _articleService.GetArticlesForSystem(systemName);
            return response;
        }

        /// <summary>
        /// The GetFacadeInsertUnit.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        public string GetFacadeInsertUnit()
        {
            var articles = _articleService.GetFacadeInsertUnit();
            var response = JsonConvert.SerializeObject(articles);
            return response;
        }

        /// <summary>
        /// The GetFacadeProfile.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        public string GetFacadeProfile()
        {
            var articles = _articleService.GetFacadeProfile();
            var response = JsonConvert.SerializeObject(articles);
            return response;
        }

        /// <summary>
        /// The GetFacadeSpacer.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        public string GetFacadeSpacer()
        {
            var articles = _articleService.GetFacadeSpacer();
            var response = JsonConvert.SerializeObject(articles);
            return response;
        }

        /// <summary>
        /// The GetInsulatingBarsForArticle.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("api/Article/GetInsulatingBarsForArticle/")]
        public string GetInsulatingBarsForArticle()
        {
            var insulatingBarList = _articleService.GetInsulatingBarsForArticle();
            var response = JsonConvert.SerializeObject(insulatingBarList);
            return response;
        }

        /// <summary>
        /// The GetMullionTransomForSystem.
        /// </summary>
        /// <param name="systemName">The systemName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("api/Article/GetMullionTransomForSystem/{systemName}")]
        public string GetMullionTransomForSystem(string systemName)
        {
            var articles = _articleService.GetMullionTransomForSystem(systemName);
            if (articles == null)
            {
                throw new InvalidDataException();
            }
            var response = JsonConvert.SerializeObject(articles);
            return response;
        }

        /// <summary>
        /// The GetOuterFramesForSystem.
        /// </summary>
        /// <param name="systemName">The systemName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("api/Article/GetOuterFramesForSystem/{systemName}")]
        public string GetOuterFramesForSystem(string systemName)
        {
            var articles = _articleService.GetOuterFramesForSystem(systemName);
            if (articles == null)
            {
                throw new InvalidDataException();
            }
            var response = JsonConvert.SerializeObject(articles);
            return response;
        }

        /// <summary>
        /// The GetVentFramesForSystem.
        /// </summary>
        /// <param name="systemName">The systemName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("api/Article/GetVentFramesForSystem/{systemName}")]
        public string GetVentFramesForSystem(string systemName)
        {
            var articles = _articleService.GetVentFramesForSystem(systemName);
            if (articles == null)
            {
                throw new InvalidDataException();
            }
            var response = JsonConvert.SerializeObject(articles);
            return response;
        }

        /// <summary>
        /// The GetDoorHandleHingeForSystem.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("api/Article/GetDoorHandleHingeForSystem/")]
        public string GetDoorHandleHingeForSystem()
        {
            string response = _articleService.GetDoorHandleHingeForSystem();
            return response;
        }
    }
}

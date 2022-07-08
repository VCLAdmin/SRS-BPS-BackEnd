using Newtonsoft.Json;
using System.IO;
//using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VCLWebAPI.Services;

namespace VCLWebAPI.Controllers
{

    /// <summary>
    /// Defines the <see cref="GlassController" />.
    /// </summary>
    [Authorize]
    [Route("api/Glass")]
    public class GlassController : BaseController
    {
        /// <summary>
        /// Defines the _articleService.
        /// </summary>
        internal GlassService _glassService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleController"/> class.
        /// </summary>
        public GlassController()
        {
            _glassService = new GlassService();
        }

        /// <summary>
        /// The GetArticleByName.
        /// </summary>
        /// <param name="applicationName">The name<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("GetGlassInfo/{applicationName}")]
        public string GetGlassInfo(string applicationName)
        {
            string response = _glassService.GetGlassInfo(applicationName);
            return response;
        }
    }
}

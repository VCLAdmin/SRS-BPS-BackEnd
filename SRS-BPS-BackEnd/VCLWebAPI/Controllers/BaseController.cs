//using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using log4net;

namespace VCLWebAPI.Controllers
{

    /// <summary>
    /// Defines the <see cref="BaseController" />.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public static readonly ILog log = log4net.LogManager.GetLogger(nameof(BaseController));
    }
}

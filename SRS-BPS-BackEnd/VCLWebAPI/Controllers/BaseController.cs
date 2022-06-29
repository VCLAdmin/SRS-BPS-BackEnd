﻿//using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VCLWebAPI.Controllers
{

    /// <summary>
    /// Defines the <see cref="BaseController" />.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
    }
}

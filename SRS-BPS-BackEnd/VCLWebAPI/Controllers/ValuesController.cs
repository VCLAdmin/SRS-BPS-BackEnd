using System.Collections.Generic;
//using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="ValuesController" />.
    /// </summary>
    [Authorize]
    public class ValuesController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// The Delete.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        public void Delete(int id)
        {
        }

        /// <summary>
        /// The Get.
        /// </summary>
        /// <returns>The <see cref="IEnumerable{string}"/>.</returns>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// The Post.
        /// </summary>
        /// <param name="value">The value<see cref="string"/>.</param>
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// The Put.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <param name="value">The value<see cref="string"/>.</param>
        public void Put(int id, [FromBody] string value)
        {
        }
    }
}

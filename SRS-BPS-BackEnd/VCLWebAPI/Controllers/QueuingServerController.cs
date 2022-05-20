using System;
using System.Threading.Tasks;
using System.Web.Http;
using VCLWebAPI.Services;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="QueuingServerController" />.
    /// </summary>
    [Authorize]
    public class QueuingServerController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// Defines the qss.
        /// </summary>
        private readonly QueuingServerService qss;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuingServerController"/> class.
        /// </summary>
        public QueuingServerController()
        {
            qss = new QueuingServerService();
        }

        // GET: QueuingServer
        /// <summary>
        /// The ConversionCompleted.
        /// </summary>
        /// <param name="orderId">The orderId<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet]
        [Route("api/QueuingServer/ConversionCompleted/{orderId}")]
        public string ConversionCompleted(string orderId)
        {
            try
            {
                var id = Guid.Parse(orderId);
                qss.ConversionCompleted(id);
                return "";
            }
            catch (Exception ex)
            {
                return "error";
                //throw;
            }
        }

        /// <summary>
        /// The SnsNewMessage.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        [HttpPost]
        public async Task<string> SnsNewMessage(string id = "")
        {
            return await qss.SnsNewMessage(Request, id);
        }

        /// <summary>
        /// The SnsToPriorityQueue.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        [HttpPost]
        public async Task<string> SnsToPriorityQueue(string id = "")
        {
            return await qss.SnsNewMessage(Request, id);
        }
    }
}

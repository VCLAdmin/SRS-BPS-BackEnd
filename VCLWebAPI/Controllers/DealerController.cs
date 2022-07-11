using System;
using System.Collections.Generic;
//using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VCLWebAPI.Models.SRS;
using VCLWebAPI.Services;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="DealerController" />.
    /// </summary>
    [Authorize]
    [Route("api/Dealer")]
    public class DealerController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// Defines the _dealerService.
        /// </summary>
        private readonly DealerService _dealerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DealerController"/> class.
        /// </summary>
        public DealerController()
        {
            _dealerService = new DealerService();
        }

        /// <summary>
        /// The CanDelete.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [HttpGet]
        [Route("CanDelete/{id}")]
        public bool CanDelete(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            return _dealerService.CanDelete(id);
        }

        // DELETE: api/Fabricator/5
        /// <summary>
        /// The Delete.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="List{DealerApiModel}"/>.</returns>
        [HttpDelete]
        [Route("Delete/{id}")]
        public List<DealerApiModel> Delete(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            _dealerService.Delete(id);
            return _dealerService.GetAll();
        }

        // GET: api/Fabricator
        /// <summary>
        /// The Get.
        /// </summary>
        /// <returns>The <see cref="List{DealerApiModel}"/>.</returns>
        [HttpGet]
        [Route("Get")]
        public List<DealerApiModel> Get()
        {
            //return new string[] { "value1", "value2" };
            return _dealerService.GetAll();
        }

        // GET: api/Fabricator/5
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="DealerApiModel"/>.</returns>
        [HttpGet]
        [Route("Get/{id}")]
        public DealerApiModel Get(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            return _dealerService.Get(id);
        }

        /// <summary>
        /// The GetUserDealer.
        /// </summary>
        /// <returns>The <see cref="DealerApiModel"/>.</returns>
        [HttpGet]
        [Route("GetUserDealer")]
        public DealerApiModel GetUserDealer()
        {
            //Guid extId = Guid.Parse(guid);
            return _dealerService.GetUserDealer();
        }

        // POST: api/Fabricator
        /// <summary>
        /// The Post.
        /// </summary>
        /// <param name="fab">The fab<see cref="DealerApiModel"/>.</param>
        /// <returns>The <see cref="List{DealerApiModel}"/>.</returns>
        [HttpPost]
        [Route("Post")]
        public List<DealerApiModel> Post([FromBody] DealerApiModel fab)
        {
            _dealerService.Create(fab);
            return _dealerService.GetAll();
        }

        // PUT: api/Fabricator/5
        /// <summary>
        /// The Put.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <param name="fab">The fab<see cref="DealerApiModel"/>.</param>
        /// <returns>The <see cref="List{DealerApiModel}"/>.</returns>
        [HttpPut]
        [Route("Put/{id}")]
        public List<DealerApiModel> Put(Guid id, [FromBody] DealerApiModel fab)
        {
            _dealerService.Update(id, fab);
            return _dealerService.GetAll();
        }
    }
}

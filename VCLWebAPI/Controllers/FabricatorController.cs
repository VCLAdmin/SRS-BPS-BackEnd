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
    /// Defines the <see cref="FabricatorController" />.
    /// </summary>
    [Authorize]
    [Route("api/Fabricator")]
    public class FabricatorController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// Defines the _fabricatorService.
        /// </summary>
        private readonly FabricatorService _fabricatorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricatorController"/> class.
        /// </summary>
        public FabricatorController()
        {
            _fabricatorService = new FabricatorService();
        }

        /// <summary>
        /// The CanDelete.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [HttpGet]
        [Route("CanDelete")]
        public bool CanDelete(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            return _fabricatorService.CanDelete(id);
        }

        // DELETE: api/Fabricator/5
        /// <summary>
        /// The Delete.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="List{FabricatorApiModel}"/>.</returns>
        [HttpDelete]
        [Route("Delete")]
        public List<FabricatorApiModel> Delete(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            _fabricatorService.Delete(id);
            return _fabricatorService.GetAll();
        }

        // GET: api/Fabricator
        /// <summary>
        /// The Get.
        /// </summary>
        /// <returns>The <see cref="List{FabricatorApiModel}"/>.</returns>
        [HttpGet]
        [Route("Get")]
        public List<FabricatorApiModel> Get()
        {
            //return new string[] { "value1", "value2" };
            return _fabricatorService.GetAll();
        }

        // GET: api/Fabricator/5
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="FabricatorApiModel"/>.</returns>
        [HttpGet]
        [Route("Get/{id}")]
        public FabricatorApiModel Get(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            return _fabricatorService.Get(id);
        }

        // POST: api/Fabricator
        /// <summary>
        /// The Post.
        /// </summary>
        /// <param name="fab">The fab<see cref="FabricatorApiModel"/>.</param>
        /// <returns>The <see cref="List{FabricatorApiModel}"/>.</returns>
        [HttpPost]
        [Route("Post")]
        public List<FabricatorApiModel> Post([FromBody] FabricatorApiModel fab)
        {
            _fabricatorService.Create(fab);
            return _fabricatorService.GetAll();
        }

        // PUT: api/Fabricator/5
        /// <summary>
        /// The Put.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <param name="fab">The fab<see cref="FabricatorApiModel"/>.</param>
        /// <returns>The <see cref="List{FabricatorApiModel}"/>.</returns>
        [HttpPut]
        [Route("Put")]
        public List<FabricatorApiModel> Put(Guid id, [FromBody] FabricatorApiModel fab)
        {
            _fabricatorService.Update(id, fab);
            return _fabricatorService.GetAll();
        }

        /// <summary>
        /// The ValidateUpdate.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <param name="fab">The fab<see cref="FabricatorApiModel"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpPut]
        [Route("ValidateUpdate")]
        public string ValidateUpdate(Guid id, [FromBody] FabricatorApiModel fab)
        {
            return _fabricatorService.ValidateUpdate(id, fab);
        }
    }
}

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
    /// Defines the <see cref="FinancialController" />.
    /// </summary>
    [Authorize]
    [Route("api/Financial")]
    public class FinancialController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// Defines the _dealerService.
        /// </summary>
        private readonly DealerService _dealerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FinancialController"/> class.
        /// </summary>
        public FinancialController()
        {
            _dealerService = new DealerService();
        }

        // GET: api/Fabricator
        /// <summary>
        /// The Get.
        /// </summary>
        /// <returns>The <see cref="List{FinancialApiModel}"/>.</returns>
        [HttpGet]
        [Route("Get")]
        public List<FinancialApiModel> Get()
        {
            //return new string[] { "value1", "value2" };
            return _dealerService.GetFinancials();
        }

        // GET: api/Fabricator/5
        /// <summary>
        /// The Get.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="FinancialApiModel"/>.</returns>
        [HttpGet]
        [Route("Get/{id}")]
        public FinancialApiModel Get(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            return _dealerService.GetFinance(id);
        }

        // PUT: api/Fabricator/5
        /// <summary>
        /// The Put.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <param name="fin">The fin<see cref="FinancialApiModel"/>.</param>
        /// <returns>The <see cref="List{FinancialApiModel}"/>.</returns>
        [HttpPut]
        [Route("Put/{id}")]
        public List<FinancialApiModel> Put(Guid id, [FromBody] FinancialApiModel fin)
        {
            _dealerService.UpdateFinancial(id, fin);
            return _dealerService.GetFinancials();
        }
    }
}

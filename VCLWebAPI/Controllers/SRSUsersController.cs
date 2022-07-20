using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VCLWebAPI.Models;
using VCLWebAPI.Services;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="SRSUsersController" />.
    /// </summary>
    [Authorize]
    [Route("api/SRSUsers")]
    public class SRSUsersController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        /// <summary>
        /// Defines the _srsuserService.
        /// </summary>
        private readonly SRSUserService _srsuserService;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SRSUsersController"/> class.
        /// </summary>
        public SRSUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _srsuserService = new SRSUserService(_userManager);
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
            return _srsuserService.CanDelete(id);
        }

        /// <summary>
        /// The ChangePassword.
        /// </summary>
        /// <param name="usr">The usr<see cref="SRSUserApiModel"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<string> ChangePassword([FromBody] SRSUserApiModel usr)
        {
            return await _srsuserService.ChangePassword(usr);
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="Task{List{SRSUserApiModel}}"/>.</returns>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<List<SRSUserApiModel>> Delete(Guid id)
        {
            //Guid extId = Guid.Parse(guid);
            await _srsuserService.Delete(id);
            return await _srsuserService.GetAll();
        }

        /// <summary>
        /// The Get.
        /// </summary>
        /// <returns>The <see cref="Task{List{SRSUserApiModel}}"/>.</returns>
        [HttpGet]
        [Route("Get")]
        public async Task<List<SRSUserApiModel>> Get()
        {
            //return new string[] { "value1", "value2" };
            return await _srsuserService.GetAll();
        }

        /// <summary>
        /// The GetEmail.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <returns>The <see cref="Task{string}"/>.</returns>
        [HttpGet]
        [Route("GetEmail/{id}")]
        public async Task<string> GetEmail(Guid id)
        {
            return _srsuserService.GetEmail(id);
        }

        /// <summary>
        /// The IsEmailDuplicate.
        /// </summary>
        /// <param name="email">The email<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        [HttpGet]
        [Route("IsEmailDuplicate")]
        public async Task<bool> IsEmailDuplicate(string email)
        {
            return _srsuserService.IsEmailDuplicate(email);
        }

        //[HttpGet]
        //public SRSUserApiModel Get(Guid id)
        //{
        //    //Guid extId = Guid.Parse(guid);
        //    return _fabricatorService.Get(id);
        //}
        /// <summary>
        /// The Post.
        /// </summary>
        /// <param name="usr">The usr<see cref="SRSUserApiModel"/>.</param>
        /// <returns>The <see cref="Task{List{SRSUserApiModel}}"/>.</returns>
        [HttpPost]
        [Route("Post")]
        public async Task<List<SRSUserApiModel>> Post([FromBody] SRSUserApiModel usr)
        {
            await _srsuserService.Create(usr);
            return await _srsuserService.GetAll();
        }

        /// <summary>
        /// The Put.
        /// </summary>
        /// <param name="id">The id<see cref="Guid"/>.</param>
        /// <param name="fab">The fab<see cref="SRSUserApiModel"/>.</param>
        /// <returns>The <see cref="Task{List{SRSUserApiModel}}"/>.</returns>
        [HttpPut]
        [Route("Put/{id}")]
        public async Task<List<SRSUserApiModel>> Put(Guid id, [FromBody] SRSUserApiModel fab)
        {
            await _srsuserService.Update(id, fab);
            return await _srsuserService.GetAll();
        }
    }
}

using System;
using System.IO;
using System.Linq;
//using System.Web;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VCLWebAPI.Models;
using VCLWebAPI.Models.Account;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Services;
using VCLWebAPI;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="AccountPreviousController" />.
    /// </summary>
    [Authorize]
    [Route("api/AccountPrevious")]
    public class AccountPreviousController : BaseController
    {
        /// <summary>
        /// Defines the _accountService.
        /// </summary>
        private readonly AccountService _accountService;

        /// <summary>
        /// Defines the _db.
        /// </summary>
        private readonly VCLDesignDBEntities _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountPreviousController"/> class.
        /// </summary>
        public AccountPreviousController()
        {
            _db = new VCLDesignDBEntities();
            _accountService = new AccountService();
        }

        /// <summary>
        /// The GetLanguage.
        /// </summary>
        /// <param name="userName">The userName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetLanguage(string userName)
        {
            return _accountService.GetLanguage(userName);
        }

        /// <summary>
        /// The GetVersionInformation.
        /// </summary>
        /// <returns>The <see cref="VersionInformationApiModel"/>.</returns>
        [HttpGet]
        [Route("GetVersionInformation")]
        [AllowAnonymous]
        public VersionInformationApiModel GetVersionInformation()
        {
            var fileInfo = new FileInfo(GetType().Assembly.Location);
            int days = (DateTime.Today - fileInfo.LastWriteTimeUtc).Days;
            string deployedInfo = "";
            if (days == 0)
                deployedInfo = "Today";
            else if (days == 1)
                deployedInfo = days + " day ago";
            else if (days < 7)
                deployedInfo = days + " days ago";
            else if (days == 7 || (int)days / 7 == 1)
                deployedInfo = (int)days / 7 + " week ago";
            else if (days < 30)
                deployedInfo = (int)days / 7 + " weeks ago";
            else if (days == 30 && (int)days / 30 == 1)
                deployedInfo = (int)days / 30 + " month ago";
            else if (days >= 30)
                deployedInfo = (int)days / 30 + " months ago";

            //var user = UserService.GetUser(_dbContext, ApiUtil.GetActiveUserExternalId());
            var versionModel = new VersionInformationApiModel
            {
                VersionNumber = "3.7.0",
                BuildNumber = this.GetType().Assembly.GetName().Version.Build.ToString(),
                DeployedDateInfo = deployedInfo,
                Date = fileInfo.LastWriteTimeUtc
                //Date = fileInfo.LastWriteTimeUtc.ToUserTimeZone(user.TimeZone)
            };
            return versionModel;
        }

        /// <summary>
        /// The HasUsers.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool HasUsers()
        {
            return _db.User.Any();
        }

        /// <summary>
        /// The SaltAndHashNewUsers.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int SaltAndHashNewUsers()
        {
            return _accountService.SaltAndHashNewUsers();
        }

        /// <summary>
        /// The SetLanguage.
        /// </summary>
        /// <param name="accountApiModel">The accountApiModel<see cref="AccountApiModel"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string SetLanguage(AccountApiModel accountApiModel)
        {
            return _accountService.SetLanguage(accountApiModel);
        }

        /// <summary>
        /// The SignIn.
        /// </summary>
        /// <param name="accountApiModel">The accountApiModel<see cref="AccountApiModel"/>.</param>
        /// <returns>The <see cref="UserApiModel"/>.</returns>
        [HttpPost]
        [Route("SignIn")]
        public async System.Threading.Tasks.Task<UserApiModel> SignIn([FromBody]AccountApiModel accountApiModel)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }
            return await _accountService.SignIn(accountApiModel);
        }

        /// <summary>
        /// The ValidateHash.
        /// </summary>
        /// <param name="accountApiModel">The accountApiModel<see cref="AccountApiModel"/>.</param>
        /// <returns>The <see cref="Boolean"/>.</returns>
        public Boolean ValidateHash(AccountApiModel accountApiModel)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }
            return _accountService.ValidateHash(accountApiModel.User, accountApiModel.Password);
        }

        /// <summary>
        /// The SaltAndHashAllUsers.
        /// </summary>
        private void SaltAndHashAllUsers()
        {
            _accountService.SaltAndHashAllUsers();
        }

        /// <summary>
        /// The SaltAndHashUser.
        /// </summary>
        /// <param name="accountApiModel">The accountApiModel<see cref="AccountApiModel"/>.</param>
        private void SaltAndHashUser(AccountApiModel accountApiModel) //string username, string password
        {
            _accountService.SaltAndHashUser(accountApiModel.UserName, accountApiModel.Password);
        }
    }
}

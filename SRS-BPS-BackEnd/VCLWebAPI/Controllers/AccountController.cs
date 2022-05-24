﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Owin;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.Cookies;
//using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;
using VCLWebAPI.Exceptions;
using VCLWebAPI.Models;
using VCLWebAPI.Providers;
using VCLWebAPI.Results;
using VCLWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace VCLWebAPI.Controllers
{

    /// <summary>
    /// Defines the <see cref="AccountController" />.
    /// </summary>
    [Authorize]
    [Route("api/Account")]
    public class AccountController : BaseController
    {
        /// <summary>
        /// Defines the LocalLoginProvider.
        /// Defines the _accountService.
        /// Defines the _userManager.
        /// </summary>
        private const string LocalLoginProvider = "Local";
        private AccountService _accountService;
        private ApplicationUserManager _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController()
        {
            _accountService = new AccountService();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userManager">The userManager<see cref="ApplicationUserManager"/>.</param>
        /// <param name="accessTokenFormat">The accessTokenFormat<see cref="ISecureDataFormat{AuthenticationTicket}"/>.</param>
        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        /// <summary>
        /// Gets the AccessTokenFormat.
        /// </summary>
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        /// <summary>
        /// Gets the UserManager.
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Gets the Authentication.
        /// </summary>
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        /// <summary>
        /// The AddExternalLogin.
        /// </summary>
        /// <param name="model">The model<see cref="AddExternalLoginBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("AddExternalLogin")]
        public async Task<IActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        /// <summary>
        /// The ChangePassword.
        /// </summary>
        /// <param name="model">The model<see cref="ChangePasswordBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        /// <summary>
        /// The CreateContact.
        /// </summary>
        /// <param name="model">The model<see cref="CreateContactBindingModel"/>.</param>
        /// <returns>The <see cref="Task{CreateContactBindingModel}"/>.</returns>
        [Route("CreateContact")]
        public async Task<CreateContactBindingModel> CreateContact(CreateContactBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }
            return _accountService.CreateContact(model);
        }

        [HttpGet]
        [Route("SetCurrentUserLanguage/{language}")]
        public string SetCurrentUserLanguage(string language)
        {
            return _accountService.SetCurrentUserLanguage(language);
        }

        [HttpGet]
        [Route("GetCurrentUserLanguage")]
        public string GetCurrentUserLanguage()
        {
            return _accountService.GetCurrentUserLanguage();
        }

        /// <summary>
        /// The GetContact.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{CreateContactBindingModel}"/>.</returns>
        [Route("GetContact")]
        public async Task<CreateContactBindingModel> GetContact(int id)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }
            return _accountService.GetContact(id);
        }

        /// <summary>
        /// The GetContactList.
        /// </summary>
        /// <returns>The <see cref="Task{List{CreateContactBindingModel}}"/>.</returns>
        [Route("GetContactList")]
        public async Task<List<CreateContactBindingModel>> GetContactList()
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }
            return _accountService.ContactList();
        }

        /// <summary>
        /// The GetExternalLogin.
        /// </summary>
        /// <param name="provider">The provider<see cref="string"/>.</param>
        /// <param name="error">The error<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        /// <summary>
        /// The GetExternalLogins.
        /// </summary>
        /// <param name="returnUrl">The returnUrl<see cref="string"/>.</param>
        /// <param name="generateState">The generateState<see cref="bool"/>.</param>
        /// <returns>The <see cref="IEnumerable{ExternalLoginViewModel}"/>.</returns>
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        /// <summary>
        /// The GetManageInfo.
        /// </summary>
        /// <param name="returnUrl">The returnUrl<see cref="string"/>.</param>
        /// <param name="generateState">The generateState<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{ManageInfoViewModel}"/>.</returns>
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                throw new NotFoundException();
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        /// <summary>
        /// The GetUserInfo.
        /// </summary>
        /// <returns>The <see cref="UserInfoViewModel"/>.</returns>
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        /// <summary>
        /// The Logout.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Logout")]
        public IActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        /// <summary>
        /// The Register.
        /// </summary>
        /// <param name="model">The model<see cref="RegisterBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            IdentityResult result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        /// <summary>
        /// The RegisterExternal.
        /// </summary>
        /// <param name="model">The model<see cref="RegisterExternalBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }
            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        /// <summary>
        /// The RemoveLogin.
        /// </summary>
        /// <param name="model">The model<see cref="RemoveLoginBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("RemoveLogin")]
        public async Task<IActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        /// <summary>
        /// The SetPassword.
        /// </summary>
        /// <param name="model">The model<see cref="SetPasswordBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [Route("SetPassword")]
        public async Task<IActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        /// <summary>
        /// The UpdateContact.
        /// </summary>
        /// <param name="model">The model<see cref="CreateContactBindingModel"/>.</param>
        /// <returns>The <see cref="Task{CreateContactBindingModel}"/>.</returns>
        [Route("UpdateContact")]
        public async Task<CreateContactBindingModel> UpdateContact(CreateContactBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }
            return await _accountService.UpdateContact(model);
        }

        /// <summary>
        /// The Dispose.
        /// </summary>
        /// <param name="disposing">The disposing<see cref="bool"/>.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// The GetErrorResult.
        /// </summary>
        /// <param name="result">The result<see cref="IdentityResult"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        /// <summary>
        /// Defines the <see cref="ExternalLoginData" />.
        /// </summary>
        private class ExternalLoginData
        {
            /// <summary>
            /// Gets or sets the LoginProvider.
            /// </summary>
            public string LoginProvider { get; set; }

            /// <summary>
            /// Gets or sets the ProviderKey.
            /// </summary>
            public string ProviderKey { get; set; }

            /// <summary>
            /// Gets or sets the UserName.
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// The FromIdentity.
            /// </summary>
            /// <param name="identity">The identity<see cref="ClaimsIdentity"/>.</param>
            /// <returns>The <see cref="ExternalLoginData"/>.</returns>
            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }

            /// <summary>
            /// The GetClaims.
            /// </summary>
            /// <returns>The <see cref="IList{Claim}"/>.</returns>
            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }
        }

        /// <summary>
        /// Defines the <see cref="RandomOAuthStateGenerator" />.
        /// </summary>
        private static class RandomOAuthStateGenerator
        {
            /// <summary>
            /// Defines the _random.
            /// </summary>
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            /// <summary>
            /// The Generate.
            /// </summary>
            /// <param name="strengthInBits">The strengthInBits<see cref="int"/>.</param>
            /// <returns>The <see cref="string"/>.</returns>
            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }
    }
}

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
using VCLWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using VCLWebAPI.Utils;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Web;
//using Microsoft.AspNetCore.Http.Authentication;

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
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = new AccountService();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userManager">The userManager<see cref="ApplicationUserManager"/>.</param>
        /// <param name="accessTokenFormat">The accessTokenFormat<see cref="ISecureDataFormat{AuthenticationTicket}"/>.</param>
        public AccountController(UserManager<IdentityUser> userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            _userManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        /// <summary>
        /// Gets the AccessTokenFormat.
        /// </summary>
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        /// <summary>
        /// Gets the UserManager.
        /// </summary>
        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        /// <summary>
        /// Gets the Authentication.
        /// </summary>
        //private IAuthenticationManager Authentication
        //{
        //    get { return Request.GetOwinContext().Authentication; }
        //}

        private async Task<IdentityUser> GetUser()
        {
            return await _userManager.FindByIdAsync(User.GetLoggedInUserId<string>());
        }

        /// <summary>
        /// The AddExternalLogin.
        /// </summary>
        /// <param name="model">The model<see cref="AddExternalLoginBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        //[Route("AddExternalLogin")]
        //public async Task<IActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new InvalidModelException();
        //    }

        //    //Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    await HttpContext.SignOutAsync();

        //    AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

        //    //if (ticket == null || ticket.Identity == null || (ticket.Properties != null
        //    if (ticket == null || ticket.Principal == null || (ticket.Properties != null
        //        && ticket.Properties.ExpiresUtc.HasValue
        //        && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
        //    {
        //        return BadRequest("External login failure.");
        //    }

        //    //ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket..Identity);
        //    ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Principal);

        //    if (externalData == null)
        //    {
        //        return BadRequest("The external login is already associated with an account.");
        //    }

        //    IdentityResult result = await _userManager.AddLoginAsync(await GetUser(),
        //        new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey, displayName);

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }

        //    return Ok();
        //}

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

            IdentityResult result = await _userManager.ChangePasswordAsync(await GetUser(), model.OldPassword,
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
            return await _accountService.ContactList();
        }

        /// <summary>
        /// The GetExternalLogin.
        /// </summary>
        /// <param name="provider">The provider<see cref="string"/>.</param>
        /// <param name="error">The error<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [System.Web.Mvc.OverrideAuthentication]
        [System.Web.Http.HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie)]
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
                return new ChallengeResult(provider); //return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsPrincipal);

            if (externalLogin == null)
            {
                //return InternalServerError();
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (externalLogin.LoginProvider != provider)
            {
                //Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                await HttpContext.SignOutAsync();
                return new ChallengeResult(provider);//return new ChallengeResult(provider, this);
            }

            //ApplicationUser user = await _userManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
            //    externalLogin.ProviderKey));

            IdentityUser user = await _userManager.FindByLoginAsync(externalLogin.LoginProvider,
                externalLogin.ProviderKey);

            //bool hasRegistered = user != null;

            //if (hasRegistered)
            //{
            //    //Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //    await HttpContext.SignOutAsync();

            //    //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(_userManager,
            //    //   OAuthDefaults.AuthenticationType);
            //    //ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(_userManager,
            //    //    CookieAuthenticationDefaults.AuthenticationType);

            //    ClaimsIdentity oAuthIdentity = await _userManager.CreateAsync(user, OAuthDefaults.AuthenticationType);
            //    ClaimsIdentity cookieIdentity = await _userManager.CreateAsync(user, CookieAuthenticationDefaults.AuthenticationType);

            //    AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
            //    //Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            //}
            //else
            //{
            //    IEnumerable<Claim> claims = externalLogin.GetClaims();
            //    ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
            //    //Authentication.SignIn(identity);
            //}
            await HttpContext.SignInAsync(User);

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        /// <summary>
        /// The GetExternalLogins.
        /// </summary>
        /// <param name="returnUrl">The returnUrl<see cref="string"/>.</param>
        /// <param name="generateState">The generateState<see cref="bool"/>.</param>
        /// <returns>The <see cref="IEnumerable{ExternalLoginViewModel}"/>.</returns>
        //[AllowAnonymous]
        //[Route("ExternalLogins")]
        //public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        //{
        //    IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
        //    //IEnumerable<AuthenticationDescription> descriptions = HttpContext.GetType();
        //    List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

        //    string state;

        //    if (generateState)
        //    {
        //        const int strengthInBits = 256;
        //        state = RandomOAuthStateGenerator.Generate(strengthInBits);
        //    }
        //    else
        //    {
        //        state = null;
        //    }

        //    foreach (AuthenticationDescription description in descriptions)
        //    {
        //        ExternalLoginViewModel login = new ExternalLoginViewModel
        //        {
        //            Name = description.Caption,
        //            Url = Url.Route("ExternalLogin", new
        //            {
        //                provider = description.AuthenticationType,
        //                response_type = "token",
        //                client_id = Startup.PublicClientId,
        //                redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
        //                state = state
        //            }),
        //            State = state
        //        };
        //        logins.Add(login);
        //    }

        //    return logins;
        //}

        /// <summary>
        /// The GetManageInfo.
        /// </summary>
        /// <param name="returnUrl">The returnUrl<see cref="string"/>.</param>
        /// <param name="generateState">The generateState<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{ManageInfoViewModel}"/>.</returns>
        //[Route("ManageInfo")]
        //public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        //{
        //    IdentityUser user = await GetUser();

        //    if (user == null)
        //    {
        //        throw new NotFoundException();
        //    }

        //    List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

        //    foreach (IdentityUserLogin linkedAccount in user..Logins)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = linkedAccount.LoginProvider,
        //            ProviderKey = linkedAccount.ProviderKey
        //        });
        //    }

        //    if (user.PasswordHash != null)
        //    {
        //        logins.Add(new UserLoginInfoViewModel
        //        {
        //            LoginProvider = LocalLoginProvider,
        //            ProviderKey = user.UserName,
        //        });
        //    }

        //    return new ManageInfoViewModel
        //    {
        //        LocalLoginProvider = LocalLoginProvider,
        //        Email = user.UserName,
        //        Logins = logins,
        //        ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
        //    };
        //}

        /// <summary>
        /// The GetUserInfo.
        /// </summary>
        /// <returns>The <see cref="UserInfoViewModel"/>.</returns>
        [System.Web.Http.HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            //ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsPrincipal);

            return new UserInfoViewModel
            {
                Email = User.GetLoggedInUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        /// <summary>
        /// The Logout.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            //Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            await HttpContext.SignOutAsync();
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
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
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
        [System.Web.Mvc.OverrideAuthentication]
        [System.Web.Http.HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }
            //var info = await Authentication.GetExternalLoginInfoAsync();
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                //return InternalServerError();
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await _userManager.AddLoginAsync(user, info);
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
                result = await _userManager.RemovePasswordAsync(await GetUser());
            }
            else
            {
                result = await _userManager.RemoveLoginAsync(await GetUser(), model.LoginProvider, model.ProviderKey);
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

            IdentityResult result = await _userManager.AddPasswordAsync(await GetUser(), model.NewPassword);

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
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _userManager != null)
        //    {
        //        _userManager.Dispose();
        //        _userManager = null;
        //    }

        //    base.Dispose(disposing);
        //}

        /// <summary>
        /// The GetErrorResult.
        /// </summary>
        /// <param name="result">The result<see cref="IdentityResult"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                //return InternalServerError();
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.ToString());
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
            //public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            public static ExternalLoginData FromIdentity(ClaimsPrincipal identity) // ClaimsIdentity identity)
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
                return HttpUtility.UrlEncode(data);//return HttpServerUtility.UrlTokenEncode(data);
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VCLWebAPI.Exceptions;
using VCLWebAPI.Models;
using VCLWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using VCLWebAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using VCLWebAPI.Models.Account;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VCLWebAPI.Models.Edmx;
using System.Linq;
using Newtonsoft.Json;
using VCLWebAPI.Mappers;

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
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _accountService = new AccountService(_userManager, _roleManager);
            _configuration = configuration;
        }

        // this function will generate the new token
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var dbContext = new VCLDesignDBEntities();
            if (!string.IsNullOrEmpty(model.UserName) &&
                !string.IsNullOrEmpty(model.Password))
            {
                //SaltAndHashAllUsers();
                var user = dbContext.User
                    .SingleOrDefault(x => x.Email.Equals(model.UserName, StringComparison.CurrentCultureIgnoreCase));
                //
                var userMan = await _userManager.FindByEmailAsync(model.UserName);
                if (userMan != null && await _userManager.CheckPasswordAsync(userMan, model.Password)) 
                {
                    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim("Language", user.Language)
                        };

                    // additional laims needs to be added

                    //foreach (var userRole in userRoles)
                    //{
                    //    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    //}

                    var token = GetToken(authClaims);


                    return Ok(new
                    {
                        access_token = new JwtSecurityTokenHandler().WriteToken(token),
                        expires_in = token.ValidTo
                    });
                }
                else
                {
                    return Unauthorized();
                }

            }
            return Unauthorized();
        }

        private void SaltAndHashAllUsers()
        {
            _accountService.SaltAndHashAllUsers();
        }

        internal Boolean ValidateHash(User user, string password)
        {
            UserApiModel userApiModel = new UserApiModel();
            userApiModel = new UserMapper().MapUserDbModelToApiModel(user);


            string hashedPassword = string.Empty;
            byte[] hashBytes = new byte[] { };
            byte[] hash = new byte[] { };
            byte[] salt = new byte[16];
            Rfc2898DeriveBytes pbkdf2;
            Boolean valid = true;

            salt = userApiModel.Salt;
            hashedPassword = userApiModel.Hash;
            hashBytes = Convert.FromBase64String(hashedPassword);
            Array.Copy(hashBytes, 0, salt, 0, 16);
            pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    valid = false;
                    break;
                }
            }

            //valid = true; // for debugging user login only; remember to comment!

            return valid;
        }


        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
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

        private async Task<ApplicationUser> GetUser()
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
        [HttpPost]
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
        [HttpPost]
        [Route("CreateContact")]
        public async Task<CreateContactBindingModel> CreateContact(CreateContactBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }
            return await _accountService.CreateContact(model);
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
        [HttpGet]
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
        [HttpGet]
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
        //[System.Web.Http.HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [HttpGet]
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

            var user = await _userManager.FindByLoginAsync(externalLogin.LoginProvider,
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
        //[System.Web.Http.HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
        [HttpGet]
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
        [HttpPost]
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
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }

            //var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            //IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task PasswordMigrationforBPSSSO()
        {
            string newPassword = "4nEX25CjFvQw8bOjB59Y3xuA9Tnv5j!";
            await _accountService.PasswordMigrationforBPSSSO(newPassword);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("LoginToken")]
        public async Task<object> Get(string token = "")
        {
            var tokenHandler = new Services.TokenHandler();

            if (token.StartsWith("token="))
            {
                token = token.Remove(0, 6);
            }

            var res = tokenHandler.DecodeHexString(token);
            DecryptedToken decryptedToken = JsonConvert.DeserializeObject<DecryptedToken>(res);

            var response = await tokenHandler.RequestValidation(decryptedToken.loginName, decryptedToken.securitytoken);

            // if user exists
            //ApplicationUser existingUser = await UserManager.FindByEmailAsync(decryptedToken.email);
            User existingUser = _accountService.GetUserByEmail(decryptedToken.email);

            if (existingUser == null)
            {
                // if user doesn't exist
                var user = new ApplicationUser() { UserName = decryptedToken.email, Email = decryptedToken.email };
                var defaultPassword = "4nEX25CjFvQw8bOjB59Y3xuA9Tnv5j!";
                IdentityResult result = await _userManager.CreateAsync(user, defaultPassword);

                // Create a user in the table
                var name = decryptedToken.givenName.Split(' ');
                _accountService.CreateUser(decryptedToken.givenName, decryptedToken.name, decryptedToken.email);
            }

            if (((int)response) == 200)
            {
                return decryptedToken;
            }
            else
            {
                DecryptedToken returnToken = new DecryptedToken();
                returnToken.email = "Validationfailed";
                //return "Validation failed";
                return returnToken;
            }
        }

        /// <summary>
        /// The RegisterExternal.
        /// </summary>
        /// <param name="model">The model<see cref="RegisterExternalBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [System.Web.Mvc.OverrideAuthentication]
        //[System.Web.Http.HostAuthentication(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer)]
        [HttpPost]
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

            //IdentityResult result = await _userManager.CreateAsync(user);
            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}

            //result = await _userManager.AddLoginAsync(user, info);
            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}
            return Ok();
        }

        /// <summary>
        /// The RemoveLogin.
        /// </summary>
        /// <param name="model">The model<see cref="RemoveLoginBindingModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpGet]
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
                //return HttpUtility.UrlEncode(data);//return HttpServerUtility.UrlTokenEncode(data);
                return "";
            }
        }
    }
}

////using Microsoft.AspNet.Identity.Owin;
////using Microsoft.Owin.Security;
////using Microsoft.Owin.Security.Cookies;
////using Microsoft.Owin.Security.Infrastructure;
////using Microsoft.Owin.Security.OAuth;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using VCLWebAPI.Models;
//using VCLWebAPI.Models.Edmx;
//using System.Linq;
////using Microsoft.AspNet.Identity;

//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.OAuth;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Net.Http.Headers;

//namespace VCLWebAPI.Providers
//{
//    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
//    {
//        private readonly string _publicClientId;

//        public ApplicationOAuthProvider(string publicClientId)
//        {
//            if (publicClientId == null)
//            {
//                throw new ArgumentNullException("publicClientId");
//            }

//            _publicClientId = publicClientId;
//        }

//        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
//        {
//            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

//            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

//            if (user == null)
//            {
//                context.SetError("invalid_grant", "The user name or password is incorrect.");
//                return;
//            }

//            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
//               OAuthDefaults.AuthenticationType);
//            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
//                CookieAuthenticationDefaults.AuthenticationType);

//            AuthenticationProperties properties = CreateProperties(user.UserName);
//            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
//            context.Validated(ticket);
//            context.Request.Context.Authentication.SignIn(cookiesIdentity);
//        }

//        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
//        {
//            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
//            {
//                context.AdditionalResponseParameters.Add(property.Key, property.Value);
//            }

//            return Task.FromResult<object>(null);
//        }

//        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
//        {
//            try
//            {
//                if (context.Parameters["grant_type"].Equals("client_credentials"))
//                {
//                    // Resource owner password credentials does not provide a client ID.
//                    if (context.TryGetBasicCredentials(out string clientId, out string clientSecret) ||
//                    context.TryGetFormCredentials(out clientId, out clientSecret))
//                    {
//                        using (var _db =  new VCLDesignDBEntities())
//                        {
//                            var client = _db.ExternalClient.FirstOrDefault(x => x.ClientExternalId.ToString() == clientId);
//                            var secret = _db.ExternalClient.FirstOrDefault(x => x.ClientSecret == clientSecret);
//                            if (client != null && secret != null)
//                            {
//                                context.Validated();
//                            }
//                        }
//                    }
//                }
//                else if (context.ClientId == null)
//                {
//                    context.Validated();
//                }
//            }
//            catch (Exception ex)
//            {

//            }

//            return Task.FromResult<object>(null);
//        }


//        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
//        {
//            var cl = context.ClientId;
//            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
//            var user = userManager.FindByEmail("SRSAdministrator@vcldesign.com");
//            if (user == null)
//            {
//                //context.SetError("invalid_grant", "The user name or password is incorrect.");
//                context.SetError("invalid_grant");
//                return;
//            }

//            using (var expertDb =  new VCLDesignDBEntities())
//            {
//                var userList = expertDb.User.Where(u => u.Email.ToLower() == "SRSAdministrator@vcldesign.com").ToList();
//            }

//            var requestData = await context.Request.ReadFormAsync();

//            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
//            var cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);

//            var properties = CreateProperties(user.UserName);
//            var ticket = new AuthenticationTicket(oAuthIdentity, properties);
//            context.Validated(ticket);
//            context.Request.Context.Authentication.SignIn(cookiesIdentity);

//        }

//        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
//        {
//            if (context.ClientId == _publicClientId)
//            {
//                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

//                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
//                {
//                    context.Validated();
//                }
//            }

//            return Task.FromResult<object>(null);
//        }

//        public static AuthenticationProperties CreateProperties(string userName)
//        {
//            IDictionary<string, string> data = new Dictionary<string, string>
//            {
//                { "userName", userName }
//            };
//            return new AuthenticationProperties(data);
//        }
//    }

//    public class RefreshTokenProvider : IAuthenticationTokenProvider
//    {
//        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

//        public async Task CreateAsync(AuthenticationTokenCreateContext context)
//        {
//            var guid = Guid.NewGuid().ToString();

//            // copy all properties and set the desired lifetime of refresh token
//            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
//            {
//                IssuedUtc = context.Ticket.Properties.IssuedUtc,
//                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
//            };

//            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

//            _refreshTokens.TryAdd(guid, refreshTokenTicket);

//            // consider storing only the hash of the handle
//            context.SetToken(guid);
//        }

//        public void Create(AuthenticationTokenCreateContext context)
//        {
//            throw new NotImplementedException();
//        }

//        public void Receive(AuthenticationTokenReceiveContext context)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
//        {
//            // context.DeserializeTicket(context.Token);
//            AuthenticationTicket ticket;
//            string header = context.OwinContext.Request.Headers["Authorization"];

//            if (_refreshTokens.TryRemove(context.Token, out ticket))
//            {
//                context.SetTicket(ticket);
//            }
//        }
//    }
//}
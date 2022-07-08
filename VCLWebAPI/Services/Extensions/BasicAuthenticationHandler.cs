using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using VCLWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VCLWebAPI.Services.Handlers
{
    public class BasicAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DbContext _context;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApplicationDbContext context) : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
            if(!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Authorization header was not found");
            try
            {

                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] creds = Encoding.UTF8.GetString(bytes).Split(":");
                string clientId = creds[0];
                string clientSecret = creds[1];

                //var externalClients = _context.ExternalClients.Where(ec => ec.ClientExternalId == clientId && ec.ClientSecret == clientSecret).FirstOrDefault();
                //if (externalClients == null)
                //{
                //    return AuthenticateResult.Fail("Invalid Account");
                //}
                //else {
                    var claims = new[] { new Claim(ClaimTypes.Name, HttpContextHelper.Current.User.Identity.Name) };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                //}
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail("Error has occured");
            }
        }
    }
}

using log4net;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Loggers
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        static readonly ILog log = LogManager.GetLogger(nameof(LoggingMiddleware));
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                await _next(context);

                //if (context.Request.Path.Value.Contains("/swagger/index.html") || context.Request.Path.Value.Contains("/token/"))
                //{
                //    context.Request.EnableBuffering();
                //    await _next.Invoke(context); // call next middleware  
                //}

            }
            finally
            {
                if (!context.Request.Path.Value.Contains("/swagger/") && !context.Request.Path.Value.Contains("/token/") && 
                    (context.Request.Method.Contains("GET") || context.Request.Method.Contains("PUT") || context.Request.Method.Contains("POST") || context.Request.Method.Contains("DELETE") || context.Request.Method.Contains("PATCH")))
                {
                    var userId = string.Empty;
                    var identity = (ClaimsIdentity)context.User.Identity;
                    IEnumerable<Claim> claims = identity.Claims;

                    foreach (var item in claims)
                    {
                        if (item.Type == "UserId")
                        {
                            userId = item.Value;
                            break;
                        }
                    }

                    log.Info(userId + " " + Convert.ToString(context.Request.Headers["User-Agent"]) + " " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss tt") + "[UTC]" +
                    " " + context.Request.Scheme +
                    " " + context.Request?.Host +
                    " " + context.Request?.Path.Value +
                    " " + context.Request?.Method +
                    Environment.NewLine + await GetRawBodyAsync(context.Request));
                }
            }
        }

        public static async Task<string> GetRawBodyAsync(HttpRequest request, Encoding encoding = null)
        {
            if (!request.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable,
                // as EnableBuffering will create a new stream instance
                // each time it's called
                request.EnableBuffering();
            }

            request.Body.Position = 0;

            var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

            var body = await reader.ReadToEndAsync().ConfigureAwait(false);

            request.Body.Position = 0;

            return body + Environment.NewLine;
        }
    }
}

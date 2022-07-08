using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Helpers;
using ExceptionFilterAttribute = Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute;

namespace Services.Filters
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ExceptionHelper.FilterException(context);
        }

        //public override Task OnExceptionAsync(ExceptionContext actionExecutedContext, CancellationToken cancellationToken)
        //{
        //    if (actionExecutedContext.Request.RequestUri.LocalPath.StartsWith("/api", System.StringComparison.OrdinalIgnoreCase))
        //    {
        //        return ExceptionHelper.FilterException(actionExecutedContext, cancellationToken);
        //    }
        //    return Task.CompletedTask;
        //}
    }
}
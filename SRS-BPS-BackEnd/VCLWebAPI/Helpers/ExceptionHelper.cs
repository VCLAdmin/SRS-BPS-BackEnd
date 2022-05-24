using System;
using System.IO;
using System.Net;
using System.Threading;
using VCLWebAPI.Models;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using VCLWebAPI.Exceptions;
using VCLWebAPI.Utils;

namespace Services.Helpers
{
    public static class ExceptionHelper
    {
        static readonly ILog log = log4net.LogManager.GetLogger(nameof(ExceptionHelper));

        /// <summary>
        /// Filters the exception using our custom exception types.
        /// </summary>
        /// <param name = "context" > The context in wich the exception was thrown.</param>
        /// <param name = "cancellationToken" > The cancellation token.</param>
        /// <returns></returns>
        public static void FilterException(ExceptionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            var customException = context.Exception as CustomException;
            string message;

            if (customException == null)
            {
                message = context.Exception.ToString();
                status = HttpStatusCode.InternalServerError;
            }

            if (customException is InvalidModelException)
            {
                message = context.Exception.ToString();
                status = HttpStatusCode.BadRequest;
            }
            else if (customException is NotFoundException)
            {
                message = context.Exception.ToString();
                status = HttpStatusCode.NotFound;
            }
            else if (customException is UnauthorizedException)
            {
                message = context.Exception.ToString();
                status = HttpStatusCode.Unauthorized;
            }
            else if (customException is ForbiddenException)
            {
                message = context.Exception.ToString();
                status = HttpStatusCode.Forbidden;
            }
            else if (customException is ConflictException)
            {
                message = context.Exception.ToString();
                status = HttpStatusCode.Conflict;
            }
            else
            {
                message = context.Exception.ToString();
                status = HttpStatusCode.InternalServerError;
            }

            context.ExceptionHandled = true;

            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = (int)status;
            response.ContentType = "application/json";
            var err = message + " " + context.Exception.StackTrace;

            // we log the exception in the log file

            try
            {
                if(status != HttpStatusCode.Unauthorized)
                {
                    // Unauthorized exceptions should not be logged
                    ExceptionHelper.LogException(context, status, message);
                }
            }
            catch(Exception e)
            {
                // we don't do anything, just continue with the request
            }

            response.WriteAsync(err, cancellationToken: cancellationToken);
        }

        public static void LogException(ExceptionContext context, HttpStatusCode status, string message)
        {
            ControllerActionDescriptor actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            var userClaims = ApiUtil.ExtractUserClaims(context.HttpContext.User.Claims);

            var exceptionLog = new ExceptionLog
            {
                Action = actionDescriptor.ActionName,
                Controller = actionDescriptor.ControllerName,
                DateTime = DateTime.UtcNow,
                ErrorCode = status.ToString(),
                Exception = context.Exception.GetType().Name,
                ExceptionDetail = message,
                ExceptionLogExternalId = Guid.NewGuid(),
                InnerException = context.Exception.InnerException != null ? context.Exception.InnerException.Message : null,
                RequestUrl = context.HttpContext.Request.Path.Value,
                User = userClaims.Email,
                UserExternalId = userClaims.UserId,
                URI = context.HttpContext.Request.Path.Value,
                RequestUrlReferrer = context.HttpContext.Request.Headers["Origin"],
                RequestUserAgent = context.HttpContext.Request.Headers["User-Agent"]
            };

            var msg = exceptionLog.ErrorCode + " ; " + exceptionLog.DateTime + " ; " + exceptionLog.Controller + " ; " + exceptionLog.Action + " ; " +
                exceptionLog + " ; " + exceptionLog.UserExternalId + " ; " + exceptionLog.URI + " ; " + exceptionLog.ExceptionDetail + " ; " +
                exceptionLog.RequestUrl + " ; " + exceptionLog.RequestUrlReferrer + " ; " + exceptionLog.RequestUserAgent;

            LogToTextFile(msg);
        }

        public static void LogToTextFile(string msg)
        {
            try
            {
                log.Info(msg); //write to a file using log4net

                //string filepath = Globals.AppPhysicalPath + @"\logs\";  //Text File Path

                //if (!Directory.Exists(filepath))
                //{
                //    Directory.CreateDirectory(filepath);

                //}
                //filepath = filepath + DateTime.Today.ToString("MM.dd.yyyy") + "_Application_Error_Log.txt";   //Text File Name
                //if (!File.Exists(filepath))
                //{
                //    File.Create(filepath).Dispose();

                //}
                //using (StreamWriter sw = File.AppendText(filepath))
                //{
                //    //sw.WriteLine(GetLog());
                //    sw.WriteLine(DateTime.Now.ToLongTimeString() + "---------------------------------------------------------------------");
                //    sw.WriteLine(msg);
                //    sw.Flush();
                //    sw.Close();
                //}
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }
    }
}
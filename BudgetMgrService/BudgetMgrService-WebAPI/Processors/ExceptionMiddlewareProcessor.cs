using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using BudgetMgrService_WebAPI.Models;
using BudgetMgrService_WebAPI.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetMgrService_WebAPI.Processors
{
    public static class ExceptionMiddlewareProcessor
    {

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerProcessor>();
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = $"Internal Server Error - {contextFeature.Error.Message}"
                        }.ToString());
                    }
                });
            });
        }
    }
}
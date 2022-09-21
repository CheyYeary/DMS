using DMS.Configurations;
using DMS.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DMS.Middleware.Utilities
{
    public class ExceptionHandler
    {
        private static readonly JsonSerializerOptions jsonSerializerSettings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        /// <summary>
        /// Handles converting the original exception to a known exception and writes the error model to the response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="statsDConfig"></param>
        /// <param name="metricsLogger"></param>
        /// <param name="envConfig"></param>
        /// <returns></returns>
        public static async Task HandleException(HttpContext context, ILogger logger, IEnvironmentConfig envConfig)
        {
            IExceptionHandlerFeature? contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature == null)
            {
                return;
            }
            HttpStatusCode statusCode = ExceptionConverter.GetHttpStatusCode(contextFeature.Error);
            ErrorResponseModel errorResponse = new()
            {
                Error = ExceptionConverter.CreateErrorModel(contextFeature.Error, statusCode, envConfig)
            };

            await ModifyHttpResponse(context, statusCode, errorResponse);

            logger.LogError($"ExceptionMiddleware| Http context response has been written with status: {statusCode}.", contextFeature.Error);
        }

        /// <summary>
        /// Writes the error model to the response.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <param name="statusCode">The terminal status code.</param>
        /// <param name="errorResponse">The error model.</param>
        /// <returns></returns>
        private async static Task ModifyHttpResponse(HttpContext context, HttpStatusCode statusCode, ErrorResponseModel errorResponse)
        {
            // Build HTTP response
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                errorResponse,
                jsonSerializerSettings), Encoding.UTF8);
        }
    }
}

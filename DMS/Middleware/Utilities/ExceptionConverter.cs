using DMS.Models.Exceptions;
using DMS.Models;
using System.Net;
using DMS.Configurations;

namespace DMS.Middleware.Utilities
{
    public static class ExceptionConverter
    {
        /// <summary>
        /// Convert an exception to an error model.
        /// </summary>
        /// <param name="ex">The original exception.</param>
        /// <param name="statusCode">The terminal status code.</param>
        /// <param name="envConfig">The environment configuration.</param>
        /// <returns></returns>
        public static ErrorModel CreateErrorModel(Exception ex, HttpStatusCode statusCode, IEnvironmentConfig envConfig)
        {
            KnownException? knownException = GetKnownException(ex);
            if (knownException != null)
            {
                return CreateKnownError(knownException, statusCode);
            }
            else if (ex == null || !envConfig.IsDevelopmentEnvironment)
            {
                // by default do not throw unhandled exceptions to customer.
                return new ErrorModel
                {
                    Code = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Unknown error"
                };
            }
            else
            {
                // allow unknown exceptions to be throw in dev environments.
                return CreateDetailedError(ex, statusCode);
            }
        }

        /// <summary>
        /// Cast the exception as a known exception otherwise null.
        /// </summary>
        /// <param name="ex">The original exception.</param>
        /// <returns></returns>
        public static KnownException? GetKnownException(Exception ex)
        {
            KnownException? knownException = ex as KnownException;
            knownException ??= ex?.InnerException as KnownException;

            return knownException;
        }

        /// <summary>
        /// Creates an error model for a known exception
        /// </summary>
        /// <param name="knownException">The known exception.</param>
        /// <param name="statusCode">The terminal status code</param>
        /// <returns></returns>
        public static ErrorModel CreateKnownError(KnownException knownException, HttpStatusCode statusCode)
        {
            ErrorModel error = new()
            {
                Code = knownException.Code.ToString(),
                Message = knownException.Message,
                Target = knownException.Details
            };
            if (statusCode >= HttpStatusCode.InternalServerError)
            {
                return error;
            }

            List<ErrorModel> details = new();
            Exception innerException = knownException.InnerException;
            while (innerException != null)
            {
                details.Add(new ErrorModel
                {
                    Code = knownException.Code.ToString(),
                    Message = knownException.Message,
                    Target = knownException.Details
                });

                innerException = innerException.InnerException;
            }

            error.Details = details.ToArray();

            return error;
        }

        /// <summary>
        /// Creates an error model with full inner exception details. 
        /// </summary>
        /// <param name="exception">The known exception.</param>
        /// <param name="statusCode">The terminal status code.</param>
        /// <returns></returns>
        public static ErrorModel CreateDetailedError(Exception exception, HttpStatusCode statusCode)
        {
            ErrorModel error = new()
            {
                Code = statusCode.ToString(),
                Message = exception.Message,
                Target = exception.StackTrace
            };

            List<ErrorModel> details = new();
            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                details.Add(new ErrorModel
                {
                    Code = statusCode.ToString(),
                    Message = innerException.Message,
                    Target = innerException.StackTrace
                });

                innerException = innerException.InnerException;
            }

            error.Details = details.ToArray();

            return error;
        }

        /// <summary>
        /// Create the status code to be returned for client requested.
        /// If the exception is a known exception a specific status code will be given, otherwise, an internal server error.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static HttpStatusCode GetHttpStatusCode(Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            KnownException? knownException = ex as KnownException;
            knownException ??= ex?.InnerException as KnownException;
            if (knownException != null)
            {
                statusCode = GetHttpStatusCode(knownException);
            }

            return statusCode;
        }

        /// <summary>
        /// Gets the HTTP status code per exception
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static HttpStatusCode GetHttpStatusCode(KnownException ex)
        {
            return ex.Category switch
            {
                ErrorCategory.ResourceNotFound => HttpStatusCode.NotFound,
                ErrorCategory.Conflict => HttpStatusCode.Conflict,
                ErrorCategory.AuthenticationError or ErrorCategory.Forbidden => HttpStatusCode.Unauthorized,
                ErrorCategory.InputError => HttpStatusCode.BadRequest,
                ErrorCategory.TooManyRequests => (HttpStatusCode)429,
                ErrorCategory.BackEndDependencyInaccessible => HttpStatusCode.ServiceUnavailable,
                ErrorCategory.DownStreamError => HttpStatusCode.BadGateway,
                _ => HttpStatusCode.InternalServerError,
            };
        }
    }

}

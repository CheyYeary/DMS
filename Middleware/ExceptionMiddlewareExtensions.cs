using DMS.Configurations;
using DMS.Middleware.Utilities;

namespace DMS.Middleware
{
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Adds the exception handler middleware to the pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="logger"></param>
        /// <param name="statsDConfig"></param>
        /// <param name="metricsLogger"></param>
        /// <param name="envConfig"></param>
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger, IEnvironmentConfig envConfig)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    await ExceptionHandler.HandleException(context, logger, envConfig);
                });
            });
        }
    }

}

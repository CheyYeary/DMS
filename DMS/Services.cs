using DMS.Components.DeadManSwitch;
using DMS.Components.Login;
using DMS.Configurations;
using DMS.DataProviders;
using DMS.DataProviders.DataFactory;
using DMS.DataProviders.Login;
using DMS.Logging;

namespace DMS
{
    public static class Services
    {
        /// <summary>
        /// Add congig
        /// </summary>
        /// <param name="serviceCollection">The service collection</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddConfig(this IServiceCollection serviceCollection)
        {
            // configurations

            serviceCollection.AddSingleton<IEnvironmentConfig, EnvironmentConfig>();
            serviceCollection.AddSingleton<IDataFactoryConfig, DataFactoryConfig>();

            return serviceCollection;
        }

        /// <summary>
        /// Add swagger
        /// </summary>
        /// <param name="serviceCollection">The service collection</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen();

            return serviceCollection;
        }

        /// <summary>
        /// Add logging
        /// </summary>
        /// <param name="serviceCollection">The service collection</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddLoggers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton<ILogger, ConsoleJsonLogger>();

            return serviceCollection;
        }

        /// <summary>
        /// Add components
        /// </summary>
        /// <param name="serviceCollection">The service collection</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddComponents(this IServiceCollection serviceCollection)
        {
            // components
            serviceCollection.AddScoped<IDeadManSwitchComponent, DeadManSwitchComponent>();
            serviceCollection.AddScoped<ILoginComponent, LoginComponent>();
            // repositories
            serviceCollection.AddSingleton<IBlobService, AzureBlobService>();
            serviceCollection.AddSingleton<ILoginRepository, LoginRepository>();
            serviceCollection.AddSingleton<IDataFactoryService, DataFactoryService>();

            return serviceCollection;
        }
    }
}

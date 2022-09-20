using DMS.Configurations;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using LogLevel = Microsoft.Identity.Client.LogLevel;

namespace DMS.DataProviders.DataFactory
{
    public class DataFactoryService : IDataFactoryService
    {
        private readonly ILogger<DataFactoryService> logger;
        private readonly IDataFactoryConfig configuration;
        private readonly IEnumerable<string> scopes;
        private IDataFactoryManagementClient client;

        public DataFactoryService(ILogger<DataFactoryService> logger, IDataFactoryConfig configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.scopes = new string[] { configuration.Resource + "/.default" };
        }

        public async Task Initialize()
        {
            if (this.client != null)
            {
                throw new InvalidOperationException("Data factory already initialized");
            }

            // Authenticate and create a data factory management client  
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(this.configuration.ClientId)
                .WithAuthority(new Uri(this.configuration.Authority))
                .WithLogging(LogCallback)
                .WithClientSecret(configuration.AuthenticationKey)
                .WithLegacyCacheCompatibility(false)
                .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
                .Build();

            AuthenticationResult result = await app.AcquireTokenForClient(this.scopes)
            .ExecuteAsync();
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            this.client = new DataFactoryManagementClient(cred)
            {
                SubscriptionId = configuration.SubscriptionId
            };
        }

        /// <inheritdoc/>
        public async Task CreateTrigger(string triggerName, ScheduleTriggerRecurrence recurrence, CancellationToken cancellationToken)
        {
            string pipelineName = "DeadManSwitchPipeline";

            // Create the trigger
            this.logger.LogInformation("Creating the trigger");

            // Set the start time to the current UTC time
            DateTime startTime = DateTime.UtcNow;

            // Specify values for the inputPath and outputPath parameters
            Dictionary<string, object> pipelineParameters = new()
            {
                { "inputPath", "adftutorial/input" },
                { "outputPath", "adftutorial/output" }
            };

            // Create a schedule trigger
            TriggerResource triggerResource = new()
            {
                Properties = new ScheduleTrigger()
                {
                    Pipelines = new List<TriggerPipelineReference>()
                    {
                        // Associate the Adfv2QuickStartPipeline pipeline with the trigger
                        new TriggerPipelineReference()
                        {
                            PipelineReference = new PipelineReference(pipelineName),
                            Parameters = pipelineParameters,
                        }
                    },
                    Recurrence = recurrence
                }
            };

            try
            {
                // Now, create the trigger by invoking the CreateOrUpdate method
                await client.Triggers.CreateOrUpdateAsync(configuration.ResourceGroup, configuration.DataFactoryName, triggerName, triggerResource, cancellationToken: cancellationToken);

                // Start the trigger
                this.logger.LogInformation("Starting the trigger");
                await client.Triggers.StartAsync(configuration.ResourceGroup, configuration.DataFactoryName, triggerName, cancellationToken);
            }
            catch (CloudException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await this.DisableTrigger(triggerName, cancellationToken);
                // try again
                await this.CreateTrigger(triggerName, recurrence, cancellationToken);
            }
        }

        private void LogCallback(
            LogLevel level,
            string message,
            bool containsPii)
        {
            switch (level)
            {
                case LogLevel.Error:
                    this.logger?.LogError(message);
                    break;
                case LogLevel.Warning:
                    this.logger?.LogWarning(message);
                    break;
                default:
                    // Do not log verbose or info
                    break;
            }
        }

        /// <inheritdoc/>
        public async Task DisableTrigger(string triggerName, CancellationToken cancellationToken)
        {
            await client.Triggers.StopAsync(configuration.ResourceGroup, configuration.DataFactoryName, triggerName, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<TriggerResource> GetTrigger(string triggerName, CancellationToken cancellationToken)
        {
            return await client.Triggers.GetAsync(configuration.ResourceGroup, configuration.DataFactoryName, triggerName, cancellationToken: cancellationToken);
        }
    }
}

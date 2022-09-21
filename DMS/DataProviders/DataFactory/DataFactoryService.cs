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
                return;
            }

            try
            {
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
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to initialize data factory service");
                throw;
            }

           
        }

        /// <inheritdoc/>
        public async Task CreateTrigger(Guid accountId, string triggerName, ScheduleTriggerRecurrence recurrence, CancellationToken cancellationToken)
        {
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
                            PipelineReference = new PipelineReference(GetPipelineName(accountId)),
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
                await this.CreateTrigger(accountId, triggerName, recurrence, cancellationToken);
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

        public async Task<PipelineResource> CreatePipeline(Guid accountId, CancellationToken cancellationToken)
        {
            // Create the trigger
            this.logger.LogInformation("Creating the pipeline");
            string pipelineName = GetPipelineName(accountId);
            PipelineResource pipeline = new(name: pipelineName, type: "AzureFunctionActivity", activities: new List<Activity>()
            {
                new AzureFunctionActivity{
                    Name = "AzureFunctionActivity",
                    FunctionName = "DeadManSwitchFunction",
                    Headers = new Dictionary<string, string>()
                    {
                        ["accountId"] = accountId.ToString()
                    },
                    Method = HttpMethod.Get.ToString(),
                    LinkedServiceName = new LinkedServiceReference()
                    {
                        ReferenceName = "DeadManSwitchFunction",
                    }
                }
            });
            
            // Now, create the trigger by invoking the CreateOrUpdate method
            return await client.Pipelines.CreateOrUpdateAsync(configuration.ResourceGroup, configuration.DataFactoryName, pipelineName, pipeline, cancellationToken: cancellationToken);
        }

        private static string GetPipelineName(Guid accountId) => $"DeadManSwitchPipeline_{accountId}";

    }
}

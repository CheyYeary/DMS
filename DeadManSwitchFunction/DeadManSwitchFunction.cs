using DeadManSwitchFunction;
using DMS.Components.DeadManSwitch;
using DMS.DataProviders.DataFactory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


[assembly: FunctionsStartup(typeof(Startup))]
namespace DeadManSwitchFunction
{
    public class DeadManSwitchFunction
    {
        private readonly IServiceProvider services;

        public DeadManSwitchFunction(IServiceProvider services)
        {
            this.services = services;
        }
        
        [FunctionName("DeadManSwitchFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await this.Initialize();

            Guid accountId = Guid.Parse(req.Headers["accountId"]);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            IDeadManSwitchComponent component = this.services.GetRequiredService<IDeadManSwitchComponent>();

            await component.Send(accountId, CancellationToken.None);

            return new OkObjectResult("Success");
        }

        public async Task Initialize()
        {
            // Async operations should not run in constructors invoked by dependency injection; they can lead to a deadlock, see this article
            // https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines
            var dataFactoryService = services.GetRequiredService<IDataFactoryService>();
            await dataFactoryService.Initialize();
        }
    }
}

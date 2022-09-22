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
using FluentEmail.Smtp;
using FluentEmail.Core;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;


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
            SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com");

            var _sender = "deadmanswitchhackathon@outlook.com";
            var _password = "";


            System.Net.NetworkCredential creds = new System.Net.NetworkCredential(_sender, _password);
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = creds;


            var sender = new SmtpSender(smtp);
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            Email.DefaultSender = sender;

            var email = await Email
                .From("deadmanswitchhackathon@outlook.com")
                .To("cheyj.yeary@gmail.com", "Chey")
                .Subject($"DMS has been activated number {time}")
                .Body("DMS from one of our users has been invoked and you have been placed on a list to recieve files from the user.")
                .SendAsync();

            Console.WriteLine("Successful send: " + email.Successful);

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

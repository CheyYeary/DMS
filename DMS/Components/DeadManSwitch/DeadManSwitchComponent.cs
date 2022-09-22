using DMS.DataProviders.Login;
using DMS.Models;
using DMS.Models.Exceptions;
using FluentEmail.Smtp;
using FluentEmail.Core;
using Microsoft.Azure.Management.DataFactory.Models;
using System.Net.Mail;
using FluentEmail.Core.Models;

namespace DMS.Components.DeadManSwitch
{
    public class DeadManSwitchComponent : IDeadManSwitchComponent
    {
        private readonly ILogger<DeadManSwitchComponent> logger;
        private readonly ILoginRepository loginRepository;

        public DeadManSwitchComponent(ILogger<DeadManSwitchComponent> logger, ILoginRepository loginRepository)
        {
            this.logger = logger;
            this.loginRepository = loginRepository;
        }
        
        public async Task Send(Guid accountId, CancellationToken cancellationToken)
        {
            LoginKey key = new()
            {
                AccountId = accountId
            };
            DateTime currentTime = DateTime.UtcNow;

            LoginResponseModel existingLogin = await loginRepository.GetLoginModel(key, cancellationToken);
            if (existingLogin == null)
            {
                throw new KnownException(ErrorCategory.ResourceNotFound, ServiceErrorCode.User_NotFound, "User not found");
            }

            // If the users's most recent login is before now minus the threshold, then flip the dead man switch
            TimeSpan deadManSwitchInterval = GetInterval(existingLogin.Recurrence.Frequency, existingLogin.Recurrence.Interval ?? 1);
            if (DateTime.Compare(existingLogin.LastModifiedAt, currentTime - deadManSwitchInterval) < 0)
            {
                string message = $"{existingLogin.AccountId}: Did not login in time. Flipping the switch";
                logger.LogWarning(message);
                await this.SendEmailAsync();
            }
        }

        private async Task<bool> SendEmailAsync()
        {
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

            SendResponse email = await Email
                .From("deadmanswitchhackathon@outlook.com")
                .To("cheyj.yeary@gmail.com", "Chey")
                .Subject($"DMS has been activated number {time}")
                .Body("DMS from one of our users has been invoked and you have been placed on a list to recieve files from the user.")
                .SendAsync();

            this.logger.LogInformation($"Email sent: {email.Successful}");

            return email.Successful;

        }

        private static TimeSpan GetInterval(string frequency, int interval)
        {
            TimeSpan deadManSwitchInterval;
            if (frequency.Equals(RecurrenceFrequency.Minute, StringComparison.OrdinalIgnoreCase))
            {
                deadManSwitchInterval = TimeSpan.FromMinutes(interval);
            }
            else if (frequency.Equals(RecurrenceFrequency.Hour, StringComparison.OrdinalIgnoreCase))
            {
                deadManSwitchInterval = TimeSpan.FromHours(interval);
            }
            else if (frequency.Equals(RecurrenceFrequency.Day, StringComparison.OrdinalIgnoreCase))
            {
                deadManSwitchInterval = TimeSpan.FromDays(interval);
            }
            else if (frequency.Equals(RecurrenceFrequency.Month, StringComparison.OrdinalIgnoreCase))
            {
                deadManSwitchInterval = TimeSpan.FromDays(30 * interval);
            }
            else if (frequency.Equals(RecurrenceFrequency.Year, StringComparison.OrdinalIgnoreCase))
            {
                deadManSwitchInterval = TimeSpan.FromDays(365 * interval);
            }
            else
            {
                deadManSwitchInterval = TimeSpan.FromDays(interval * 365);
            }

            return deadManSwitchInterval;
        }
    }
}

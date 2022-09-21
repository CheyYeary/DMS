using DMS.DataProviders.Login;
using DMS.Models;
using DMS.Models.Exceptions;
using Microsoft.Azure.Management.DataFactory.Models;

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
                string message = "You did not login in time. Flipping the switch";
                logger.LogWarning(message);
            }
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

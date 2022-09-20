using Microsoft.Azure.Management.DataFactory.Models;

namespace DMS.Models
{
    public class SignUpRequestModel 
    {
        public TimeSpan DeadManSwitchInterval { get; init; }

        public ScheduleTriggerRecurrence Recurrence { get; init; }
    }
}

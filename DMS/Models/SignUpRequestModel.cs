using Microsoft.Azure.Management.DataFactory.Models;

namespace DMS.Models
{
    public class SignUpRequestModel 
    {
        public ScheduleTriggerRecurrence Recurrence { get; init; }
    }
}

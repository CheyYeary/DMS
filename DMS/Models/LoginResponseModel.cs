using Microsoft.Azure.Management.DataFactory.Models;
using System.ComponentModel;

namespace DMS.Models
{

    public class LoginResponseModel
    {
        [ReadOnly(true)]
        public Guid AccountId { get; init; }

        [ReadOnly(true)]
        public DateTime CreatedAt { get; init; }

        [ReadOnly(true)]
        public DateTime LastModifiedAt { get; set; }

        public ScheduleTriggerRecurrence Recurrence { get; init; }
    }
}

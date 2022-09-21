using Microsoft.Azure.Management.DataFactory.Models;

namespace DMS.DataProviders.DataFactory
{
    public interface IDataFactoryService
    {
        Task Initialize();

        Task CreateTrigger(string triggerName, ScheduleTriggerRecurrence recurrence, CancellationToken cancellationToken);

        Task DisableTrigger(string triggerName, CancellationToken cancellationToken);

        Task<TriggerResource> GetTrigger(string triggerName, CancellationToken cancellationToken);
    }
}

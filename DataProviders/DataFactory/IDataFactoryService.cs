namespace DMS.DataProviders.DataFactory
{
    public interface IDataFactoryService
    {
        Task Initialize();

        Task CreateTrigger(CancellationToken cancellationToken);
    }
}

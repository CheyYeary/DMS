namespace DMS.Configurations
{
    public interface IDataFactoryConfig
    {
        string SubscriptionId { get; }

        string ResourceGroup { get; }

        string TenantId { get; }

        string ClientId { get; }

        string DataFactoryName { get; }

        string Authority { get; }

        string Resource { get; }

        string AuthenticationKey { get; }
    }
}

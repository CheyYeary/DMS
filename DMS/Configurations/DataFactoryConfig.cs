namespace DMS.Configurations
{
    public class DataFactoryConfig : IDataFactoryConfig
    {
        private readonly IConfigurationSection configSection;

        public DataFactoryConfig(IConfiguration configuration)
        {
            this.configSection = configuration.GetSection("DataFactoryConfig");
        }
        
        public string SubscriptionId => Environment.GetEnvironmentVariable("SubscriptionId") ?? this.configSection.GetValue<string>("SubscriptionId") ?? throw new ArgumentNullException("SubscriptionId");

        public string ResourceGroup => Environment.GetEnvironmentVariable("ResourceGroup") ?? this.configSection.GetValue<string>("ResourceGroup") ?? throw new ArgumentNullException("ResourceGroup");

        public string TenantId => Environment.GetEnvironmentVariable("TenantId") ?? this.configSection.GetValue<string>("TenantId") ?? throw new ArgumentNullException("TenantId");

        public string ClientId => Environment.GetEnvironmentVariable("ClientId") ?? this.configSection.GetValue<string>("ClientId") ?? throw new ArgumentNullException("ClientId");

        public string DataFactoryName => Environment.GetEnvironmentVariable("DataFactoryName") ?? this.configSection.GetValue<string>("DataFactoryName") ?? throw new ArgumentNullException("DataFactoryName");

        public string Authority => "https://login.microsoftonline.com/" + this.TenantId;

        public string Resource => "https://management.azure.com/";

        public string AuthenticationKey => Environment.GetEnvironmentVariable("AuthenticationKey") ?? this.configSection.GetValue<string>("AuthenticationKey") ?? throw new ArgumentNullException("AuthenticationKey");
    }
}

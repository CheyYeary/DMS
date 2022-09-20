using Microsoft.Extensions.Configuration;

namespace DMS.Configurations
{
    public class DataFactoryConfig : IDataFactoryConfig
    {
        private readonly IConfigurationSection configSection;

        public DataFactoryConfig(IConfiguration configuration)
        {
            this.configSection = configuration.GetSection("DataFactoryConfig");
        }

        public string SubscriptionId => this.configSection.GetValue<string>("SubscriptionId");

        public string ResourceGroup => this.configSection.GetValue<string>("ResourceGroup");

        public string TenantId => this.configSection.GetValue<string>("TenantId");

        public string ClientId => this.configSection.GetValue<string>("ClientId");

        public string DataFactoryName => this.configSection.GetValue<string>("DataFactoryName");

        public string Authority => "https://login.microsoftonline.com/" + this.TenantId;

        public string Resource => "https://management.azure.com/";

        public string AuthenticationKey => this.configSection.GetValue<string>("AuthenticationKey");
    }
}

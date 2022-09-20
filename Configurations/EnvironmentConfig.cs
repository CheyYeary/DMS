namespace DMS.Configurations
{
    public class EnvironmentConfig : IEnvironmentConfig
    {
        private readonly IConfigurationSection configSection;

        public EnvironmentConfig(IConfiguration configuration)
        {
            this.configSection = configuration.GetSection("EnvironmentConfig");
        }

        public string Environment => this.configSection.GetValue<string>("Environment");

        public bool IsDevelopmentEnvironment => this.Environment == "Development";
    }
}

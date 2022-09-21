namespace DMS.Configurations
{
    public class EnvironmentConfig : IEnvironmentConfig
    {

        public EnvironmentConfig()
        {
        }

        public string Environment => System.Environment.GetEnvironmentVariable("Environment") ?? throw new ArgumentNullException("Environment");

        public bool IsDevelopmentEnvironment => this.Environment == "Development";
    }
}

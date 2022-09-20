namespace DMS.Configurations
{
    public interface IEnvironmentConfig
    {
        string Environment { get; }
        
        bool IsDevelopmentEnvironment { get; }
    }
}

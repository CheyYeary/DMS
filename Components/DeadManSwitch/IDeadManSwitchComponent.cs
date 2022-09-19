namespace DMS.Components.DeadManSwitch
{
    public interface IDeadManSwitchComponent
    {
        Task Send(CancellationToken cancellationToken);
    }
}

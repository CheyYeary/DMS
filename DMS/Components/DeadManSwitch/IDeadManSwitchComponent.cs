namespace DMS.Components.DeadManSwitch
{
    public interface IDeadManSwitchComponent
    {
        Task Send(Guid accountId, CancellationToken cancellationToken);
    }
}

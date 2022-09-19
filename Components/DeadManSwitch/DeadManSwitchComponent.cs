namespace DMS.Components.DeadManSwitch
{
    public class DeadManSwitchComponent : IDeadManSwitchComponent
    {
        public Task Send(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces
{
    public interface IStateWithPayload<TData> : IState
    {
        public void Enter(TData data);
    }
}
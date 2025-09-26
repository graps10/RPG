namespace Player
{
    public class PlayerStateMachine
    {
        public PlayerState CurrentState { get; private set; }

        public void Initialize(PlayerState _startState)
        {
            CurrentState = _startState;
            CurrentState.Enter();
        }

        public void ChangeState(PlayerState _newState)
        {
            CurrentState.Exit();
            CurrentState = _newState;
            CurrentState.Enter();
        }
    }
}

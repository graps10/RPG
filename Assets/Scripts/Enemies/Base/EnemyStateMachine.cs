using UnityEngine;

namespace Enemies.Base
{
    public class EnemyStateMachine
    {
        public EnemyState CurrentState { get; private set; }

        public void Initialize(EnemyState _startState)
        {
            CurrentState = _startState;
            CurrentState.Enter();
        }

        public void ChangeState(EnemyState _newState)
        {
            if (_newState == null)
            {
                Debug.LogWarning("Trying to change to a NULL state! Check your Enemy's Awake method.");
                return;
            }
            
            CurrentState.Exit();
            CurrentState = _newState;
            CurrentState.Enter();
        }
    }
}
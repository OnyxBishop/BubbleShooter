using System;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IState> _states = new();
        private IState _currentState;
        
        public void AddStates(params IState[] states)
        {
            foreach (var state in states)
            {
                var type = state.GetType();
                _states.TryAdd(type, state);
            }
        }

        public void ChangeState<TState>()
            where TState : IState
        {
            var type = typeof(TState);

            if (!_states.TryGetValue(type, out var nextState))
                throw new ArgumentException($"State {type.Name} has not been added to the StateMachine.");

            _currentState?.Exit();
            _currentState = nextState;
            _currentState.Enter();
        }

        public void ChangeState<TState, TData>(TData data)
            where TState : IStateWithPayload<TData>
        {
            var type = typeof(TState);

            if (!_states.TryGetValue(type, out var nextState))
                throw new ArgumentException($"You dont added state {nameof(type)}");

            _currentState?.Exit();
            _currentState = nextState;

            var typedState = (IStateWithPayload<TData>)_currentState;
            typedState.Enter(data);
        }
    }
}
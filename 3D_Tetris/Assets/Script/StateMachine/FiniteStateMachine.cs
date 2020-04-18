using System;
using System.Collections.Generic;

public class FiniteStateMachine<T> where T : Enum
{
    public event Action<T> onStateChanged;

    public T currentState { get; private set; }

    public T previousState { get; private set; }

    private readonly Dictionary<T, Action> _transitions = new Dictionary<T, Action>();

    protected void AddTransition(T state, Action stateChangeHandler)
    {
        if (!_transitions.ContainsKey(state))
            _transitions.Add(state, stateChangeHandler);
    }

    protected void SetState(T state)
    {
        previousState = currentState;
        currentState = state;

        if (_transitions.TryGetValue(state, out var transition))
            transition();

        onStateChanged?.Invoke(currentState);
    }
}

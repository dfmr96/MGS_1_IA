using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class FSM<T>
{
    IState<T> _current;
    [SerializeField] private string _currentStateName;
    
    public IState<T> CurrentState
    {
        get => _current;
        set
        {
            _current = value;
            _currentStateName = _current?.ToString();
        }
    }
    public FSM() { }
    public FSM(IState<T> initState)
    {
        SetInit(initState);
    }
    public void SetInit(IState<T> initState)
    {
        CurrentState = initState;
        _current.SetFSM = this;
        _current.Enter();
    }
    public void OnUpdate()
    {
        if (_current != null)
            _current.Execute();
    }
    public void OnLateUpdate()
    {
        if (_current != null)
            _current.LateExecute();
    }
    public void Transition(T input)
    {
        IState<T> newState = _current.GetTransition(input);
        if (newState != null)
        {
            _current.Sleep();
            CurrentState = newState;
            _current.SetFSM = this;
            _current.Enter();
        }
    }
}

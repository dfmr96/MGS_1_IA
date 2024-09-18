using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    IMove _move;
    ISpin _spin; //TODO SPIN
    FSM<StateEnum> _fsm;
    void Start()
    {
        _move = GetComponent<IMove>();
        _spin = GetComponent<ISpin>(); //TODO SPIN
        InitializedFSM();
    }
    void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var idle = new PlayerStateIdle(_fsm, _move);
        var move = new PlayerStateMove(_fsm, _move);
        var spin = new PlayerStateSpin(_fsm, _spin); //TODO SPIN

        idle.AddTransition(StateEnum.Move, move);
        idle.AddTransition(StateEnum.Spin, spin); //TODO SPIN

        move.AddTransition(StateEnum.Idle, idle);
        move.AddTransition(StateEnum.Spin, spin); //TODO SPIN

        spin.AddTransition(StateEnum.Idle, idle); //TODO SPIN


        _fsm.SetInitial(idle);
    }
    void Update()
    {
        _fsm.OnUpdate();
    }
    private void FixedUpdate()
    {
        _fsm.OnFixedUpdate();
    }
    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
}

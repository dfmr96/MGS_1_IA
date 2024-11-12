using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle : State<StateEnum>
{
    FSM<StateEnum> _fsm;
    IMove _move;

    public PlayerStateIdle(FSM<StateEnum> fsm, IMove move)
    {
        _fsm = fsm;
        _move = move;
    }

    public override void Enter()
    {
        base.Enter();
        _move.Move(UnityEngine.Vector3.zero);
    }
    public override void Execute()
    {
        base.Execute();
        var h = Input.GetAxis("Horizontal"); //TODO INPUT MANAGER
        var v = Input.GetAxis("Vertical"); //TODO INPUT MANAGER

        if (h != 0 || v != 0) //TODO INPUT MANAGER
        {
            _fsm.Transition(StateEnum.Move); //TODO CAMBIAR A DECISION TREE
        }
        if (Input.GetKeyDown(KeyCode.Space)) //TODO INPUT MANAGER
        {
            _fsm.Transition(StateEnum.Spin); //TODO CAMBIAR A DECISION TREE
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMove : State<StateEnum>
{
    IMove _move;
    FSM<StateEnum> _fsm;
    public PlayerStateMove(FSM<StateEnum> fsm, IMove move)
    {
        _move = move;
        _fsm = fsm;
    }
    public override void FixedExecute()
    {
        var h = Input.GetAxis("Horizontal"); //TODO INPUT MANAGER
        var v = Input.GetAxis("Vertical"); //TODO INPUT MANAGER

        Vector3 dir = new Vector3(h, 0, v); 

        if (h == 0 && v == 0) //TODO INPUT MANAGER
        {
            _fsm.Transition(StateEnum.Idle); //TODO CAMBIAR A DECISION TREE
        }
        else
        {
            _move.Move(dir.normalized);
            _move.Look(dir);
        }

        if (Input.GetKeyDown(KeyCode.Space)) //TODO INPUT MANAGER
        {
            _fsm.Transition(StateEnum.Spin); //TODO CAMBIAR A DECISION TREE
        }
    }
}

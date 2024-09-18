using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraStateIdle : State<StateEnum>
{
    FSM<StateEnum> _fsm;
    LineOfSight _los;
    Transform _target;
    ISpin _spin;

    public CameraStateIdle(FSM<StateEnum> fsm, LineOfSight los, Transform target, ISpin spin)
    {
        _fsm = fsm;
        _los = los;
        _target = target;
        _spin = spin;
    }
    public override void Enter()
    {
        base.Enter();
        Debug.Log("IDLE ENTER");
    }
    public override void Execute()
    {
        base.Execute();
        if ((_spin == null || _spin.IsDetectable) && _los.CheckRange(_target) && _los.CheckAngle(_target) && _los.CheckView(_target))
        {
            _fsm.Transition(StateEnum.Alert);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateAlert : State<StateEnum>
{
    FSM<StateEnum> _fsm;
    LineOfSight _los;
    Transform _target;
    ISpin _spin;
    IAlert _alert;

    public CameraStateAlert(FSM<StateEnum> fsm, LineOfSight los, Transform target, ISpin spin, IAlert alert)
    {
        _fsm = fsm;
        _los = los;
        _target = target;
        _spin = spin;
        _alert = alert;
    }
    public override void Enter()
    {
        base.Enter();
        _alert.IsAlert = true;
        Debug.Log("ALERT ENTER");
    }
    public override void Execute()
    {
        base.Execute();
        if ((_spin != null && !_spin.IsDetectable) || !_los.CheckRange(_target) || !_los.CheckAngle(_target) || !_los.CheckView(_target))
        {
            _fsm.Transition(StateEnum.Idle);
        }
    }
    public override void Exit()
    {
        base.Exit();
        _alert.IsAlert = false;
    }
}

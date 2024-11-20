using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateAlert : State<StateEnum>
{
    private CameraModel _cameraModel;
    private Transform _target;
    public CameraStateAlert(CameraModel camModel, Transform target)
    {
        _cameraModel = camModel;
        _target = target;
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Execute()
    {
        base.Execute();
        _cameraModel.SetRotation(_target);
        //Los
    }
    //Get
}

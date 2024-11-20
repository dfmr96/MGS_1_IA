using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel : MonoBehaviour, IAlert
{
    bool _isAlert;
    [SerializeField] float _angularSpeed;
    [SerializeField] float _minAngle;
    [SerializeField] float _maxAngle;
    [SerializeField] float _angle;
    [SerializeField] private GameObject _body;
    public bool IsAlert
    {
        get
        {
            return _isAlert;
        }
        set
        {
            _isAlert = value;
        }
    }

    private void Start()
    {
        _angle = _body.transform.rotation.y;
    }

    public void RotateBase()
    {
        _angle += _angularSpeed * Time.deltaTime;
        _body.transform.rotation = Quaternion.Euler(0,_angle,0);
        if (_angle >= _maxAngle || _angle <= _minAngle)
        {
            _angularSpeed = -_angularSpeed;
        }
        
    }

    public void SetRotation(Transform target)
    {
        Vector3 dir = (target.position - _body.transform.position).normalized;
        _body.transform.forward = dir;
    }
}

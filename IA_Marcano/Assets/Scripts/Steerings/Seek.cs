using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : ISteering
{
    Transform _entity;
    Transform _target;
    Vector3 _entityPosition;

    public Seek(Transform entity, Transform target)
    {
        _entity = entity;
        _target = target;
    }
    public Seek(Transform target)
    {
        _target = target;
    }

    public virtual Vector3 GetDir()
    {
        //a: entity;
        //b: _target;
        if (_target == null) return Vector3.zero;
        return (_target.position - _entityPosition).normalized;
    }
    public Vector3 Entity
    {
        set
        {
            _entityPosition = value;
        }
    }
    public Transform Target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
        }
    }
}

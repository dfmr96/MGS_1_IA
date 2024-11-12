using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Entity : MonoBehaviour, IMove
{
    Rigidbody _rb;
    public float walkSpeed = 0.6f;
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public virtual void Move(UnityEngine.Vector3 dir)
    {
        dir = dir.normalized;
        dir *= walkSpeed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }

    public void LookDir(UnityEngine.Vector3 dir)
    {
        //throw new System.NotImplementedException(); TODO
    }

    public void SetPosition(UnityEngine.Vector3 pos)
    {
        //throw new System.NotImplementedException(); TODO
    }

    public void Look(UnityEngine.Vector3 dir)
    {
        transform.forward = dir;
    }
    public void Look(Transform target)
    {
        UnityEngine.Vector3 dir = target.position - transform.position;
        Look(dir);
    }

    public void Stop()
    {
        _rb.velocity = UnityEngine.Vector3.zero;
    }
}

using System;
using Enemy.EnemyStates;
using UnityEngine;

[Serializable]
public class RatEvadeState : EnemySteeringState
{
    private RatModel _entity;
    [SerializeField] private float _aggroBuffer;
    [SerializeField] private float _aggroTimer;
    [SerializeField] private bool _isAggroBufferActive;
    [SerializeField] private Rigidbody _rb;
    public bool IsAggroBufferActive => _isAggroBufferActive;
    public RatEvadeState(RatModel entity, Rigidbody rb) : base(entity, new Evade(entity.transform, rb, 0.5f))
    {
        _rb = rb;
        _entity = entity;
        _aggroBuffer = entity.AggroBuffer;
    }
    
    public override void Enter()
    {
        base.Enter();
        _aggroTimer = _aggroBuffer;
        _isAggroBufferActive = true;
        Debug.Log("Enter Evade");
    }
    
    public override void Execute()
    {
        base.Execute();
        _aggroTimer -= Time.deltaTime;

        if (_aggroTimer <= 0)
        {
            _isAggroBufferActive = false;
        }
    }
}
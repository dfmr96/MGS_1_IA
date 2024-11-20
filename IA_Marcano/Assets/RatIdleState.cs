using System;
using UnityEngine;

[Serializable]
public class RatIdleState : State<StateEnum>
{
    private Entity _entity;
    [SerializeField] private float _idleTimer;
    [SerializeField] private float _idleTime;
    private bool _isIdle;
    public bool IsIdle => _isIdle;
    private RatView _ratView;

    public RatIdleState(Entity entity, float idleTime)
    {
        _entity = entity;
        _idleTime = idleTime;
    }

    public override void Enter()
    {
        _idleTimer = 0;
        _isIdle = true;
        _entity.Stop();
    }

    public override void Execute()
    {
        if (!_isIdle) return;
            
        _idleTimer += Time.deltaTime;

        if (_idleTimer > _idleTime)
        {
            _idleTimer = 0;
            _isIdle = false;
        }
    }

    public override void Sleep()
    {
        base.Sleep();
        _isIdle = true;
    }
}
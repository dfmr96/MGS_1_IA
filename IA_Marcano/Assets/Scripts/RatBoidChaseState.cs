using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RatBoidChaseState: StatePathfinding<StateEnum>
{
    private readonly Transform _ratLeader;
    private float repathTime = 2;
    private float repathTimer;
    private Entity _entity;
    private RatModel _ratModel;
    public RatBoidChaseState(Transform ratLeader, Transform entityTransform, IMove move, Animator anim, float distanceToPoint = 0.2f) 
        : base(entityTransform, move, anim, distanceToPoint)
    {
        _ratLeader = ratLeader;
        _entity = _entityTransform.GetComponent<Entity>();
        _ratModel = _entityTransform.GetComponent<RatModel>();
    }

    public override void Enter()
    {
        base.Enter();
        repathTimer = 0;
        _entity.SetSpeed(_ratModel.runSpeed);
        SetPathAStarPlus(_ratLeader.transform.position);
        Debug.Log($"Perdio de vista al leader, pathfind a Leader");
    }

    public override void Execute()
    {
        base.Execute();
        repathTimer += Time.deltaTime;

        if (repathTimer >= repathTime)
        {
            Debug.Log("New Path");
            repathTimer = 0;
            SetPathAStarPlus(_ratLeader.transform.position);
        }
    }
}
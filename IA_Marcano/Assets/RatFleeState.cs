using System;
using System.Collections.Generic;
using UnityEngine;

public class RatFleeState : StatePathfinding<StateEnum>
{
    public Action onFinishPath = delegate { };

    public RatFleeState(Transform entityTransform, IMove move, Animator anim, float maxDistance, float distanceToPoint = 0.2f) 
        : base(entityTransform, move, anim, maxDistance, distanceToPoint)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        SetPathForFlee();
    }
    public void SetPathForFlee()
    {
        start = GetNearNode(_entityTransform.position);
        List<Node> path = ASTAR.Run<Node>(start, IsSatisfiesFlee, GetConnections, GetCostForFlee, HeuristicForFlee);
        //path = ASTAR.CleanPath(path, InView);
        if (path.Count <= 0) return;
        SetWaypoints(GetPathVector(path));
    }

    protected override void OnFinishPath()
    {
        base.OnFinishPath();
        onFinishPath();
    }
}
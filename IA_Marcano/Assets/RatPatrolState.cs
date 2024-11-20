using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RatPatrolState : StatePathfinding<StateEnum>
{
    private List<Node> _patrolWaypoints;
    private int _waypointsIndex;
    private RatView _ratView;

    public RatPatrolState(Transform entityTransform, IMove move, RatView ratView,List<Node> patrolWaypoints, float distanceToPoint = 0.2f) : base(entityTransform, move, ratView.Anim, distanceToPoint)
    {
        _patrolWaypoints = patrolWaypoints;
        _waypointsIndex = 0;
        _ratView = ratView;
    }

    public override void Enter()
    {
        SetPathAStarPlus(_patrolWaypoints[_waypointsIndex].transform.position);
        isFinishPath = false;
    }
        
    protected override void OnFinishPath()
    {
        base.OnFinishPath();
        if (_waypointsIndex < _patrolWaypoints.Count - 1)
        {
            _waypointsIndex++;
        }
        else
        {
            _waypointsIndex = 0;
        }
    }
}
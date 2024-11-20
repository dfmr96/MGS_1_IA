using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class PatrolState : StatePathfinding<StateEnum>
    {
    private List<Node> _patrolWaypoints;
    private int _index;

    public PatrolState(Transform entityTransform, IMove move, EnemyView enemyView,List<Node> patrolWaypoints, float distanceToPoint = 0.2f) : base(entityTransform, move, enemyView.Anim, distanceToPoint)
    {
        _patrolWaypoints = patrolWaypoints;
        _index = 0;
    }

    public override void Enter()
    {
        SetPathAStarPlus(_patrolWaypoints[_index].transform.position);
        isFinishPath = false;
    }
        
    protected override void OnFinishPath()
    {
        base.OnFinishPath();
        if (_index < _patrolWaypoints.Count - 1)
        {
            _index++;
        }
        else
        {
            _index = 0;
        }
    }
    }
}
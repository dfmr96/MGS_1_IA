using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyPatrolState : StatePathfinding<StateEnum>
    {
        private List<Node> _patrolWaypoints;
        private int _index;
        private EnemyView _enemyView;
        private float turnThreshold = 0.5f;

        public EnemyPatrolState(Transform entity, IMove move, EnemyView enemyView,List<Node> patrolWaypoints, float distanceToPoint = 0.2f) : base(entity, move, enemyView.Anim, distanceToPoint)
        {
            _patrolWaypoints = patrolWaypoints;
            _index = 0;
            _enemyView = enemyView;
        }

        public override void Enter()
        {
            SetPathAStarPlus(_patrolWaypoints[_index].transform.position);
            isFinishPath = false;
            _enemyView.OnPatrol(true);
            Debug.Log(isFinishPath);
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
            Debug.Log(isFinishPath);
        }

        public override void Sleep()
        {
            base.Sleep();
            Debug.Log("Salió de Patrol");
            _enemyView.OnPatrol(false);
        }
    }
}
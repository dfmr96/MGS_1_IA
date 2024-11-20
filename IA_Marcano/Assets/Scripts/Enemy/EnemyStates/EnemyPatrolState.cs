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
        private EnemyModel _enemyModel;
        private Entity _entity;

        public EnemyPatrolState(EnemyModel enemyModel,Entity entity, IMove move, EnemyView enemyView,List<Node> patrolWaypoints, float distanceToPoint = 0.2f) : base(entity.transform, move, enemyView.Anim, distanceToPoint)
        {
            _patrolWaypoints = patrolWaypoints;
            _enemyModel = enemyModel;
            _index = 0;
            _enemyView = enemyView;
            _entity = entity;
        }

        public override void Enter()
        {
            SetPathAStarPlus(_patrolWaypoints[_index].transform.position);
            _entity.SetSpeed(_enemyModel.walkSpeed);
            isFinishPath = false;
            _enemyView.OnPatrol(true);
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

        public override void Sleep()
        {
            base.Sleep();
            _enemyView.OnPatrol(false);
        }
    }
}
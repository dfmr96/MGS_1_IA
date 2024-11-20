using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.EnemyStates
{
    [Serializable]
    public class EnemyChaseState : StatePathfinding<StateEnum>
    {
        private EnemyModel _enemyModel;
        private EnemyView _enemyView;
        private Entity _entity;
        public EnemyChaseState(EnemyModel enemyModel,Transform entityTransform, IMove move, EnemyView enemyView, float distanceToPoint = 0.2f) : 
            base(entityTransform, move, enemyView.GetComponent<Animator>(), distanceToPoint)
        {
            _enemyView = enemyView;
            _entity = entityTransform.GetComponent<Entity>();
            _enemyModel = enemyModel;
            AlertManager.Instance.OnLastPlayerPositionChanged += OnPlayerPositionChanged;
        }

        public override void Enter()
        {
            base.Enter();
            _entity.SetSpeed(_enemyModel.RunSpeed);
            _enemyView.OnRunning(true);
            AlertManager.Instance.TryUpdatePlayerLastPosition();
            SetPathAStarPlus(AlertManager.Instance.PlayerLastPosition);
        }

        private void OnPlayerPositionChanged()
        {
            SetPathAStarPlus(AlertManager.Instance.PlayerLastPosition);
        }

        public override void Sleep()
        {
            base.Sleep();
            _enemyView.OnRunning(false);
        }
    }
}
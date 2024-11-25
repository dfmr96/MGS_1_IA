using System;
using UnityEngine;

namespace Enemy.EnemyStates
{
    
    [Serializable]
    public class EnemyIdleState : State<StateEnum>
    {
        private readonly Entity _entity;
        [SerializeField] private float _idleTimer;
        [SerializeField] private float _idleTime;
        private IPatrol _patrol;
        private bool _isIdle;
        public bool IsIdle => _isIdle;
        private EnemyView _enemyView;
        private EnemyModel _enemyModel;

        public EnemyIdleState(EnemyModel enemyModel, Entity entity, float idleTime, EnemyView enemyView)
        {
            _entity = entity;
            _idleTime = idleTime;
            _enemyView = enemyView;
            _enemyModel = enemyModel;
        }

        public override void Enter()
        {
            Debug.Log($"{_entity.gameObject} entró en Idle");
            _isIdle = true;
            _entity.SetSpeed(_enemyModel.WalkSpeed);
            _enemyView.OnIdle(true);
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
                _enemyView.OnIdle(_isIdle);
            }
        }

        public override void Sleep()
        {
            base.Sleep();
            Debug.Log($"Salió de Idle {IsIdle}");
            _enemyView.OnIdle(false);
            _isIdle = true;
        }
    }
}
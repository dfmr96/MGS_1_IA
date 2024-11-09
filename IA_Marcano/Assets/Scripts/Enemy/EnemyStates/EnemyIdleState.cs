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

        public EnemyIdleState(Entity entity, float idleTime, float idleTimer, EnemyView enemyView)
        {
            _entity = entity;
            _idleTime = idleTime;
            _idleTimer = idleTimer;
            _enemyView = enemyView;
        }

        public override void Enter()
        {
            Debug.Log("Entró en Idle");
            _isIdle = true;
            _enemyView.OnIdle(_isIdle);
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
            _isIdle = true;
        }
    }
}
using System;
using UnityEngine;

namespace Enemy.EnemyStates
{
    [Serializable]
    public class EnemyPursuitState : EnemySteeringState
    {
        private EnemyModel _enemyModel;
        private EnemyView _enemyView;
        private float _aggroBuffer;
        [SerializeField] private float _aggroTimer;
        [SerializeField] private bool _isAggroBufferActive;
        private Entity _entity;
        public bool IsAggroBufferActive => _isAggroBufferActive;
        public EnemyPursuitState(EnemyModel enemyModel, Rigidbody rb, float timePrediction, EnemyView enemyView) : base(enemyModel, new Pursuit(enemyModel.transform, rb, timePrediction))
        {
            _enemyView = enemyView;
            _enemyModel = enemyModel;
            _entity = enemyModel.GetComponent<Entity>();
            _aggroBuffer = enemyModel.aggroBuffer;
        }

        public override void Enter()
        {
            base.Enter();
            _entity.SetSpeed(_enemyModel.runSpeed);
            _aggroTimer = _aggroBuffer;
            _isAggroBufferActive = true;
            _enemyView.OnRunning(true);
        }

        public override void Execute()
        {
            base.Execute();
            _aggroTimer -= Time.deltaTime;

            if (_aggroTimer <= 0)
            {
                _isAggroBufferActive = false;
            }
        }

        public override void Sleep()
        {
            base.Sleep();
            _enemyView.OnRunning(false);
            AlertManager.Instance.TryUpdatePlayerLastPosition();
        }
    }
}
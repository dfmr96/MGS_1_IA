using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.EnemyStates
{
    [Serializable]
    public class EnemyPursuitState : EnemySteeringState
    {
        private EnemyModel _enemyModel;
        private EnemyView _enemyView;
        private Entity _entity;
        private float _aggroBuffer;
        [SerializeField] private float aggroTimer;
        [SerializeField] private bool isAggroBufferActive;
        public bool IsAggroBufferActive => isAggroBufferActive;
        public EnemyPursuitState(EnemyModel enemyModel, Rigidbody rb, float timePrediction, EnemyView enemyView) : base(enemyModel, new Pursuit(enemyModel.transform, rb, timePrediction))
        {
            _enemyView = enemyView;
            _enemyModel = enemyModel;
            _entity = enemyModel.GetComponent<Entity>();
            _aggroBuffer = enemyModel.AggroBuffer;
        }

        public override void Enter()
        {
            base.Enter();
            _entity.SetSpeed(_enemyModel.RunSpeed);
            aggroTimer = _aggroBuffer;
            isAggroBufferActive = true;
            _enemyView.OnRunning(true);
        }

        public override void Execute()
        {
            base.Execute();
            aggroTimer -= Time.deltaTime;

            if (aggroTimer <= 0)
            {
                isAggroBufferActive = false;
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
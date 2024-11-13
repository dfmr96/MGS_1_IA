using System;
using UnityEngine;

namespace Enemy.EnemyStates
{
    [Serializable]
    public class EnemyPursuitState : EnemySteeringState
    {
        private EnemyModel _entity;
        private float _aggroBuffer;
        [SerializeField] private float _aggroTimer;
        [SerializeField] private bool _isAggroBufferActive;
        public bool IsAggroBufferActive => _isAggroBufferActive;
        public EnemyPursuitState(EnemyModel entity, Rigidbody rb, float timePrediction) : base(entity, new Pursuit(entity.transform, rb, timePrediction))
        {
            _entity = entity;
            _aggroBuffer = entity.aggroBuffer;
        }

        public override void Enter()
        {
            base.Enter();
            _aggroTimer = _aggroBuffer;
            _isAggroBufferActive = true;
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
            AlertManager.Instance.UpdatePlayerLastPosition(Constants.Player.transform.position);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemySteeringState : State<StateEnum>
    {
        IMove _move;
        ISteering _steering;
        private Rigidbody _target;
        private Transform _transform;
        private Dictionary<SteeringMode, ISteering> _steeringBehaviors;
        private Cooldown _evadeCooldown;
        
        public EnemySteeringState(IMove move, ISteering steering, Dictionary<SteeringMode, ISteering> steeringBehaviors, Cooldown evadeCooldown)
        {
            _move = move;
            _steering = steering;
            _steeringBehaviors = steeringBehaviors;
            _evadeCooldown = evadeCooldown;
            _evadeCooldown = new Cooldown(1f, false, () => _steering = _steeringBehaviors[SteeringMode.Pursuit]);
        }

        public override void Execute()
        {
            base.Execute();
            if (_steering == _steeringBehaviors[SteeringMode.Evade])
            {
                Debug.Log(_evadeCooldown.OnCooldown());
                _evadeCooldown.RunCooldown();
            }
            
            UnityEngine.Vector3 dir = _steering.GetDir();
            _move.Move(dir.normalized);
        }

        /*public override void Exit()
        {
            if (_steering == _steeringBehaviors[SteeringMode.Evade])
            {
                BeginEvade();
            }
        }*/
        
        void BeginEvade()
        {
            _steering = _steeringBehaviors[SteeringMode.Evade];
            _evadeCooldown.SetTimer(1f,true);
        }
    }
}
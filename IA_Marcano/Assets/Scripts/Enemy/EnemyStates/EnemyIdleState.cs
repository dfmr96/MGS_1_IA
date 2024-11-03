using System;
using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyIdleState : State<StateEnum>
    {
        private readonly IMove _move;
        private float _idleTimer;
        private float _idleTime;
        private IPatrol _patrol;
        public Action onFinishIdle();
        public bool isIdle;

        public EnemyIdleState(IMove move, float idleTime, ref float idleTimer, IPatrol patrol)
        {
            _move = move;
            _idleTime = idleTime;
            _idleTimer = idleTimer;
            _patrol = patrol;
        }

        public override void Enter()
        {
            _move.Stop();
            _idleTimer = 0f;
        }

        public override void Execute()
        {
            _idleTimer += Time.deltaTime;

            if (_idleTimer > _idleTime)
            {
                _patrol.RemainingWaypointsToRest = _patrol.CurrentWaypoint;
                //Event TODO
            }
        }
    }
}
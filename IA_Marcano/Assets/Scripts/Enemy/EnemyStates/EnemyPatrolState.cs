using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyPatrolState: State<StateEnum>//, IPAtrol TODO
    {
        private IPatrol _patrol;
        private IMove _move;
        private Transform _transform;

        public EnemyPatrolState(IPatrol patrol, IMove move, Transform transform)
        {
            _patrol = patrol;
            _move = move;
            _transform = transform;
            _patrol.RemainingWaypointsToRest = _patrol.WaypointsToRest;
        }
        
        public override void Enter()
        {
            _patrol.RemainingWaypointsToRest = _patrol.WaypointsToRest;
        }

        public override void Execute()
        {
            if (IsAtCurrentWaypoint())
            {
                HandleWaypointArrival();
            }
            else
            {
                MoveTowardsCurrentWaypoint();
            }
        }

        private void HandleWaypointArrival()
        {
            _patrol.RemainingWaypointsToRest--;
            UpdateWaypoint();
        }
        private void UpdateWaypoint()
        {
            //TODO Podria volverse un action;
            if (_patrol.IsReversing)
            {
                MoveToPreviousWaypoint();
            }
            else
            {
                MoveToNextWaypoint();
            }
        }

        private void MoveToPreviousWaypoint()
        {
            if (_patrol.CurrentWaypoint - 1 < 0)
            {
                _patrol.IsReversing = false;
            }
            else
            {
                _patrol.CurrentWaypoint--;
            }        
        }
        private void MoveToNextWaypoint()
        {
            if (_patrol.CurrentWaypoint + 1 >= _patrol.Waypoints.Length)
            {
                _patrol.IsReversing = true;
            }
            else
            {
                _patrol.CurrentWaypoint++;
            }
        }

        private bool IsAtCurrentWaypoint()
        {
            return Vector3.Distance(_transform.position,  _patrol.Waypoints[_patrol.CurrentWaypoint].position) <
                   _patrol.WaypointDistanceThreshold;
        }
        private void MoveTowardsCurrentWaypoint()
        {
            _move.Move(_patrol.Waypoints[_patrol.CurrentWaypoint].position - _transform.transform.position);
        }
    }
}
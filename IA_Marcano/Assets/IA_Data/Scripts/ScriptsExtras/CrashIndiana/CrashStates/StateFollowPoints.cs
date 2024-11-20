using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class StateFollowPoints<T> : State<T>
{
    protected List<UnityEngine.Vector3> _waypoints;
    int _index;
    protected Transform _entityTransform;
    float _distanceToPoint = 0.2f;

    protected bool isFinishPath;
    public bool IsFinishPath => isFinishPath;
    public StateFollowPoints(Transform entityTransform, float distanceToPoint = 0.2f)
    {
        _entityTransform = entityTransform;
        _distanceToPoint = distanceToPoint;
        isFinishPath = true;
    }
    public StateFollowPoints(Transform entityTransform, List<UnityEngine.Vector3> waypoints, float distanceToPoint = 0.2f)
    {
        _entityTransform = entityTransform;
        _distanceToPoint = distanceToPoint;
        _waypoints = waypoints;
        isFinishPath = true;
    }


    public override void Execute()
    {
        base.Execute();
        Run();
    }
    public void SetWaypoints(List<Vector3> newPoints)
    {
        if (newPoints.Count == 0 || newPoints == _waypoints) return;
        _waypoints = newPoints;
        Vector3 dirToPoint = (_waypoints[0] - _entityTransform.position).normalized;
        bool skipFirstPoint = Vector3.Dot(dirToPoint, _entityTransform.forward) <= 0;
        _index = skipFirstPoint ? 1 : 0;
        isFinishPath = false;
        OnStartPath();
    }
    void Run()
    {
        if (IsFinishPath) return;
        Vector3 point = _waypoints[_index];
        point.y = _entityTransform.position.y; //Horizontal move
        Vector3 dir = point - _entityTransform.position;
        if (dir.magnitude < _distanceToPoint)
        {
            if (_index + 1 < _waypoints.Count)
                _index++;
            else
            {
                isFinishPath = true;
                OnFinishPath();
                return;
            }
        }
        OnMove(dir.normalized);
    }
    protected virtual void OnMove(UnityEngine.Vector3 dir)
    {

    }
    protected virtual void OnStartPath()
    {

    }
    protected virtual void OnFinishPath()
    {
        //Debug.Log("camino  terminado");
        isFinishPath = true;
        //Debug.Log(isFinishPath);
    }

}

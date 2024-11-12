using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance
{
    Transform _entity;
    float _radius;
    float _angle;
    float _personalArea;
    Collider[] _colls;

    public ObstacleAvoidance(Transform entity, float radius, float angle, float personalArea, int countMaxObs = 5)
    {
        _entity = entity;
        _radius = radius;
        //_radius = Mathf.Min(_radius, 1);
        _angle = angle;
        _colls = new Collider[countMaxObs];
        _personalArea = personalArea;
    }

    public UnityEngine.Vector3 GetDir(UnityEngine.Vector3 currDir, bool calculateY = true)
    {

        int count = Physics.OverlapSphereNonAlloc(_entity.position, _radius, _colls, Constants.obsMask);

        Collider nearColl = null;
        float nearCollDistance = 0;
        UnityEngine.Vector3 nearClosetPoint = UnityEngine.Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            var currColl = _colls[i];
            UnityEngine.Vector3 closetPoint = currColl.ClosestPoint(_entity.position);
            if (!calculateY) closetPoint.y = _entity.position.y;
            UnityEngine.Vector3 dirToColl = closetPoint - _entity.position;
            float distance = dirToColl.magnitude;
            float currAngle = UnityEngine.Vector3.Angle(dirToColl, currDir);
            if (currAngle > _angle / 2) continue;

            if (nearColl == null || distance < nearCollDistance)
            {
                nearColl = currColl;
                nearCollDistance = distance;
                nearClosetPoint = closetPoint;
            }
        }

        if (nearColl == null)
        {
//            Debug.Log(currDir);
            return currDir;
        }

        UnityEngine.Vector3 relativePos = _entity.InverseTransformPoint(nearClosetPoint);
        UnityEngine.Vector3 dirToClosetPoint = (nearClosetPoint - _entity.position).normalized;
        UnityEngine.Vector3 newDir;
        if (relativePos.x < 0)
        {
            newDir = UnityEngine.Vector3.Cross(_entity.up, dirToClosetPoint);
        }
        else
        {
            newDir = -UnityEngine.Vector3.Cross(_entity.up, dirToClosetPoint);
        }
        Debug.DrawRay(_entity.position, newDir, Color.red);
        return UnityEngine.Vector3.Lerp(currDir, newDir, (_radius - Mathf.Clamp(nearCollDistance - _personalArea, 0, _radius)) / _radius);
    }
}

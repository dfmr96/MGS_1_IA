using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RatModel : Entity, IBoid
{
    [SerializeField] private float aggroBuffer;
    public float AggroBuffer => aggroBuffer;
    
    public float walkSpeed;
    public float runSpeed;
    
    [Header("Obstacle Avoidance")]
    public float radius;
    public float angle;
    public float personalArea;
    ObstacleAvoidance _obs;
    public ObstacleAvoidance Obs => _obs;
    protected override void Awake()
    {
        base.Awake();
        _obs = new ObstacleAvoidance(transform, radius, angle, personalArea, Constants.obsMask);
    }
    
    public override void Move(Vector3 dir)
    {
        dir = _obs.GetDir(dir, false);
        dir.y = 0;
        LookDir(dir);
        base.Move(dir);
    }
    
    private void OnDrawGizmosSelected()
    {
        Color myColor = Color.cyan;
        myColor.a = 0.5f;
        Gizmos.color = myColor;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, personalArea);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius);
    }

    public bool InView(LineOfSight los, Transform target)
    {
        bool inViewCurrentFrame = los.CheckRange(target.transform)
                                  && los.CheckAngle(target.transform)
                                  && los.CheckView(target.transform);
        
        return inViewCurrentFrame;
    }

    public Vector3 Position => transform.position;
    public Vector3 Forward => transform.forward;
}

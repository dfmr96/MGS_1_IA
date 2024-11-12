using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISteering
{
    UnityEngine.Vector3 GetDir();
}

public enum SteeringMode
{
    Seek,
    Flee,
    Pursuit,
    Evade
}

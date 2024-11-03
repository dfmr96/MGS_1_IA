using Unity.VisualScripting;
using UnityEngine;

public interface IPatrol
{
    Transform[] Waypoints { get; set; }
    int CurrentWaypoint { get; set;}
    float WaypointDistanceThreshold { get; set; }
    int WaypointsToRest { get; set; }
    int RemainingWaypointsToRest { get; set; }
    bool IsReversing { get; set; }
}
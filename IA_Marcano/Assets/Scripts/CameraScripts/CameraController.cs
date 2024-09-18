using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    LineOfSight _los;
    IAlert _alert;
    ISpin _spin;
    private void Awake()
    {
        _los = GetComponent<LineOfSight>();
        _alert = GetComponent<IAlert>();
        _spin = target.GetComponent<ISpin>();
    }
    // Update is called once per frame
    void Update()
    {
        if ((_spin == null || _spin.IsDetectable) && _los.CheckRange(target) && _los.CheckAngle(target) && _los.CheckView(target))
        {
            _alert.IsAlert = true;
            print("Camera alert: True");
        }
        else
        {
            _alert.IsAlert = false;
            print("Camera alert: False");
        }
    }
}

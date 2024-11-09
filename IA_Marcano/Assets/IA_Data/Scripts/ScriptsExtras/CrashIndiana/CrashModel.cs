using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashModel : MonoBehaviour, IMove
{
    public float speed = 2;
    public float speedRot = 10;
    public void Move(Vector3 dir)
    {
        dir.y = 0;
        transform.position += Time.deltaTime * dir * speed; ;
    }
    public void LookDir(Vector3 dir)
    {
        if (Vector3.Angle(transform.forward, dir) > (Mathf.PI * Mathf.Rad2Deg) / 2)
        {
            transform.forward = dir;
        }
        else
        {
            transform.forward = Vector3.Lerp(transform.forward, dir, speedRot * Time.deltaTime);
        }
    }
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}

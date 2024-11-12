using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove
{
    void Move(UnityEngine.Vector3 dir);
    void LookDir(UnityEngine.Vector3 dir);
    void SetPosition(UnityEngine.Vector3 pos);
}

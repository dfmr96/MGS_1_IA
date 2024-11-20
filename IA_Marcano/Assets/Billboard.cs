using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
}

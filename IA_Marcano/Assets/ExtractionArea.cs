using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerModel>() != null)
        {
            GameManager.Instance.Victory();
        }
    }
}

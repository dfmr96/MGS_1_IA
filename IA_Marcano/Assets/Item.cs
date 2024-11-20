using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public TMP_Text itemText;

    private void Start()
    {
        itemText.SetText(itemName);
    }

    private void Update()
    {
        transform.Rotate(0,1,0);
    }
}

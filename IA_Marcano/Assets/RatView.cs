using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatView : MonoBehaviour
{
    private Animator _anim;
    public Animator Anim => _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
}

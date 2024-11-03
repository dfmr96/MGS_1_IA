using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    Rigidbody _rb;
    [SerializeField] Animator _anim;
    PlayerModel _playerModel;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerModel = GetComponent<PlayerModel>();
    }

    private void Start()
    {
        _playerModel.OnDamaged += PlayDeadAnim;
    }

    private void PlayDeadAnim()
    {
        _anim.SetBool("isDead", true);
    }

    private void Update()
    {
        _anim.SetFloat("Vel", _rb.velocity.magnitude);
    }
}

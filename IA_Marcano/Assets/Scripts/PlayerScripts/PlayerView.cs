using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    Rigidbody _rb;
    [SerializeField] Animator _anim;
    PlayerModel _playerModel;
    
    [SerializeField] private Slider healthBar;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerModel = GetComponent<PlayerModel>();
    }

    private void Start()
    {
        _playerModel.OnDead += PlayDeadAnim;
        _playerModel.OnDamaged += UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        healthBar.value = _playerModel.Health / _playerModel.MaxHealth;
    }

    private void PlayDeadAnim()
    {
        _anim.SetBool("isDead", true);
    }

    public void OnAiming(bool isAiming)
    {
        _anim.SetBool("Aiming", isAiming);
    }

    private void Update()
    {
        _anim.SetFloat("Vel", _rb.velocity.magnitude);
    }
}

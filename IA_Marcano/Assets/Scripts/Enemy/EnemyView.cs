using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [SerializeField]
    Animator _anim;
    Rigidbody _rb;
    IAttack _attack;

    public Animator Anim => _anim;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _attack = GetComponent<IAttack>();
        _anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Anim.SetFloat("Vel", _rb.velocity.magnitude);
    }
    void OnAttackAnim()
    {
        Anim.SetTrigger("Attack");
    }

    public void OnIdle(bool isIdle)
    {
        Anim.SetBool("isIdle", isIdle);
    }
    
    public void OnPatrol(bool isPatrol)
    {
        Anim.SetBool("isPatrol", isPatrol);
    }

    public void OnDead(bool isDead)
    {
        Anim.SetBool("isDead", isDead);
    }

    public void OnAttack(bool isAttacking)
    {
        Anim.SetBool("isAttack", isAttacking);
    }

    public void OnRunning(bool isRunning)
    {
        Anim.SetBool("isRunning", isRunning);
    }

    public void TurnAround()
    {
        Anim.SetTrigger("TurnAround");
    }
}

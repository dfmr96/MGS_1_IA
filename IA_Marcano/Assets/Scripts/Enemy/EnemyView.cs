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
    private void Start()
    {
        _attack.OnAttack += OnAttackAnim;
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

    public void TurnAround()
    {
        Anim.SetTrigger("TurnAround");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Entity
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool IsDead { get; private set; } = false;
    public event Action OnDamaged;
    public Action OnDead;

    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        OnDamaged?.Invoke();
        if (health <= 0)
        {
            OnDead?.Invoke();
        }
    }
    
}

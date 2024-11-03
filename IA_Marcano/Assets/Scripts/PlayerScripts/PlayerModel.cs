using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Entity
{
    public bool IsDead { get; private set; } = false;
    bool _isDetectable = true;
    public event Action OnDamaged;
    public void TakeDamage()
    {
        IsDead = true;
        Debug.Log("Dead");
        GameManager.Instance.GameOver();
        OnDamaged?.Invoke();
    }
}

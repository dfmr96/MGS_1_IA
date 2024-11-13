using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : Entity, IAttack
{
    public LayerMask attackMask;
    [SerializeField] LineOfSight _attackOfSight;
    Action _onAttack;
    public float attackCooldownTime;
    Cooldown _attackCooldown;
    public float aggroBuffer;
    
    [Header("Obstacle Avoidance")]
    public float radius;
    public float angle;
    public float personalArea;
    ObstacleAvoidance _obs;

    public float GetAttackRange => _attackOfSight.range;

    public Action OnAttack { get => _onAttack; set => _onAttack = value; }
    public Cooldown AttackCooldown { get => _attackCooldown; }

    //Collider[] _enemies = new Collider[5];
    protected override void Awake()
    {
        base.Awake();
        _attackCooldown = new Cooldown(attackCooldownTime);
        _obs = new ObstacleAvoidance(transform, radius, angle, personalArea, Constants.obsMask);
    }
    public void Attack()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _attackOfSight.range, attackMask);
        //int count = Physics.OverlapSphereNonAlloc(transform.position, _attackOfSight.range, _enemies, attackMask);
        foreach (var item in colls)
        {
            var currTarget = item.transform;
            if (!_attackOfSight.CheckAngle(currTarget)) continue;
            if (!_attackOfSight.CheckView(currTarget)) continue;
            //
            if (currTarget.TryGetComponent(out PlayerModel _playerModel))
            {
                if (_playerModel.IsDead) return;
                _playerModel.TakeDamage();
            }
            break;
        }
        _attackCooldown.ResetCooldown();
        _onAttack();
    }
    public override void Move(UnityEngine.Vector3 dir)
    {
        dir = _obs.GetDir(dir, false);
        dir.y = 0;
        Look(dir);
        base.Move(dir);
    }
    private void OnDrawGizmosSelected()
    {
        Color myColor = Color.cyan;
        myColor.a = 0.5f;
        Gizmos.color = myColor;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, personalArea);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius);
    }


    
}
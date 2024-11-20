using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyModel : Entity, IAttack
    {
        Cooldown _attackCooldown;
        Action _onAttack;
        
        [Header("Stats")] 
        [SerializeField] private float runSpeed = 1.2f;
        [SerializeField] private float walkSpeed = 0.6f;
        [SerializeField] private LayerMask attackMask;
        [SerializeField] private float idleTime;
        [SerializeField] private LineOfSight attackOfSight;
        [SerializeField] private float attackCooldownTime;
        [SerializeField] private float aggroBuffer;
        [SerializeField] private float damage = 1;
        [SerializeField] private float maxHealth;
    
        [Header("Line of Sights")] 
        [SerializeField] private LineOfSight _aggroLoS;
        [SerializeField] private LineOfSight _attackLoS;
        [SerializeField] private LineOfSight _detectionLoS;
        public LineOfSight AggroLoS => _aggroLoS;
        public LineOfSight AttackLoS => _attackLoS;
        public LineOfSight DetectionLoS => _detectionLoS;
        
        [Header("Obstacle Avoidance")]
        public float radius;
        public float angle;
        public float personalArea;
        ObstacleAvoidance _obs;

        public float GetAttackRange => attackOfSight.range;

        public Action OnAttack { get => _onAttack; set => _onAttack = value; }
        public Cooldown AttackCooldown { get => _attackCooldown; }

        public float RunSpeed => runSpeed;

        public float WalkSpeed => walkSpeed;

        public float IdleTime => idleTime;
        
        public float AggroBuffer => aggroBuffer;
        public float MaxHealth => maxHealth;


        //Collider[] _enemies = new Collider[5];
        protected override void Awake()
        {
            base.Awake();
            _attackCooldown = new Cooldown(attackCooldownTime);
            _obs = new ObstacleAvoidance(transform, radius, angle, personalArea, Constants.obsMask);
        }
        public void Attack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, attackOfSight.range, attackMask);
            //int count = Physics.OverlapSphereNonAlloc(transform.position, _attackOfSight.range, _enemies, attackMask);
            foreach (var item in colls)
            {
                var currTarget = item.transform;
                if (!attackOfSight.CheckAngle(currTarget)) continue;
                if (!attackOfSight.CheckView(currTarget)) continue;
                //
                if (currTarget.TryGetComponent(out PlayerModel _playerModel))
                {
                    if (_playerModel.IsDead) return;
                    _playerModel.TakeDamage(damage);
                }
                break;
            }
            _attackCooldown.ResetCooldown();
        }
        public override void Move(UnityEngine.Vector3 dir)
        {
            dir = _obs.GetDir(dir, false);
            dir.y = 0;
            LookDir(dir);
            base.Move(dir);
        }
    
        public bool InView(LineOfSight los, Transform target)
        {
            bool inViewCurrentFrame = los.CheckRange(target.transform)
                                      && los.CheckAngle(target.transform)
                                      && los.CheckView(target.transform);

            if (inViewCurrentFrame && (!AlertManager.Instance.isOnAlert || AlertManager.Instance.isOnEvasion))
            {
                AlertManager.Instance.CallAlert();
            }
            return inViewCurrentFrame;
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
}
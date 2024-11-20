using System.Collections.Generic;
using Enemy.EnemyStates;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        //Components
        private PlayerController _player;
        private Rigidbody _playerRb;
        private IAttack _entityAttack;
        private ITreeNode _root;
        private ISteering _steering;
        private Cooldown _evadeCooldown;
        private EnemyView _enemyView;
        private EnemyModel _enemyModel;
        private EnemyAudio _enemyAudio;
        
        //Stats
        [SerializeField] private float health;
        private float _idleTime;
        
        //LoS
        private LineOfSight _aggroLoS;
        private LineOfSight _attackLoS;
        private LineOfSight _detectionLoS;
        
        [SerializeField] private List<Node> waypoints;
        
        //States
        [Header("States")]  
        [SerializeField] private FSM<StateEnum> fsm;
        [SerializeField] private EnemyPatrolState _patrolState;
        [SerializeField] private EnemyIdleState idleState;
        [SerializeField] private EnemyPursuitState pursuitState;
        [SerializeField] private EnemyChaseState chaseState;

        private bool IsDead => health <= 0;


        private void Start()
        {
            InitComponents();
            InitLoS();
            InitializedFSM();
            InitializedTree();
        }

        private void InitComponents()
        {
            _player = Constants.Player;
            _playerRb = _player.GetComponent<Rigidbody>();
            _enemyView = GetComponent<EnemyView>();
            _entityAttack = GetComponent<IAttack>();
            _enemyAudio = GetComponent<EnemyAudio>();
            _enemyModel = GetComponent<EnemyModel>();
            _idleTime = _enemyModel.IdleTime;
            health = _enemyModel.MaxHealth;
        }

        private void InitLoS()
        {
            _aggroLoS = _enemyModel.AggroLoS;
            _attackLoS = _enemyModel.AttackLoS;
            _detectionLoS = _enemyModel.DetectionLoS;
        }

        private void InitializedFSM()
        {
            idleState = new EnemyIdleState(_enemyModel, _enemyModel, _idleTime, _enemyView);
            pursuitState = new EnemyPursuitState(_enemyModel, _playerRb, 0.5f, _enemyView);
            var attackState = new EnemyAttackState(_entityAttack, _enemyModel, _enemyView);
            _patrolState = new EnemyPatrolState(_enemyModel, _enemyModel, _enemyModel, _enemyView, waypoints);
            chaseState = new EnemyChaseState(_enemyModel, _enemyModel.transform, _enemyModel, _enemyView);
            var deadState = new EnemyDeadState(_enemyView, _enemyAudio);

            idleState.AddTransition(StateEnum.Attack, attackState);
            idleState.AddTransition(StateEnum.Pursuit, pursuitState);
            idleState.AddTransition(StateEnum.Patrol, _patrolState);
            idleState.AddTransition(StateEnum.Chase, chaseState);
            idleState.AddTransition(StateEnum.Dead, deadState);

            pursuitState.AddTransition(StateEnum.Attack, attackState);
            pursuitState.AddTransition(StateEnum.Idle, idleState);
            pursuitState.AddTransition(StateEnum.Patrol, _patrolState);
            pursuitState.AddTransition(StateEnum.Chase, chaseState);
            pursuitState.AddTransition(StateEnum.Dead, deadState);

            attackState.AddTransition(StateEnum.Pursuit, pursuitState);
            attackState.AddTransition(StateEnum.Patrol, _patrolState);
            attackState.AddTransition(StateEnum.Idle, idleState);
            attackState.AddTransition(StateEnum.Chase, chaseState);
            attackState.AddTransition(StateEnum.Dead, deadState);

            _patrolState.AddTransition(StateEnum.Idle, idleState);
            _patrolState.AddTransition(StateEnum.Pursuit, pursuitState);
            _patrolState.AddTransition(StateEnum.Attack, attackState);
            _patrolState.AddTransition(StateEnum.Chase, chaseState);
            _patrolState.AddTransition(StateEnum.Dead, deadState);

            chaseState.AddTransition(StateEnum.Idle, idleState);
            chaseState.AddTransition(StateEnum.Patrol, _patrolState);
            chaseState.AddTransition(StateEnum.Pursuit, pursuitState);
            chaseState.AddTransition(StateEnum.Attack, attackState);
            chaseState.AddTransition(StateEnum.Dead, deadState);

            fsm = new FSM<StateEnum>(idleState);
        }

        private void InitializedTree()
        {
            var idle = new ActionTree(() => fsm.Transition(StateEnum.Idle));
            var pursuit = new ActionTree(() => fsm.Transition(StateEnum.Pursuit));
            var attack = new ActionTree(() => fsm.Transition(StateEnum.Attack));
            var patrol = new ActionTree(() => fsm.Transition(StateEnum.Patrol));
            var chase = new ActionTree(() => fsm.Transition(StateEnum.Chase));
            var dead = new ActionTree(() => fsm.Transition(StateEnum.Dead));
            
            var qInIdle = new QuestionTree(() => idleState.IsIdle, idle, patrol);
            var qInPatrol = new QuestionTree(() => _patrolState.IsFinishPath, qInIdle, patrol);
            var qInViewNoAttackRange = new QuestionTree(InAggro, pursuit, chase);
            var qAttackRange = new QuestionTree(InAttackRange, attack, qInViewNoAttackRange);
            var qInViewNoEvasion = new QuestionTree(InDetectionView, pursuit, qInPatrol);
            var qInViewEvasion = new QuestionTree(InAggro, pursuit, chase); //TODO cambiar de chase a search
            var qIsOnEvasion =
                new QuestionTree(() => AlertManager.Instance.isOnEvasion, qInViewEvasion, qInViewNoEvasion);
            var qIsOnAlert = new QuestionTree(() => AlertManager.Instance.isOnAlert, qAttackRange, qIsOnEvasion);


            var qIsExist = new QuestionTree(() => _player != null, qIsOnAlert, qInPatrol);
            var qIsAlive = new QuestionTree(() => IsDead, dead, qIsExist); 

            _root = qIsAlive;
        }

        private bool InView(LineOfSight los)
        {
            return _enemyModel.InView(los, _player.transform);
        }
        
        //Controller
        //InView -| Los
        
        //Model
        //IsplayerFound -| Los -| PlayerFound
        //Los
        //  lastFrame == currentFrame

        private bool InDetectionView()
        {
            return InView(_detectionLoS);
        }

        private bool InAggro()
        {
            return InView(_aggroLoS) || pursuitState.IsAggroBufferActive;
        }

        bool InAttackRange()
        {
            return InView(_attackLoS);
        }

        private void Update()
        {
            fsm.OnUpdate();
            _root.Execute();
        }

        private void LateUpdate()
        {
            fsm.OnLateUpdate();
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            _enemyAudio.PlayRandomDeathAudio();
        }
    }
}

using System.Collections.Generic;
using Enemy.EnemyStates;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyModel _enemyModel;
        public PlayerController player;
        public Rigidbody target;
        public LineOfSight aggroLoS;
        public LineOfSight attackLoS;
        public LineOfSight detectionLoS;
        public float timePrediction;
        [SerializeField] FSM<StateEnum> _fsm;
        IAttack _entityAttack;
        ITreeNode _root;
        ISteering _steering;
        Dictionary<SteeringMode, ISteering> _steeringBehaviors = new Dictionary<SteeringMode, ISteering>();
        Cooldown _evadeCooldown;
        public float idleTime;
        public float idleTimer;
        

        private EnemyView _enemyView;
        
        [SerializeField] private List<Node> waypoints;
        [SerializeField] private EnemyPatrolState patrolState;
        [SerializeField] private EnemyIdleState idleState;
        [SerializeField] private EnemyPursuitState _pursuitState;


        private void Start()
        {
            player = Constants.Player;
            target = player.GetComponent<Rigidbody>();
            _enemyView = GetComponent<EnemyView>();
            _entityAttack = GetComponent<IAttack>();
            InitializedFSM();
            InitializedTree();
        }

        void InitializedFSM()
        {
            //var pursuit = new Pursuit(transform, target, timePrediction);
            var evade = new Evade(transform, target, timePrediction);

            idleState = new EnemyIdleState(_enemyModel, idleTime, idleTimer, _enemyView);
            //var steering = new EnemySteeringState(_enemyModel,pursuit);
            _pursuitState = new EnemyPursuitState(_enemyModel, target, 0.5f); 
            var attackState = new EnemyAttackState(_entityAttack, _enemyModel);
            patrolState = new EnemyPatrolState(_enemyModel.transform, _enemyModel, _enemyView, waypoints);
            var chaseState = new EnemyChaseState(_enemyModel.transform, _enemyModel, _enemyView);

            
            idleState.AddTransition(StateEnum.Attack, attackState);
            idleState.AddTransition(StateEnum.Pursuit, _pursuitState);
            idleState.AddTransition(StateEnum.Patrol, patrolState);
            idleState.AddTransition(StateEnum.Chase, chaseState);

            _pursuitState.AddTransition(StateEnum.Attack, attackState);
            _pursuitState.AddTransition(StateEnum.Idle, idleState);
            _pursuitState.AddTransition(StateEnum.Patrol, patrolState);
            _pursuitState.AddTransition(StateEnum.Chase, chaseState);
        
            attackState.AddTransition(StateEnum.Pursuit, _pursuitState);
            attackState.AddTransition(StateEnum.Patrol, patrolState);
            attackState.AddTransition(StateEnum.Idle, idleState);
            attackState.AddTransition(StateEnum.Chase, chaseState);

            patrolState.AddTransition(StateEnum.Idle, idleState);
            patrolState.AddTransition(StateEnum.Pursuit, _pursuitState);
            patrolState.AddTransition(StateEnum.Attack, attackState);
            patrolState.AddTransition(StateEnum.Chase, chaseState);
            
            chaseState.AddTransition(StateEnum.Idle, idleState);
            chaseState.AddTransition(StateEnum.Patrol, patrolState);
            chaseState.AddTransition(StateEnum.Pursuit, _pursuitState);
            chaseState.AddTransition(StateEnum.Attack, attackState);

            _fsm = new FSM<StateEnum>(idleState);
        }

        void InitializedTree()
        {
            var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
            var pursuit = new ActionTree(() => _fsm.Transition(StateEnum.Pursuit));
            var attack = new ActionTree(() => _fsm.Transition(StateEnum.Attack));
            var patrol = new ActionTree(() =>
            {
                Debug.Log($"{gameObject.name} entró a Patrol ");
                _fsm.Transition(StateEnum.Patrol);
            });
            var chase = new ActionTree(() =>
            {
                _fsm.Transition(StateEnum.Chase);
                Debug.Log($"{gameObject.name} entró a Chase");
            });
            //var noOperation = new ActionTree(() => { });
            
            var qInIdle = new QuestionTree(() => idleState.IsIdle, idle, patrol);
            var qInPatrol = new QuestionTree(() => patrolState.IsFinishPath, qInIdle, patrol);
            var qInViewNoAttackRange = new QuestionTree(InAggro, pursuit, chase);
            var qAttackRange = new QuestionTree(InAttackRange, attack, qInViewNoAttackRange); //TODO Pathfind
            //var qInView = new QuestionTree(InView, chase, qInPatrol);
            var qInViewNoEvasion = new QuestionTree(InDetectionView, pursuit , qInPatrol);
            var qInViewEvasion = new QuestionTree(InAggro, pursuit , chase); //TODO cambiar de chase a search
            var qIsOnEvasion = new QuestionTree(() => AlertManager.Instance.isOnEvasion, qInViewEvasion, qInViewNoEvasion); //Tirar random entre todos los nodos
            var qIsOnAlert = new QuestionTree(()=> AlertManager.Instance.isOnAlert, qAttackRange, qIsOnEvasion);


            var qIsExist = new QuestionTree( () => player != null, qIsOnAlert, qInPatrol);//qInPatrol);

            _root = qIsExist;
        }

        public bool InView(LineOfSight los)
        {
            bool inView = los.CheckRange(player.transform)
                          && los.CheckAngle(player.transform)
                          && los.CheckView(player.transform);
            if (inView)
            {
                AlertManager.Instance.PlayerFound(player.transform.position);
                return true;
            }
            return false;
        }
        public bool InDetectionView()
        {
            return InView(detectionLoS);
        }

        public void InAggroBuffer()
        {
            //Physics.OverlapSphere()
        }

        public bool InAggro()
        {
            if (InView(aggroLoS) || _pursuitState.IsAggroBufferActive)
            {
                return true;
            }

            return false;
        }
        bool InAttackRange()
        {
            return InView(attackLoS);
        }
        private void Update()
        {
            _fsm.OnUpdate();
            _root.Execute();
        }
        private void LateUpdate()
        {
            _fsm.OnLateUpdate();
        }
    }
}

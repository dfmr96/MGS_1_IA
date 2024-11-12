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
        public LineOfSight detectionLoS;
        public LineOfSight attackLoS;
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


        private void Start()
        {
            player = Constants.Player;
            target = player.GetComponent<Rigidbody>();
            _enemyView = GetComponent<EnemyView>();
            _entityAttack = GetComponent<IAttack>();
            InitializeSteeringBehaviors();
            InitializedFSM();
            InitializedTree();
        }

        private void InitializeSteeringBehaviors()
        {
            var pursuit = new Pursuit(transform, target, timePrediction);
            var evade = new Evade(transform, target, timePrediction);
            _steeringBehaviors.Add(SteeringMode.Pursuit, pursuit);
            _steeringBehaviors.Add(SteeringMode.Evade, evade);
            _steering = _steeringBehaviors[SteeringMode.Pursuit];
        }

        void ChangeSteering(ISteering steering)
        {
            _steering = steering;
        }
    
        void InitializedFSM()
        {
            

            idleState = new EnemyIdleState(_enemyModel, idleTime, idleTimer, _enemyView);
            //idle.onFinishIdle+=
            //idle.isIdle
            var steering = new EnemySteeringState(_enemyModel, _steering, _steeringBehaviors, _evadeCooldown);
            var attack = new EnemyAttackState(_entityAttack, _enemyModel);
            patrolState = new EnemyPatrolState(_enemyModel.transform, _enemyModel, _enemyView, waypoints);
            var chaseState = new EnemyChaseState(_enemyModel.transform, _enemyModel, _enemyView);

            
            idleState.AddTransition(StateEnum.Attack, attack);
            idleState.AddTransition(StateEnum.Steering, steering);
            idleState.AddTransition(StateEnum.Patrol, patrolState);
            idleState.AddTransition(StateEnum.Chase, chaseState);

            steering.AddTransition(StateEnum.Attack, attack);
            steering.AddTransition(StateEnum.Idle, idleState);
            steering.AddTransition(StateEnum.Patrol, patrolState);
            steering.AddTransition(StateEnum.Chase, chaseState);
        
            attack.AddTransition(StateEnum.Steering, steering);
            attack.AddTransition(StateEnum.Patrol, patrolState);
            attack.AddTransition(StateEnum.Idle, idleState);
            attack.AddTransition(StateEnum.Chase, chaseState);

            patrolState.AddTransition(StateEnum.Idle, idleState);
            patrolState.AddTransition(StateEnum.Steering, steering);
            patrolState.AddTransition(StateEnum.Attack, attack);
            patrolState.AddTransition(StateEnum.Chase, chaseState);
            
            chaseState.AddTransition(StateEnum.Idle, idleState);
            chaseState.AddTransition(StateEnum.Patrol, patrolState);
            chaseState.AddTransition(StateEnum.Steering, steering);
            chaseState.AddTransition(StateEnum.Attack, attack);

            _fsm = new FSM<StateEnum>(idleState);
        }

        void InitializedTree()
        {
            var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
            var steering = new ActionTree(() => _fsm.Transition(StateEnum.Steering));
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
            var qInViewNoAttackRange = new QuestionTree(() => patrolState.IsFinishPath, steering, chase);
            var qAttackRange = new QuestionTree(InAttackRange, attack, qInViewNoAttackRange); //TODO Pathfind
            //var qInView = new QuestionTree(InView, chase, qInPatrol);
            var qInViewNoEvasion = new QuestionTree(InView, steering , qInPatrol);
            var qInViewEvasion = new QuestionTree(InView, steering , chase); //TODO cambiar de chase a search
            var qIsOnEvasion = new QuestionTree(() => AlertManager.Instance.isOnEvasion, qInViewEvasion, qInViewNoEvasion); //Tirar random entre todos los nodos
            var qIsOnAlert = new QuestionTree(()=> AlertManager.Instance.isOnAlert, qAttackRange, qIsOnEvasion);


            var qIsExist = new QuestionTree( () => player != null, qIsOnAlert, qInPatrol);//qInPatrol);

            _root = qIsExist;
        }


        bool InRange()
        {
            return detectionLoS.CheckRange(player.transform);
        }
        public bool InView()
        {
            bool inView = detectionLoS.CheckRange(player.transform)
                          && detectionLoS.CheckAngle(player.transform)
                          && detectionLoS.CheckView(player.transform);
            if (inView)
            {
                AlertManager.Instance.PlayerFound(player.transform.position);
                return true;
            }
            return false;
        }
        bool InAttackRange()
        {
            return attackLoS.CheckRange(player.transform);
        }
        private void Update()
        {
            _fsm.OnUpdate();
            _root.Execute();
        
            UpdateSteeringMode();//Remove TODO
        }
        private void LateUpdate()
        {
            _fsm.OnLateUpdate();
        }
    
        private void UpdateSteeringMode()
        {
            if (!InView())
            {
                ChangeSteering(_steeringBehaviors[SteeringMode.Evade]);
            }
            else
            {
                ChangeSteering(_steeringBehaviors[SteeringMode.Pursuit]);
            }
        }
    }
}

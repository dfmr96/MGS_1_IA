using System.Collections.Generic;
using Enemy.EnemyStates;
using JetBrains.Annotations;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyModel _enemyModel;
        public Rigidbody target;
        public LineOfSight los;
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
            _enemyView = GetComponent<EnemyView>();
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
            _entityAttack = GetComponent<IAttack>();

            idleState = new EnemyIdleState(_enemyModel, idleTime, idleTimer, _enemyView);
            //idle.onFinishIdle+=
            //idle.isIdle
            var steering = new EnemySteeringState(_enemyModel, _steering, _steeringBehaviors, _evadeCooldown);
            var attack = new EnemyAttackState(_entityAttack, _enemyModel);
            patrolState = new EnemyPatrolState(_enemyModel.transform, _enemyModel, _enemyView, waypoints);

            
            idleState.AddTransition(StateEnum.Attack, attack);
            idleState.AddTransition(StateEnum.Steering, steering);
            idleState.AddTransition(StateEnum.Patrol, patrolState);

            steering.AddTransition(StateEnum.Attack, attack);
            steering.AddTransition(StateEnum.Idle, idleState);
            steering.AddTransition(StateEnum.Patrol, patrolState);
        
            attack.AddTransition(StateEnum.Steering, steering);
            attack.AddTransition(StateEnum.Idle, idleState);

            patrolState.AddTransition(StateEnum.Idle, idleState);
            patrolState.AddTransition(StateEnum.Steering, steering);
            patrolState.AddTransition(StateEnum.Attack, attack);
            

            _fsm = new FSM<StateEnum>(idleState);
        }

        void InitializedTree()
        {
            var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
            var steering = new ActionTree(() => _fsm.Transition(StateEnum.Steering));
            var attack = new ActionTree(() => _fsm.Transition(StateEnum.Attack));
            var patrol = new ActionTree(() => _fsm.Transition(StateEnum.Patrol));
            
            
            var qInIdle = new QuestionTree(() => idleState.IsIdle, idle, patrol);
            var qInPatrol = new QuestionTree(() => patrolState.IsFinishPath, qInIdle, patrol);
            var qAttackRange = new QuestionTree(InAttackRange, attack, steering);
            //var qInEvadeMode = new QuestionTree(InEvadeMode, steering, qAttackRange);
            var qInView = new QuestionTree(InView, qAttackRange, qInPatrol);
            var qInRange = new QuestionTree(InRange, qInView, qInPatrol);

            var qIsExist = new QuestionTree(() => target != null, qInRange, qInPatrol);

            _root = qIsExist;
        }

        bool InPatrol()
        {
            return !patrolState.IsFinishPath && !idleState.IsIdle;
        }
    
        bool InEvadeMode()
        {
            return _steering == _steeringBehaviors[SteeringMode.Pursuit];
        }

        bool InRange()
        {
            return false; //Vector3.Distance(target.transform.position, transform.position) <= los.range;
        }

        bool HaveToRest()
        {
            return false; //_patrol.RemainingWaypointsToRest <= 0;
        }
        public bool InView()
        {
            return false; //los.CheckAngle(target.transform);
        }
        bool InAttackRange()
        {
            return false; //Vector3.Distance(target.transform.position, transform.position) <= _entityAttack.GetAttackRange;
        }
        private void Update()
        {
            _fsm.OnUpdate();
            _root.Execute();
        
            UpdateSteeringMode();//Remove TODO
        }
        private void FixedUpdate()
        {
            // _fsm.OnFixedUpdate();
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

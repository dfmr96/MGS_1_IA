using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.EnemyStates;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour
{
    public Rigidbody target;
    public LineOfSight los;
    public float timePrediction;
    FSM<StateEnum> _fsm;
    IAttack _entityAttack;
    ITreeNode _root;
    ISteering _steering;
    Dictionary<SteeringMode, ISteering> steeringBehaviors = new Dictionary<SteeringMode, ISteering>();
    IPatrol _patrol;
    Cooldown _evadeCooldown;
    public float idleTime;
    public float idleTimer;
    private void Start()
    {
        InitializeSteeringBehaviors();
        InitializedFSM();
        InitializedTree();

    }

    private void InitializeSteeringBehaviors()
    {
        var pursuit = new Pursuit(transform, target, timePrediction);
        var evade = new Evade(transform, target, timePrediction);
        steeringBehaviors.Add(SteeringMode.Pursuit, pursuit);
        steeringBehaviors.Add(SteeringMode.Evade, evade);
        _steering = steeringBehaviors[SteeringMode.Pursuit];
    }

    void ChangeSteering(ISteering steering)
    {
        _steering = steering;
    }
    
    void InitializedFSM()
    {
        IMove entityMove = GetComponent<IMove>();
        _patrol = GetComponent<IPatrol>();
        _entityAttack = GetComponent<IAttack>();

        var idle = new EnemyIdleState(entityMove, idleTime,ref idleTimer, _patrol);
        //idle.onFinishIdle+=
        //idle.isIdle
        var steering = new EnemySteeringState(entityMove, _steering, steeringBehaviors, _evadeCooldown);
        var attack = new EnemyAttackState(_entityAttack, entityMove);
        var patrol = new EnemyPatrolState(_patrol, entityMove, transform);

        idle.AddTransition(StateEnum.Attack, attack);
        idle.AddTransition(StateEnum.Steering, steering);
        idle.AddTransition(StateEnum.Patrol, patrol);

        steering.AddTransition(StateEnum.Attack, attack);
        steering.AddTransition(StateEnum.Idle, idle);
        steering.AddTransition(StateEnum.Patrol, patrol);
        
        attack.AddTransition(StateEnum.Steering, steering);
        attack.AddTransition(StateEnum.Idle, idle);

        patrol.AddTransition(StateEnum.Idle, idle);
        patrol.AddTransition(StateEnum.Steering, steering);
        patrol.AddTransition(StateEnum.Attack, attack);

        _fsm = new FSM<StateEnum>(patrol);
    }

    void InitializedTree()
    {
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
        var steering = new ActionTree(() => _fsm.Transition(StateEnum.Steering));
        var attack = new ActionTree(() => _fsm.Transition(StateEnum.Attack));
        var patrol = new ActionTree(() => _fsm.Transition(StateEnum.Patrol));

        var qInPatrol = new QuestionTree(HaveToRest, idle, patrol);
        var qAttackRange = new QuestionTree(InAttackRange, attack, steering);
        //var qInEvadeMode = new QuestionTree(InEvadeMode, steering, qAttackRange);
        var qInView = new QuestionTree(InView, qAttackRange, qInPatrol);
        var qInRange = new QuestionTree(InRange, qInView, qInPatrol);

        var qIsExist = new QuestionTree(() => target != null, qInRange, qInPatrol);

        _root = qIsExist;
    }
    
    bool InEvadeMode()
    {
        return _steering == steeringBehaviors[SteeringMode.Pursuit];
    }

    bool InRange()
    {
        return Vector3.Distance(target.transform.position, transform.position) <= los.range;
    }

    bool HaveToRest()
    {
        return _patrol.RemainingWaypointsToRest <= 0;
    }
    public bool InView()
    {
        return los.CheckAngle(target.transform);
    }
    bool InAttackRange()
    {
        return Vector3.Distance(target.transform.position, transform.position) <= _entityAttack.GetAttackRange;
    }
    private void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
        
        UpdateSteeringMode();//Remove TODO
    }
    private void FixedUpdate()
    {
        _fsm.OnFixedUpdate();
    }
    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
    
    private void UpdateSteeringMode()
    {
        if (!InView())
        {
            ChangeSteering(steeringBehaviors[SteeringMode.Evade]);
        }
        else
        {
            ChangeSteering(steeringBehaviors[SteeringMode.Pursuit]);
        }
    }
}

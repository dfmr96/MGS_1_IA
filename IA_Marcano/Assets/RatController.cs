using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] RatModel _ratModel;
    private RatView _ratView;
    [SerializeField] LineOfSight _detectionLoS;
    [SerializeField] private FSM<StateEnum> _fsm;
    private ITreeNode _root;
    [SerializeField] private float fleeThreshold = 5.0f;
    [SerializeField] private List<Node> waypoints;
    [SerializeField] private float idleTime;
    [SerializeField] private RatIdleState _idleState;
    [SerializeField] private RatEvadeState _evadeState;
    [SerializeField] private RatPatrolState _patrolState;
    [SerializeField] private RatFleeState _fleeState;
    [SerializeField] private float _maxDistance;
    private bool _needToFlee;

    private void Start()
    {
        _ratModel = GetComponent<RatModel>();
        _ratView = GetComponent<RatView>();
        _player = Constants.Player.transform;
        InitializeFSM();
        InitializeTree();
    }

    private void InitializeFSM()
    {
        _fsm = new FSM<StateEnum>();
        _idleState = new RatIdleState(_ratModel, idleTime);
        _evadeState = new RatEvadeState(_ratModel, _player.GetComponent<Rigidbody>());

        _fleeState = new RatFleeState(_ratModel.transform, _ratModel, null, _maxDistance);
        _fleeState.onFinishPath += OnFinishPath;
        _patrolState = new RatPatrolState(_ratModel.transform, _ratModel, _ratView, waypoints);

        _idleState.AddTransition(StateEnum.Evade, _evadeState);
        _idleState.AddTransition(StateEnum.Flee, _fleeState);
        _idleState.AddTransition(StateEnum.Patrol, _patrolState);

        _evadeState.AddTransition(StateEnum.Idle, _idleState);
        _evadeState.AddTransition(StateEnum.Flee, _fleeState);
        _evadeState.AddTransition(StateEnum.Patrol, _patrolState);

        _fleeState.AddTransition(StateEnum.Idle, _idleState);
        _fleeState.AddTransition(StateEnum.Evade, _evadeState);
        _fleeState.AddTransition(StateEnum.Patrol, _patrolState);

        _patrolState.AddTransition(StateEnum.Evade, _evadeState);
        _patrolState.AddTransition(StateEnum.Flee, _fleeState);
        _patrolState.AddTransition(StateEnum.Idle, _idleState);

        _fsm.SetInit(_idleState);
    }

    private void InitializeTree()
    {
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
        var evade = new ActionTree(() => _fsm.Transition(StateEnum.Evade));
        var flee = new ActionTree(() => _fsm.Transition(StateEnum.Flee));
        var patrol = new ActionTree(() => _fsm.Transition(StateEnum.Patrol));

        //var qisFinishedPath = new QuestionTree(() => !_fleeState.IsFinishPath, flee, evade); 
        var qShouldFlee = new QuestionTree(ShouldFlee, flee, evade);
        var qInIdle = new QuestionTree(() => _idleState.IsIdle, idle, patrol);
        var qInPatrol = new QuestionTree(() => _patrolState.IsFinishPath, qInIdle, patrol);
        var qInView = new QuestionTree(InView, qShouldFlee, qInPatrol);
        var qIsExist = new QuestionTree(() => _player != null, qInView, qInPatrol);
        _root = qIsExist;
    }

    private bool ShouldFlee()
    {
        _needToFlee = true;
        var distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        return distanceToPlayer >= fleeThreshold || !_fleeState.IsFinishPath;
    }

    void OnFinishPath()
    {
        _needToFlee = false;
    }

    public bool InView()
    {
        bool inView = _detectionLoS.CheckRange(_player)
                      && _detectionLoS.CheckAngle(_player)
                      && _detectionLoS.CheckView(_player);
        if (inView || _needToFlee)
        {
            return true;
        }

        return false;
    }

    private void Update()
    {
        _root.Execute();
        _fsm.OnUpdate();
    }

    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
}
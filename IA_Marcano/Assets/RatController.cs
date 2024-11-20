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
    [SerializeField] private List<Node> waypoints;
    [SerializeField] private float idleTime;
    [SerializeField] private RatIdleState _idleState;
    [SerializeField] private RatEvadeState _evadeState;
    [SerializeField] private RatPatrolState _patrolState;

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
        var flee = new RatFleeState();
        _patrolState = new RatPatrolState(_ratModel.transform,_ratModel, _ratView, waypoints);

        _idleState.AddTransition(StateEnum.Evade, _evadeState);
        _idleState.AddTransition(StateEnum.Flee, flee);
        _idleState.AddTransition(StateEnum.Patrol,_patrolState);
        
        _evadeState.AddTransition(StateEnum.Idle, _idleState);
        _evadeState.AddTransition(StateEnum.Flee, flee);
        _evadeState.AddTransition(StateEnum.Patrol,_patrolState);
        
        flee.AddTransition(StateEnum.Idle,_idleState);
        flee.AddTransition(StateEnum.Evade, _evadeState);
        flee.AddTransition(StateEnum.Patrol,_patrolState);
        
        _patrolState.AddTransition(StateEnum.Evade, _evadeState);
        _patrolState.AddTransition(StateEnum.Flee, flee);
        _patrolState.AddTransition(StateEnum.Idle, _idleState);
        
        _fsm.SetInit(_idleState);
    }

    private void InitializeTree()
    {
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
        var evade = new ActionTree(() => _fsm.Transition(StateEnum.Evade));
        var flee = new ActionTree(() => _fsm.Transition(StateEnum.Flee));
        var patrol = new ActionTree(() => _fsm.Transition(StateEnum.Patrol));
        
            
        var qInIdle = new QuestionTree(() => _idleState.IsIdle, idle, patrol);
        var qInPatrol = new QuestionTree(() => _patrolState.IsFinishPath, qInIdle, patrol);
        var qInView = new QuestionTree(InView, evade , qInPatrol);
        var qIsExist = new QuestionTree( () => _player != null, qInView, qInPatrol);//qInPatrol);
        Debug.Log($"{_player}");
        _root = qIsExist;
    }
    
    public bool InView()
    {
        
        bool inView = _detectionLoS.CheckRange(_player)
                      && _detectionLoS.CheckAngle(_player)
                      && _detectionLoS.CheckView(_player);
        if (inView || _evadeState.IsAggroBufferActive)
        {
            Debug.Log("In view");
            return true;
        }
        Debug.Log("Not in view");
        return false;
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
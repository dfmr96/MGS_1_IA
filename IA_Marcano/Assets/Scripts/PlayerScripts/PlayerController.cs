using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScripts.PlayerStates;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    IMove _move;
    FSM<StateEnum> _fsm;
    private PlayerModel _playerModel;
    public PlayerModel playerModel => _playerModel;
    public float Health => _playerModel.Health;
    private PlayerView _playerView;
    [SerializeField] private PlayerStateAiming<StateEnum> _aim;

    [SerializeField] private float _detectionRadius = 10f; 
    [SerializeField] private LayerMask _enemyLayer; 
    [SerializeField] private LayerMask _obstructionLayer;
    [SerializeField] private Transform _playerTransform;

    [SerializeField] private float damage;
    
    private void Awake()
    {
        Constants.SetPlayer(this);
    }

    void Start()
    {
        _move = GetComponent<IMove>();
        _playerModel = GetComponent<PlayerModel>();
        _playerView = GetComponent<PlayerView>();
        InitializedFSM();
    }
    void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var idle = new PlayerStateIdle<StateEnum>(_fsm,StateEnum.Move, _move, StateEnum.Aim, StateEnum.Dead, playerModel);
        var move = new PlayerStateMove<StateEnum>(_fsm, _move, StateEnum.Idle, StateEnum.Aim, StateEnum.Dead, playerModel);
        _aim = new PlayerStateAiming<StateEnum>(_fsm, StateEnum.Move,StateEnum.Idle, _playerView, _detectionRadius, _enemyLayer, _obstructionLayer, damage, StateEnum.Dead, playerModel);
        var dead = new PlayerStateDead();

        idle.AddTransition(StateEnum.Move, move);
        idle.AddTransition(StateEnum.Dead, dead);
        idle.AddTransition(StateEnum.Aim, _aim);

        move.AddTransition(StateEnum.Idle, idle);
        move.AddTransition(StateEnum.Dead, dead);
        move.AddTransition(StateEnum.Aim, _aim);
        
        _aim.AddTransition(StateEnum.Move, move);
        _aim.AddTransition(StateEnum.Dead, dead);
        _aim.AddTransition(StateEnum.Idle, idle);

        _fsm.SetInit(idle);
    }
    void Update()
    {
        _fsm.OnUpdate();
    }
    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
}
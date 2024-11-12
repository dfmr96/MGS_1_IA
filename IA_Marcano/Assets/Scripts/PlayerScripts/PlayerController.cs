using System;
using System.Collections;
using System.Collections.Generic;
using PlayerScripts.PlayerStates;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    IMove _move;
    FSM<StateEnum> _fsm;
    private PlayerModel _playerModel;
    private ITreeNode _root;

    private void Awake()
    {
        Constants.SetPlayer(this);
    }

    void Start()
    {
        _move = GetComponent<IMove>();
        
        InitializedFSM();
        InitializeTree();
        _playerModel = GetComponent<PlayerModel>();
    }
    void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var idle = new PlayerStateIdle(_fsm, _move);
        var move = new PlayerStateMove(_fsm, _move);
        var dead = new PlayerStateDead();

        idle.AddTransition(StateEnum.Move, move);
        idle.AddTransition(StateEnum.Dead, dead);

        move.AddTransition(StateEnum.Idle, idle);
        move.AddTransition(StateEnum.Dead, dead);

        _fsm.SetInit(idle);
    }

    void InitializeTree()
    {
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
        var move = new ActionTree(() => _fsm.Transition(StateEnum.Move));
        var dead = new ActionTree(() => _fsm.Transition(StateEnum.Dead));
        
        var qIsIdle = new QuestionTree(isIdle, idle, move);
        var qIsMoving = new QuestionTree(isMoving, move, qIsIdle);
        var qIsDead = new QuestionTree(() => _playerModel.IsDead, dead, qIsMoving);

        _root = qIsDead;
    }

    bool isIdle()
    {
        var h = Input.GetAxis("Horizontal"); //TODO INPUT MANAGER
        var v = Input.GetAxis("Vertical"); 
        if (h == 0 && v == 0) //TODO INPUT MANAGER
        {
            return true;
        }

        return false;
    }
    bool isMoving()
    {
        var h = Input.GetAxis("Horizontal"); //TODO INPUT MANAGER
        var v = Input.GetAxis("Vertical"); //TODO INPUT MANAGER
        if (h != 0 || v != 0) //TODO INPUT MANAGER
        {
            return true;
        }
        return false;
    }
    void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }
    private void FixedUpdate()
    {
        //_fsm.OnFixedUpdate();
    }
    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle<T> : State<T>
{
    private T _inputToMove;
    private T _inputToAim;
    IMove _move;
    private readonly T _inputToDeath;
    private readonly PlayerModel _playerModel;

    public PlayerStateIdle(FSM<T> fsm, T inputToMove, IMove move, T inputToAim, T inputToDeath, PlayerModel playerModel)
    {
        _fsm = fsm;
        _inputToMove = inputToMove;
        _inputToAim = inputToAim;
        _inputToDeath = inputToDeath;
        _playerModel = playerModel;
        _move = move;
        _playerModel.OnDead += ToDeath;
    }

    public override void Enter()
    {
        base.Enter();
        //_move.Move(Vector3.zero);
    }

    public override void Execute()
    {
        base.Execute();
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        Debug.Log($"{h},{v}");
        if (h != 0 || v != 0)
        {
            _fsm.Transition(_inputToMove);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Intenta entrar a Aim");
            _fsm.Transition(_inputToAim);
        }
    }

    public void ToDeath()
    {
        _fsm.Transition(_inputToDeath);
    }
}

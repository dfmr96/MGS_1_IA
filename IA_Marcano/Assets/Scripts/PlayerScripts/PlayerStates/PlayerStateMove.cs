using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMove<T>: State<T>
{
    IMove _move;
    private T _inputToIdle;
    private T _inputToAim;
    private Vector3 lastDir;
    private readonly T _inputToDeath;
    private readonly PlayerModel _playerModel;

    public PlayerStateMove(FSM<T> fsm, IMove move, T inputToIdle, T inputToAim, T inputToDeath, PlayerModel playerModel)
    {
        _move = move;
        _fsm = fsm;
        _inputToIdle = inputToIdle;
        _inputToAim = inputToAim;
        _inputToDeath = inputToDeath;
        _playerModel = playerModel;
        _playerModel.OnDead += ToDeath;
    }

    public void ToDeath()
    {
        _fsm.Transition(_inputToDeath);
    }

    public override void Execute()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(-h, 0, -v); 
        _move.Move(dir.normalized);

        if (h == 0 && v == 0)
        {
            _fsm.Transition(_inputToIdle);
        }
        if (h != 0 || v != 0) _move.LookDir(dir);
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Intenta entrar a Aim");
            _fsm.Transition(_inputToAim);
        }
        
    }
}

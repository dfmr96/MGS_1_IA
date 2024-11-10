using System;
using System.Collections;
using System.Collections.Generic;
using Alert.AlertStates;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance { get; private set; }

    private FSM<StateEnum> _fsm;
    private ITreeNode _root;

    [SerializeField] private float _redAlertTime;
    [SerializeField] private Color _alertColor;
    [SerializeField] private float _yellowAlertTime;
    [SerializeField] private AlertState _alertState;
    [SerializeField] private GameObject _alertUI;
    [SerializeField] private TMP_Text _countdown;
    [SerializeField] private GameObject _stateBackground;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializedFSM();
        InitializeTree();
    }

    private void InitializedFSM()
    {
        var idleState = new IdleState();
        _alertState = new AlertState(_redAlertTime, _countdown, _stateBackground, _alertUI, _alertColor);

        idleState.AddTransition(StateEnum.Alert, _alertState);
        
        _alertState.AddTransition(StateEnum.Idle,idleState);

        _fsm = new FSM<StateEnum>(idleState);
    }

    private void InitializeTree()
    {
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
        var alert = new ActionTree(() => _fsm.Transition(StateEnum.Alert));

        var qInAlert = new QuestionTree(() => _alertState.IsAlertFinished, idle, alert);

        _root = qInAlert;
    }

    private void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }

    [ContextMenu("Call Alert")]
    public void CallAlert()
    {
        _alertState.CallAlert();
    }
}

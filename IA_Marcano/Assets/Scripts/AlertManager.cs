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

    [SerializeField] private float _alertTime;
    [SerializeField] private Color _alertColor;
    [SerializeField] private float _evasionTime;
    [SerializeField] private Color _evasionColor;
    [SerializeField] private AlertManagerState _alertState;
    [SerializeField] private AlertManagerState _evasionState;
    [SerializeField] private GameObject _alertUI;
    [SerializeField] private GameObject _evasionUI;
    [SerializeField] private TMP_Text _countdown;
    [SerializeField] private GameObject _stateBackground;

    public bool isOnAlert => !_alertState.IsStateFinished;
    public bool isOnEvasion => !_evasionState.IsStateFinished && !isOnAlert;

    private Vector3 _playerLastPosition;
    public Vector3 PlayerLastPosition => _playerLastPosition;
    public Action OnLastPlayerPositionChanged;
    [SerializeField] private float distanceThreshold;

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
        _playerLastPosition = Vector3.positiveInfinity;
        InitializedFSM();
        InitializeTree();
    }

    private void InitializedFSM()
    {
        var idleState = new IdleState();
        _alertState = new AlertState("Alert",_alertTime, _countdown, _stateBackground, _alertUI, _alertColor);
        _evasionState = new EvasionState("Evasion",_evasionTime, _countdown, _stateBackground, _evasionUI, _evasionColor);

        idleState.AddTransition(StateEnum.Alert, _alertState);
        idleState.AddTransition(StateEnum.Evasion, _evasionState);
        
        _alertState.AddTransition(StateEnum.Evasion, _evasionState);
        //_alertState.AddTransition(StateEnum.Idle, idleState);
        
        _evasionState.AddTransition(StateEnum.Alert, _alertState);
        _evasionState.AddTransition(StateEnum.Idle, idleState);
        

        _fsm = new FSM<StateEnum>(idleState);
    }

    private void InitializeTree()
    {
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
        var evasion = new ActionTree(() => _fsm.Transition(StateEnum.Evasion));
        var alert = new ActionTree(() => _fsm.Transition(StateEnum.Alert));

        var qInEvasion = new QuestionTree(() => !_evasionState.IsStateFinished, evasion,idle);
        var qInAlert = new QuestionTree(() => !_alertState.IsStateFinished, alert,qInEvasion);
        //var qInIdle = new QuestionTree(IsIdle, idle, qInAlert);

        _root = qInAlert;
    }

    public bool IsIdle()
    {
        if (_evasionState.IsStateFinished && _alertState.IsStateFinished)
        {
            return true;
        }

        return false;
    }

    public bool IsAlert()
    {
        if (_alertState.IsStateFinished) return true;
        return false;
    }

    private void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }

    [ContextMenu("Call Alert")]
    public void CallAlert()
    {
        _alertState.CallState();
        _evasionState.CallState();
    }
    [ContextMenu("Call Evasion")]
    public void CallEvasion()
    {
        _evasionState.CallState();
    }
    
    [ContextMenu("End Alert")]
    public void EndAlert()
    {
        _alertState.EndState();
        CallEvasion();
    }
    
    [ContextMenu("End Evasion")]
    public void EndEvasion()
    {
        TryUpdatePlayerLastPosition();
        _evasionState.EndState();
    }

    public void TryUpdatePlayerLastPosition()
    {
        Vector3 playerCurrent = Constants.Player.transform.position;
        float positionDistance = Vector3.Distance(_playerLastPosition, playerCurrent);
        if (positionDistance > distanceThreshold)
        {
            _playerLastPosition = playerCurrent;
            OnLastPlayerPositionChanged?.Invoke();
        }
    }
    
    [ContextMenu("PlayerFound")]
    public void PlayerFound()
    {
        TryUpdatePlayerLastPosition();
        CallAlert();
    }
}

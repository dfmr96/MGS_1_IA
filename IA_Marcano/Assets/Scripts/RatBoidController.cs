using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class RatBoidController : MonoBehaviour
    { 
        [SerializeField] private Transform _player;
        [SerializeField] RatModel _ratModel;
        private RatView _ratView;
        [SerializeField] LineOfSight _detectionLoS;
        [SerializeField] private FSM<StateEnum> _fsm;

    private void Start()
    {
        _ratModel = GetComponent<RatModel>();
        _ratView = GetComponent<RatView>();
        _player = Constants.Player.transform;
        InitializeFSM();
    }
    
    private void InitializeFSM()
    {
        _fsm = new FSM<StateEnum>();
        var flockingState = new RatBoidSteeringState<StateEnum>(_ratModel, GetComponent<FlockingManager>(), _ratModel.Obs);
        _fsm.SetInit(flockingState);
    }
    private void Update()
    {
        _fsm.OnUpdate();
    }
    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
    }
}
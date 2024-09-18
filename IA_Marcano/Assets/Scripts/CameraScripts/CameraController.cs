using CameraScripts.CameraStates;
using UnityEngine;

namespace CameraScripts
{
    public class CameraController : MonoBehaviour
    {
        public Transform target;
        LineOfSight _los;
        IAlert _alert;
        ISpin _spin; ////TODO SPIN
        private FSM<StateEnum> _fsm;
        private void Awake()
        {
            _los = GetComponent<LineOfSight>();
            _alert = GetComponent<IAlert>();
            _spin = target.GetComponent<ISpin>();
        }

        private void Start()
        {
            InitializeFSM();
        }

        private void InitializeFSM()
        {
            _fsm = new FSM<StateEnum>();
            var idle = new CameraStateIdle(_fsm, _los, target, _spin); //TODO SPIN
            var alert = new CameraStateAlert(_fsm, _los, target, _spin, _alert); //TODO SPIN

            idle.AddTransition(StateEnum.Alert, alert);
            alert.AddTransition(StateEnum.Idle, idle);
        
            _fsm.SetInitial(idle);
        }
        // Update is called once per frame
        private void Update()
        {
            _fsm.OnUpdate();
        }

        private void LateUpdate()
        {
            _fsm.OnLateUpdate();
        }

        private void FixedUpdate()
        {
            _fsm.OnFixedUpdate();
        }
    }
}

using System;
using CameraScripts.CameraStates;
using UnityEngine;

namespace CameraScripts
{
    public class CameraController : MonoBehaviour
    {
        public Transform target;
        LineOfSight _los;
        IAlert _alert;
        private FSM<StateEnum> _fsm;
        private ITreeNode _root;
        private void Awake()
        {
            _los = GetComponent<LineOfSight>();
            _alert = GetComponent<IAlert>();
        }

        private void Start()
        {
            InitializeFSM();
            InitializeTree();
        }

        private void InitializeFSM()
        {
            _fsm = new FSM<StateEnum>();
            var idle = new CameraStateIdle();
            var alert = new CameraStateAlert(_alert);

            idle.AddTransition(StateEnum.Alert, alert);
            alert.AddTransition(StateEnum.Idle, idle);
        
            //_fsm.SetInitial(idle);
        }

        private void InitializeTree()
        {
            var alert = new ActionTree(() => _fsm.Transition(StateEnum.Alert));
            var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));

            var qInView = new QuestionTree(InView, alert, idle);
            var qIsExist = new QuestionTree(() => target != null, qInView, idle); 
            _root = qIsExist;
        }

        public bool InView()
        {
            return _los.CheckRange(target) &&
                   _los.CheckAngle(target) &&
                   _los.CheckView(target);
        }
        // Update is called once per frame
        private void Update()
        {
            _fsm.OnUpdate();
            _root.Execute();
        }

        private void LateUpdate()
        {
            _fsm.OnLateUpdate();
        }

        private void FixedUpdate()
        {
            //_fsm.OnFixedUpdate();
        }
    }
}

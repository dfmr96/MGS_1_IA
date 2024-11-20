using System;
using CameraScripts.CameraStates;
using UnityEngine;

namespace CameraScripts
{
    public class CameraController : MonoBehaviour
    {
        private Transform _target;
        [SerializeField] LineOfSight _detectionLoS;
        IAlert _alert;
        private FSM<StateEnum> _fsm;
        private ITreeNode _root;
        private CameraModel _cameraModel;
        private void Awake()
        {
            _alert = GetComponent<IAlert>();
            _cameraModel = GetComponent<CameraModel>();
        }

        private void Start()
        {
            _target = Constants.Player.transform;
            InitializeFSM();
            InitializeTree();
        }

        private void InitializeFSM()
        {
            _fsm = new FSM<StateEnum>();
            var idle = new CameraStateIdle(_cameraModel);
            var alert = new CameraStateAlert(_cameraModel, _target);

            idle.AddTransition(StateEnum.Alert, alert);
            alert.AddTransition(StateEnum.Idle, idle);
        
            _fsm.SetInit(idle);
        }

        private void InitializeTree()
        {
            var alert = new ActionTree(() => _fsm.Transition(StateEnum.Alert));
            var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));

            var qInView = new QuestionTree(InView, alert, idle);
            var qIsExist = new QuestionTree(() => _target != null, qInView, idle); 
            _root = qIsExist;
        }
        
        public bool InView()
        {
            bool inView = _detectionLoS.CheckRange(_target)
                          && _detectionLoS.CheckAngle(_target)
                          && _detectionLoS.CheckView(_target);
            if (inView)
            {
                AlertManager.Instance.PlayerFound();
                return true;
            }
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

        private void FixedUpdate()
        {
            //_fsm.OnFixedUpdate();
        }
    }
}

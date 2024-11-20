using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class RatBoidController : MonoBehaviour
    { 
        [SerializeField] private Transform _player;
        [SerializeField] RatModel _ratModel;
        [SerializeField] private RatController ratLeader;
        private RatView _ratView;
        [SerializeField] LineOfSight _leaderDetectionLoS;
        [SerializeField] private FSM<StateEnum> _fsm;
        
        
        private ITreeNode _root;


    private void Start()
    {
        _ratModel = GetComponent<RatModel>();
        _ratView = GetComponent<RatView>();
        _player = Constants.Player.transform;
        InitializeFSM();
        InitDecisionTree();
    }
    
    private void InitializeFSM()
    {
        _fsm = new FSM<StateEnum>();

        var chaseLeaderState = new RatBoidChaseState(ratLeader.transform, transform, _ratModel, null, 0.5f);
        var flockingState = new RatBoidSteeringState<StateEnum>(_ratModel, GetComponent<FlockingManager>(), _ratModel.Obs);
        
        chaseLeaderState.AddTransition(StateEnum.Flock, flockingState);
        flockingState.AddTransition(StateEnum.Chase, chaseLeaderState);
        
        _fsm.SetInit(flockingState);
    }

    private void InitDecisionTree()
    {
        var flock = new ActionNode(() => _fsm.Transition(StateEnum.Flock));
        var chase = new ActionNode(() => _fsm.Transition(StateEnum.Chase));

        var qLeaderInView = new QuestionTree(() => _ratModel.InView(_leaderDetectionLoS, ratLeader.transform), flock, chase);

        _root = qLeaderInView;
        Debug.Log($"{_root}");
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
    }
}
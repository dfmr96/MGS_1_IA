using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CrashController : MonoBehaviour
{
    CrashModel _model;
    Animator _anim;
    FSM<StatesEnum> _fsm;
    ITreeNode _root;
    StatePathfinding<StatesEnum> _statePathfinding;
    public Node start;
    public Node goal;
    public Transform target;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _model = GetComponent<CrashModel>();
    }
    private void Start()
    {
        InitializeFSM();
        InitializeTree();
        Test();
    }
    void Test()
    {
        var lut = new LookUpTable<GameObject, CrashModel>(Method);

        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(transform.gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(start.gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
        lut.Run(gameObject);
    }
    CrashModel Method(GameObject obj)
    {
        print("SE HIZO");
        return obj.GetComponent<CrashModel>();
    }
    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();
        var idle = new CrashStateIdle<StatesEnum>(_anim);
        _statePathfinding = new StatePathfinding<StatesEnum>(_model.transform, _model, _anim);

        idle.AddTransition(StatesEnum.Waypoints, _statePathfinding);
        _statePathfinding.AddTransition(StatesEnum.Idle, idle);
        _fsm.SetInit(idle);
    }
    void InitializeTree()
    {
        var idle = new ActionNode(() => _fsm.Transition(StatesEnum.Idle));
        var follow = new ActionNode(() => _fsm.Transition(StatesEnum.Waypoints));

        var qFollowPoints = new QuestionNode(() => _statePathfinding.IsFinishPath, idle, follow);
        _root = qFollowPoints;
    }
    private void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }
    public void RePath()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.SetPath();
    }
    public void RePathDFS()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.SetPathDFS();
    }
    public void RePathDijkstra()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.SetPathDijkstra();
    }

    public void RePathAstar()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.target = target;
        _statePathfinding.SetPathAStar();
    }
    public void RePathAstarPlus()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.target = target;
        _statePathfinding.SetPathAStarPlus();
    }
    public void RePathAstarPlusVector()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.target = target;
        _statePathfinding.SetPathAStarPlusVector();
    }
    public void RePathThetaStar()
    {
        _statePathfinding.start = start;
        _statePathfinding.goal = goal;
        _statePathfinding.target = target;
        _statePathfinding.SetPathThetaStar();
    }
}

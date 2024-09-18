using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody target;
    public LineOfSight los;
    public float timePrediction;
    FSM<StateEnum> _fsm;
    IAttack _entityAttack;
    ITreeNode _root;
    ISteering _steering;
    private void Start()
    {
        InitializedSteering();
        InitializedFSM();
        InitializedTree();
    }
    void InitializedSteering()
    {
        var seek = new Seek(transform, target.transform);
        var flee = new Flee(transform, target.transform);
        var pursuit = new Pursuit(transform, target, timePrediction);
        var evade = new Evade(transform, target, timePrediction);
        _steering = seek;
    }
    public void ChangeSteering(ISteering steering)
    {
        _steering = steering;
    }
    void InitializedFSM()
    {
        IMove entityMove = GetComponent<IMove>();
        _entityAttack = GetComponent<IAttack>();

        var idle = new EnemyIdleState();
        var chase = new EnemySteeringState(entityMove, _steering);
        var attack = new EnemyAttackState(_entityAttack);

        idle.AddTransition(StateEnum.Attack, attack);
        idle.AddTransition(StateEnum.Chase, chase);

        chase.AddTransition(StateEnum.Attack, attack);
        chase.AddTransition(StateEnum.Idle, idle);

        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.Idle, idle);

        _fsm = new FSM<StateEnum>(idle);
    }

    void InitializedTree()
    {
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
        var chase = new ActionTree(() => _fsm.Transition(StateEnum.Chase));
        var attack = new ActionTree(() => _fsm.Transition(StateEnum.Attack));

        var qDistance = new QuestionTree(InAttackRange, attack, chase);
        var qInView = new QuestionTree(InView, qDistance, idle);
        var qIsExist = new QuestionTree(() => target != null, qInView, idle);

        _root = qIsExist;
    }
    bool InView()
    {
        return true;
    }
    bool InAttackRange()
    {
        return Vector3.Distance(target.transform.position, transform.position) <= _entityAttack.GetAttackRange;
    }
    private void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }
    private void FixedUpdate()
    {
        _fsm.OnFixedUpdate();
    }
    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
}

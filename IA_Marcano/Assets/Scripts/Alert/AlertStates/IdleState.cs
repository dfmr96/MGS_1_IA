using UnityEngine;

namespace Alert.AlertStates
{
    public class IdleState: State<StateEnum>
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Alert Manager en Idle");
        }
    }
}
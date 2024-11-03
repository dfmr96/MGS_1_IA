using UnityEngine;

namespace CameraScripts.CameraStates
{
    public class CameraStateIdle : State<StateEnum>
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("IDLE ENTER");
        }
    }
}

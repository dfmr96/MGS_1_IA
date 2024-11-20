using UnityEngine;

namespace CameraScripts.CameraStates
{
    public class CameraStateIdle : State<StateEnum>
    {
        private GameObject _body;
        private CameraModel _camModel;
        public CameraStateIdle(CameraModel camModel)
        {
            _camModel = camModel;
        }

        public override void Execute()
        {
            base.Execute();
            _camModel.RotateBase();
        }
    }
}

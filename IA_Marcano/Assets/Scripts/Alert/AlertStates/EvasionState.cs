using TMPro;
using UnityEngine;

namespace Alert.AlertStates
{
    public class EvasionState : AlertManagerState
    {
        public EvasionState(string stateName, float stateTime, TMP_Text countdown, GameObject stateBackground, GameObject stateGif, Color stateColor) : base(stateName, stateTime, countdown, stateBackground, stateGif, stateColor)
        {
        }

        public override void Sleep()
        {
            base.Sleep();
            AudioManager.Instance.EvasionFinished();
        }
    }
}
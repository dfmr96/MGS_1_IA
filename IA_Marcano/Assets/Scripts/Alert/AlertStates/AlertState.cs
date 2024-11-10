using TMPro;
using UnityEngine;

namespace Alert.AlertStates
{
    public class AlertState : AlertManagerState
    {
        public AlertState(string stateName, float stateTime, TMP_Text countdown, GameObject stateBackground, GameObject stateGif, Color stateColor) : base(stateName, stateTime, countdown, stateBackground, stateGif, stateColor)
        {
        }

        public override void Enter()
        {
            base.Enter();
            AudioManager.Instance.PlayAlertBGM();
        }

        public void PlayMusic()
        {
            AudioManager.Instance.PlayAlertBGM();
        }
    }
}
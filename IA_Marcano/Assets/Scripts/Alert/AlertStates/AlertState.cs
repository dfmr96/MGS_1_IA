using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alert.AlertStates
{
    [Serializable]
    public class AlertState : State<StateEnum>
    {
        [SerializeField] float _alertTimer;
        [SerializeField] private float _alertTime;
        [SerializeField] private Color _alertColor;
        
        private bool _isAlertFinished;
        private TMP_Text _countdown;
        private GameObject _alertGif;
        private GameObject _stateBackground;
        private Image _stateBackgroundImage;

        public bool IsAlertFinished => _isAlertFinished;

        public AlertState(float alertTime, TMP_Text countdown, GameObject stateBackground, GameObject alertGif, Color alertColor)
        {
            _alertTime = alertTime;
            _stateBackground = stateBackground;
            _alertGif = alertGif;
            _alertColor = alertColor;
            _stateBackgroundImage = _stateBackground.GetComponent<Image>();
            _countdown = countdown;
            _isAlertFinished = true;
        }

        public override void Enter()
        {
            base.Enter();
            _alertTimer = _alertTime;
            _stateBackgroundImage.color = _alertColor;
            _alertGif.SetActive(true);
            _countdown.gameObject.SetActive(true);
            _stateBackground.SetActive(true);
            _isAlertFinished = false;
        }

        public override void Execute()
        {
            base.Execute();

            if (_isAlertFinished) return;
            _alertTimer -= Time.deltaTime;
            _countdown.SetText(_alertTimer.ToString("00.00"));

            if (_alertTimer < 0)
            {
                _alertTimer = 0;
                _isAlertFinished = true;
            }
        }

        public override void Sleep()
        {
            base.Sleep();
            _alertGif.SetActive(false);
            _stateBackground.SetActive(false);
            _countdown.gameObject.SetActive(false);
        }


        public void CallAlert()
        {
            _isAlertFinished = false;
        }
    }
}
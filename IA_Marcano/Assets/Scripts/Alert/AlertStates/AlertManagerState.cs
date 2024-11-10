using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Alert.AlertStates
{
    [Serializable]
    public class AlertManagerState : State<StateEnum>
    {
        [SerializeField] private string _stateName = "";
        [SerializeField] float _stateTimer;
        [SerializeField] private float _stateTime;
        [SerializeField] private Color _stateColor;
        
        [SerializeField] private bool _isStateFinished;
        private TMP_Text _countdown;
        private GameObject _stateGif;
        private GameObject _stateBackground;
        private Image _stateBackgroundImage;

        public bool IsStateFinished => _isStateFinished;

        public AlertManagerState(string stateName,float stateTime, TMP_Text countdown, GameObject stateBackground, GameObject stateGif, Color stateColor)
        {
            _stateName = stateName;
            _stateTime = stateTime;
            _stateBackground = stateBackground;
            _stateGif = stateGif;
            _stateColor = stateColor;
            _stateBackgroundImage = _stateBackground.GetComponent<Image>();
            _countdown = countdown;
            _isStateFinished = true;
        }

        public override void Enter()
        {
            base.Enter();
            _stateTimer = _stateTime;
            _stateBackgroundImage.color = _stateColor;
            _stateGif.SetActive(true);
            _countdown.gameObject.SetActive(true);
            _stateBackground.SetActive(true);
            _isStateFinished = false;
            Debug.Log($"Entró en {_stateName}");
        }

        public override void Execute()
        {
            base.Execute();

            if (_isStateFinished) return;
            _stateTimer -= Time.deltaTime;
            _countdown.SetText(_stateTimer.ToString("00.00"));

            if (_stateTimer < 0)
            {
                _stateTimer = 0;
                _isStateFinished = true;
            }
        }

        public override void Sleep()
        {
            base.Sleep();
            _stateGif.SetActive(false);
            _stateBackground.SetActive(false);
            _countdown.gameObject.SetActive(false);
            Debug.Log($"Salió de {_stateName}");
        }


        public void CallState()
        {
            _isStateFinished = false;
            _stateTimer = _stateTime;
        }

        public void EndState()
        {
            _stateTimer = 0;
        }
        
    }
}
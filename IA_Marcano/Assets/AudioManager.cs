using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource _audioSourceMain;
    [SerializeField] private AudioSource _audioSourceBackup;
    [SerializeField] private AudioClip _level1BGM;
    [SerializeField] private AudioClip _alertBGM;
    [SerializeField] private AudioClip _evasionFinishedSFX;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
        
        
        _audioSourceBackup.clip = _alertBGM;
    }

    private void Start()
    {
        PlayBGM(_level1BGM);
    }

    [ContextMenu("PlayAlertBGM")]
    public void PlayAlertBGM()
    {
        _audioSourceMain.Pause();
        _audioSourceBackup.Play();
    }

    public void PlayAudioOneShot(AudioClip audioClip)
    {
        _audioSourceMain.PlayOneShot(audioClip);
    }

    public void PlayBGM(AudioClip audioClip)
    {
        _audioSourceMain.clip = audioClip;
        _audioSourceMain.Play();
    }

    public void EvasionFinished()
    {
        _audioSourceBackup.Stop();
        _audioSourceBackup.PlayOneShot(_evasionFinishedSFX);
        _audioSourceMain.UnPause();
    }
}

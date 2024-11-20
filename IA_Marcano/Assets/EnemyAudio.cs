using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Randoms;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyAudio : MonoBehaviour
{
    public AudioSource _audioSource;
    public List<AudioClip> _deathAudioClips;
    public Dictionary<AudioClip, float> _audios = new Dictionary<AudioClip, float>();
    private void Start()
    {
        if (_deathAudioClips.Count <= 0) return;
        foreach (var audioClip in _deathAudioClips)
        {
            _audios.Add(audioClip, 1);
        }
    }
    
    public void PlayRandomDeathAudio()
    {
        AudioClip selectedClip = MyRandoms.Roulette(_audios);
        
        if (selectedClip != null)
        {
            _audioSource.clip = selectedClip;
            _audioSource.Play();
        }
    }
}

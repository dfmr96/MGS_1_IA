using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Animator gameOverAnimator;
    public GameObject codec;
    public AudioSource codecSource;
    public AudioSource audioSource;

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
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Victory()
    {
        codec.SetActive(true);
        codecSource.Play();
    }

    public void GameOver()
    {
        audioSource.PlayDelayed(0.5f);
        gameOverAnimator.SetTrigger("Fade");
    }
}

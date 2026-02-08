using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioPlayerManager: MonoBehaviour
{
    private static AudioPlayerManager instance = null;
    public AudioClip Scorpion;
    public AudioClip Segaaaaa;
    public AudioClip SteveDamage;
    public AudioClip AntoineDaniel;
    public AudioClip GameOver;
    public AudioClip overworldMusic;
    private AudioSource audio;
    public AudioClip Squalala;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        if (instance == this) return; 
        Destroy(gameObject);
    }

    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = Segaaaaa;
        audio.Play();
    }

    public void PlayScorpion()
    {
        audio.Stop();
        audio.clip = Scorpion;
        audio.Play();
    }

    public void PlaySegaaaaa()
    {
        audio.Stop();
        audio.clip = Segaaaaa;
        audio.Play();
    }

    public void PlaySteveDamage()
    {
        audio.Stop();
        audio.clip = SteveDamage;
        audio.Play();
    }

    public void PlayAntoineDaniel()
    {
        audio.Stop();
        audio.clip = AntoineDaniel;
        audio.Play();
    }

    public void PlayGameOver()
    {
        audio.Stop();
        audio.clip = GameOver;
        audio.Play();
    }

    public void PlayOverworldMusic()
    {
        audio.Stop();
        audio.clip = overworldMusic;
        audio.Play();
    }

    public void PlaySqualala()
    {
        audio.Stop();
        audio.clip = Squalala;
        audio.Play();
    }

    public void Update()
    {
        if (!audio.isPlaying)
        {
            PlayOverworldMusic();
        }
    }
}
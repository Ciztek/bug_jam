using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioPlayerManager : MonoBehaviour
{
    private static AudioPlayerManager instance = null;

    // SFX
    public AudioClip Scorpion;
    public AudioClip Segaaaaa;
    public AudioClip SteveDamage;
    public AudioClip AntoineDaniel;
    public AudioClip GameOver;
    public AudioClip Squalala;

    // Overworld music
    public AudioClip overworldMusic;
    public AudioClip Visitor;

    // AudioSources
    private AudioSource musicMain;      // main music layer
    private AudioSource musicSecondary; // secondary music layer for overlap
    private AudioSource sfxSource;      // plays sound effects

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
        // Initialize AudioSources
        musicMain = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        musicSecondary = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        // Loop music
        musicMain.loop = true;
        musicSecondary.loop = true;

        // Adjust volumes
        musicMain.volume = 1f;
        musicSecondary.volume = 0.3f;
        sfxSource.volume = 1f;

        // Start initial music
        PlaySegaaaaa();
        PlayOverworldMusic();
    }

    private void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // SFX public methods
    public void PlayScorpion()      { PlaySfx(Scorpion); }
    public void PlaySegaaaaa()      { PlaySfx(Segaaaaa); }
    public void PlaySteveDamage()   { PlaySfx(SteveDamage); }
    public void PlayAntoineDaniel() { PlaySfx(AntoineDaniel); }
    public void PlayGameOver()      { PlaySfx(GameOver); }
    public void PlaySqualala()      { PlaySfx(Squalala); }

    // Overworld music with 1/4 chance to overlap
    public void PlayOverworldMusic()
    {
        musicMain.Stop();
        musicSecondary.Stop();

        if (Random.Range(0, 4) == 0) // 1/4 chance
        {
            musicMain.clip = overworldMusic;
            musicSecondary.clip = Visitor;
            musicMain.volume = 0.65f;
            musicSecondary.volume = 0.255f;
            musicMain.Play();
            musicSecondary.Play();
        } else {
            musicMain.clip = Random.Range(0, 2) == 0 ? overworldMusic : Visitor;
            musicMain.volume = 0.85f;
            musicMain.Play();
        }
    }

    void Update()
    {
        if (!musicMain.isPlaying && !musicSecondary.isPlaying)
        {
            PlayOverworldMusic();
        }
    }
}

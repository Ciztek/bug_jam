using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicType
{
    Overworld
}

public enum SFXType
{
    Scorpion,
    Segaaaaa,
    SteveDamage,
    AntoineDaniel,
    GameOver,
    Squalala
}


public class AudioPlayerManager : MonoBehaviour
{
    private static AudioPlayerManager instance;

    [Header("SFX Clips")]
    public AudioClip Scorpion;
    public AudioClip Segaaaaa;
    public AudioClip SteveDamage;
    public AudioClip AntoineDaniel;
    public AudioClip GameOver;
    public AudioClip Squalala;

    [Header("Settings")]
    public float musicVolume = 0.6f;
    public float sfxVolume = 1.0f;
    public float crossfadeDuration = 2.0f;

    private AudioSource musicSourceA;
    private AudioSource musicSourceB;
    private AudioSource sfxSource;

    private AudioSource activeMusicSource;
    private AudioSource inactiveMusicSource;

    private AudioClip[] overworldPlaylist;
    private int currentTrackIndex = 0;

    private Dictionary<SFXType, AudioClip> sfxMap;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSourceA = gameObject.AddComponent<AudioSource>();
        musicSourceB = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        // Music sources
        musicSourceA.loop = false;
        musicSourceB.loop = false;
        musicSourceA.playOnAwake = false;
        musicSourceB.playOnAwake = false;

        musicSourceA.volume = musicVolume;
        musicSourceB.volume = 0f;

        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;

        // Active / inactive swap setup
        activeMusicSource = musicSourceA;
        inactiveMusicSource = musicSourceB;

        // SFX enum → clip map
        sfxMap = new Dictionary<SFXType, AudioClip>
        {
            { SFXType.Scorpion, Scorpion },
            { SFXType.Segaaaaa, Segaaaaa },
            { SFXType.SteveDamage, SteveDamage },
            { SFXType.AntoineDaniel, AntoineDaniel },
            { SFXType.GameOver, GameOver },
            { SFXType.Squalala, Squalala }
        };
    }

    private void Start()
    {
        overworldPlaylist = Resources.LoadAll<AudioClip>("clair_obscur");

        if (overworldPlaylist.Length > 0)
        {
            ShufflePlaylist();
            PlayNextOverworldTrack();
        }
        else
        {
            Debug.LogWarning("No music found in Resources/clair_obscur");
        }
    }

    private void Update()
    {
        if (!activeMusicSource.isPlaying && overworldPlaylist.Length > 0)
        {
            PlayNextOverworldTrack();
        }
    }

    /* =========================
       MUSIC
       ========================= */

    public void PlayMusic(MusicType type)
    {
        switch (type)
        {
            case MusicType.Overworld:
                PlayNextOverworldTrack();
                break;
        }
    }

    private void PlayNextOverworldTrack()
    {
        AudioClip nextClip = overworldPlaylist[currentTrackIndex];
        currentTrackIndex = (currentTrackIndex + 1) % overworldPlaylist.Length;

        StartCoroutine(Crossfade(nextClip));
    }

    private IEnumerator Crossfade(AudioClip newClip)
    {
        inactiveMusicSource.clip = newClip;
        inactiveMusicSource.volume = 0f;
        inactiveMusicSource.Play();

        float time = 0f;

        while (time < crossfadeDuration)
        {
            time += Time.deltaTime;
            float t = time / crossfadeDuration;

            inactiveMusicSource.volume = Mathf.Lerp(0f, musicVolume, t);
            activeMusicSource.volume = Mathf.Lerp(musicVolume, 0f, t);

            yield return null;
        }

        activeMusicSource.Stop();

        // swap sources
        AudioSource temp = activeMusicSource;
        activeMusicSource = inactiveMusicSource;
        inactiveMusicSource = temp;

        activeMusicSource.volume = musicVolume;
    }

    private void ShufflePlaylist()
    {
        for (int i = 0; i < overworldPlaylist.Length; i++)
        {
            int j = Random.Range(i, overworldPlaylist.Length);
            AudioClip temp = overworldPlaylist[i];
            overworldPlaylist[i] = overworldPlaylist[j];
            overworldPlaylist[j] = temp;
        }
    }

    /* =========================
       SFX
       ========================= */

    public void PlaySFX(SFXType type)
    {
        if (!sfxMap.ContainsKey(type) || sfxMap[type] == null)
            return;

        sfxSource.PlayOneShot(sfxMap[type], sfxVolume);
    }
}

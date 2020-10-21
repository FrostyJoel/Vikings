using UnityEngine;
using UnityEngine.Audio;
using System;

public class SC_AudioManager : MonoBehaviour
{
    public static SC_AudioManager single;

    public AudioMixer MasterMixer;
    public Sound[] sounds;
    public Music[] music;

    [HideInInspector]
    public bool volumeHalfed;

    private void Awake()
    {
        if (single != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            single = this;
        }
    }
    private void Start()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixer;
        }

        foreach (Music m in music)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.audioClip;
            m.source.volume = m.volume;
            m.source.pitch = m.pitch;
            m.source.loop = true;
            m.source.outputAudioMixerGroup = m.mixer;
        }
    }

    #region Sounds
    public void PlaySound(AudioType typeSound)
    {
        Sound s = Array.Find(sounds, sound => sound.audioType == typeSound);
        s.source.Play();
    }

    public bool IsPlaying(AudioType typeSound)
    {
        Sound s = Array.Find(sounds, sound => sound.audioType == typeSound);
        return s.source.isPlaying;
    }

    public void StopSound(AudioType typeSound)
    {
        Sound s = Array.Find(sounds, sound => sound.audioType == typeSound);
        s.source.Stop();
    }

    #endregion

    #region Music
    public void PlayMusic(MusicType typeMusic)
    {
        Music m = Array.Find(music, music => music.musicType == typeMusic);
        m.source.Play();
    }

    public bool IsPlaying(MusicType typeMusic)
    {
        Music m = Array.Find(music, music => music.musicType == typeMusic);
        return m.source.isPlaying;
    }
    #endregion


    public void UpdateVolume(float volumeAmount)
    {
        MasterMixer.SetFloat("MasterVol", Mathf.Log10 (volumeAmount)*20);
    }

    public void HalfCurrentMusic(MusicType typeMusic)
    {
        GetAudioSource(typeMusic).volume /= 2f;
        volumeHalfed = true;
    }

    public void DoubleCurrentMusic(MusicType typeMusic)
    {
        GetAudioSource(typeMusic).volume *= 2f;
        volumeHalfed = false;
    }

    public AudioSource GetAudioSource(MusicType typeMusic)
    {
        Music m = Array.Find(music, music=> music.musicType == typeMusic);
        return m.source;
    }
}

[System.Serializable]
public class Sound
{
    public AudioType audioType;
    public AudioMixerGroup mixer;
    public AudioClip audioClip;

    [Range(0f,1f)]
    public float volume;
    [Range(0.1f,3f)]
    public float pitch;
    public bool loop;


    [HideInInspector]
    public AudioSource source;
}

public enum AudioType
{
    PlayerAttack,
    PlayerCharge,
    PlayerThrow,
    PlayerCatch,
    PlayerTakeDamage,
    PlayerDeath,

    LeftFoot,
    RightFoot,

    EnemyHowl,
    EnemyAttack,
    EnemyTakeDamage,
    EnemyDeath,

    Lightning
}

[System.Serializable]
public class Music
{
    public MusicType musicType;
    public AudioMixerGroup mixer;
    public AudioClip audioClip;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}

public enum MusicType
{
    MainMenuTheme,
    CombatTheme,
    GameOverScreenTheme,
    VictoryScreenTheme
}

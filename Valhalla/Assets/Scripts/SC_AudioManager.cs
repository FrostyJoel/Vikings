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
        UpdateVolume(SC_UiManager.single.volumeSlider.value);
    }

    #region Sounds
    public void PlaySound(AudioType typeSound)
    {
        Sound s = Array.Find(sounds, sound => sound.audioType == typeSound);
        s.source.Play();
    }

    public bool IsPlayingSound(AudioType typeSound)
    {
        Sound s = Array.Find(sounds, sound => sound.audioType == typeSound);
        return s.source.isPlaying;
    }

    public void StopSound(AudioType typeSound)
    {
        Sound s = Array.Find(sounds, sound => sound.audioType == typeSound);
        s.source.Stop();
    }

    public void StopAllSound()
    {
        foreach (Sound sound in sounds)
        {
            sound.source.Stop();
        }
    }
    public AudioSource GetSoundSource(AudioType typeSound)
    {
        Sound s = Array.Find(sounds, sound => sound.audioType == typeSound);
        return s.source;
    }

    #endregion

    #region Music
    public void PlayMusic(MusicType typeMusic)
    {
        Music m = Array.Find(music, music => music.musicType == typeMusic);
        m.source.Play();
    }

    public void StopMusic(MusicType typeMusic)
    {
        Music m = Array.Find(music, music => music.musicType == typeMusic);
        m.source.Stop();
    }

    public void StopAllMusic()
    {
        foreach (Music music in music)
        {
            music.source.Stop();
        }
    }

    public bool IsPlayingMusic(MusicType typeMusic)
    {
        Music m = Array.Find(music, music => music.musicType == typeMusic);
        return m.source.isPlaying;
    }
    #endregion

    public void UpdateVolume(float volumeAmount)
    {
        MasterMixer.SetFloat("MasterVol", Mathf.Log10 (volumeAmount)*20);
    }
    public AudioSource GetMusicSource(MusicType typeMusic)
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

    Lightning,
    ButtonSound
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
    VictoryScreenTheme,
    LoadingScreenAmbient
}

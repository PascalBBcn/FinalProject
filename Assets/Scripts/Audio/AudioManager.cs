using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] music, sfx;
    public AudioSource musicSrc, sfxSrc;

    // SINGLETON PATTERN
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PlayMusic(string name)
    {
        Sound sound = Array.Find(music, n => n.audioName == name);
        if (sound == null) Debug.Log("Sound missing");
        else
        {
            musicSrc.clip = sound.audioClip;
            musicSrc.Play();
        }
    }
    public void PlaySFX(string name)
    {
        Sound sound = Array.Find(sfx, n => n.audioName == name);
        if (sound == null) Debug.Log("Sound missing");
        else sfxSrc.PlayOneShot(sound.audioClip);

    }
    public void PlayLoopSFX(string name)
    {
        Sound sound = Array.Find(sfx, n => n.audioName == name);
        if (sound == null) Debug.Log("Sound missing");
        else
        {
            sfxSrc.clip = sound.audioClip;
            sfxSrc.loop = true;
            if (!sfxSrc.isPlaying) sfxSrc.Play();
        }

    }
    public void StopLoopSFX()
    {
        if (sfxSrc.isPlaying)
        {
            sfxSrc.Stop();
            sfxSrc.loop = false;
        }
    }
    public void ToggleMusic()
    {
        musicSrc.mute = !musicSrc.mute;
    }
    public void ToggleSFX()
    {
        sfxSrc.mute = !sfxSrc.mute;
    }
    public void VolumeMusic(float volume)
    {
        musicSrc.volume = volume;
    }
    public void VolumeSFX(float volume)
    {
        sfxSrc.volume = volume;
    }


}

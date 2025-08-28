using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] music, sfx;
    public AudioSource musicSrc, sfxSrc;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        PlayMusic("Music");
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

}

using UnityEngine;
using UnityEngine.UI;

public class UIControls : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    // Syncs slider positions to current actual value
    void Start()
    {
        if (_musicSlider != null) _musicSlider.value = AudioManager.Instance.musicSrc.volume;
        if (_sfxSlider != null) _sfxSlider.value = AudioManager.Instance.sfxSrc.volume;
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }
    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
    }
    public void VolumeMusic()
    {
        AudioManager.Instance.VolumeMusic(_musicSlider.value);
    }
    public void VolumeSFX()
    {
        AudioManager.Instance.VolumeSFX(_sfxSlider.value);
    }
}

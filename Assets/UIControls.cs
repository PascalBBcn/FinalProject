using UnityEngine;
using UnityEngine.UI;

public class UIControls : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

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

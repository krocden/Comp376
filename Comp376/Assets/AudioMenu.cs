using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioMenu : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider announcerVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip sliderChangeSound;

    
    private void OnEnable()
    {
        masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
        musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        announcerVolumeSlider.value = AudioManager.Instance.GetAnnouncerVolume();
        sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
    }

    public void UpdateMasterVolume(float vol)
    {
        if (!gameObject.activeSelf) return;
        AudioManager.Instance.SetMasterVolume(vol);
        AudioManager.Instance.PlaySFX(sliderChangeSound);
    }

    public void UpdateMusicVolume(float vol)
    {
        if (!gameObject.activeSelf) return;
        AudioManager.Instance.SetMusicVolume(vol);
        AudioManager.Instance.PlaySFX(sliderChangeSound);
    }

    public void UpdateAnnouncerVolume(float vol)
    {
        if (!gameObject.activeSelf) return;
        AudioManager.Instance.SetAnnouncerVolume(vol);
        AudioManager.Instance.PlaySFX(sliderChangeSound);
    }

    public void UpdateSFXVolume(float vol)
    {
        if (!gameObject.activeSelf) return;
        AudioManager.Instance.SetSFXVolume(vol);
        AudioManager.Instance.PlaySFX(sliderChangeSound);
    }

    public void Close()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        gameObject.SetActive(false);
    }
}

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

    private void Awake()
    {
        Close();
    }

    private void OnEnable()
    {
        masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
        musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        announcerVolumeSlider.value = AudioManager.Instance.GetAnnouncerVolume();
        sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
    }

    public void UpdateMasterVolume(float vol)
    {
        AudioManager.Instance.SetMasterVolume(vol);
    }

    public void UpdateMusicVolume(float vol)
    {
        AudioManager.Instance.SetMusicVolume(vol);
    }

    public void UpdateAnnouncerVolume(float vol)
    {
        AudioManager.Instance.SetAnnouncerVolume(vol);
    }

    public void UpdateSFXVolume(float vol)
    {
        AudioManager.Instance.SetSFXVolume(vol);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}

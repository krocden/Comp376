using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private float currentMasterVolume;
    private float currentMusicVolume;
    private float currentAnnouncerVolume;
    private float currentSFXVolume;

    [SerializeField] private AudioSource musicSource;
    public AudioSource announcerSource;
    public AudioSource sfxSource;
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        currentMasterVolume = PlayerPrefs.HasKey("master_volume") ? PlayerPrefs.GetFloat("master_volume") : 1f;
        currentMusicVolume = PlayerPrefs.HasKey("music_volume") ? PlayerPrefs.GetFloat("music_volume") : 1f;
        currentAnnouncerVolume = PlayerPrefs.HasKey("announcer_volume") ? PlayerPrefs.GetFloat("announcer_volume") : 1f;
        currentSFXVolume = PlayerPrefs.HasKey("sfx_volume") ? PlayerPrefs.GetFloat("sfx_volume") : 1f;

        SetMasterVolume(currentMasterVolume);
        SetMusicVolume(currentMusicVolume);
        SetAnnouncerVolume(currentAnnouncerVolume);
        SetSFXVolume(currentSFXVolume);
    }

    private void Start()
    {
        
    }

    public void SetMasterVolume(float vol)
    {
        currentMasterVolume = vol;
        AudioListener.volume = vol;
        PlayerPrefs.SetFloat("master_volume", vol);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float vol)
    {
        currentMusicVolume = vol;
        musicSource.volume = vol;
        PlayerPrefs.SetFloat("music_volume", vol);
        PlayerPrefs.Save();
    }

    public void SetAnnouncerVolume(float vol)
    {
        currentAnnouncerVolume = vol;
        announcerSource.volume = vol;
        PlayerPrefs.SetFloat("announcer_volume", vol);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float vol)
    {
        currentSFXVolume = vol;
        sfxSource.volume = vol;
        PlayerPrefs.SetFloat("sfx_volume", vol);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume()
    {
        return currentMasterVolume;
    }

    public float GetMusicVolume()
    {
        return currentMusicVolume;
    }

    public float GetAnnouncerVolume()
    {
        return currentAnnouncerVolume;
    }

    public float GetSFXVolume()
    {
        return currentSFXVolume;
    }

    public void PlayClip(AudioClip clip)
    {
        announcerSource.clip = clip;
        announcerSource.Play();
        FadeMusic();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    private async Task FadeMusic()
    {
        float baseVol = currentMusicVolume;
        while (announcerSource.isPlaying)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, baseVol / 5, Time.deltaTime * 4f);
            await Task.Yield();
        }

        while(musicSource.volume != baseVol)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, baseVol, Time.deltaTime * 4f);
            await Task.Yield();
        }
    }
}

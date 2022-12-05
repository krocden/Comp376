using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private AudioClip buttonClickSound;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OpenAudioPanel()
    {
        audioPanel.SetActive(true);
        AudioManager.Instance.PlaySFX(buttonClickSound);
    }

    public void OpenHelpPanel()
    {
        //helpPanel.SetActive(true);
        AudioManager.Instance.PlaySFX(buttonClickSound);
    }

    public void Close(bool playSound=true)
    {
        gameObject.SetActive(false);
        audioPanel.SetActive(false);
        if(playSound)
            AudioManager.Instance.PlaySFX(buttonClickSound);
        //helpPanel.SetActive(false);
    }
}

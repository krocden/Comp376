using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private AudioClip buttonClickSound;

    private void OnEnable()
    {
        audioPanel.SetActive(false);
        //helpPanel.SetActive(false);
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

    public void Close()
    {
        gameObject.SetActive(false);
        AudioManager.Instance.PlaySFX(buttonClickSound);
    }
}

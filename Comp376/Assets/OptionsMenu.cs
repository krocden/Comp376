using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject helpPanel;

    private void Awake()
    {
        Close();
    }

    public void OpenAudioPanel()
    {
        audioPanel.SetActive(true);
    }

    public void OpenHelpPanel()
    {
        helpPanel.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        audioPanel.SetActive(false);
        helpPanel.SetActive(false);
    }
}

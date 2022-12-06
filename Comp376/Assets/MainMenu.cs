using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private AudioClip buttonClickSound;

    public void StartGame()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        SceneManager.LoadScene("Arena");
    }

    public void OpenOptionsMenu()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        optionsMenu.SetActive(true);
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        Application.Quit();
    }
}

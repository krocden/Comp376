using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private GameObject difficultyMenu;

    public void OpenDifficultySelection()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        difficultyMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void StartGame(int difficulty)
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        PlayerPrefs.SetInt("level", difficulty);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Arena");
    }

    public void OpenOptionsMenu()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        optionsMenu.SetActive(true);
        difficultyMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void OpenCreditsMenu()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        creditsMenu.SetActive(true);
        difficultyMenu.SetActive(false);
        optionsMenu.SetActive(false);
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject optionsMenu;

    public void StartGame()
    {
        SceneManager.LoadScene("Arena");
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

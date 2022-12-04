using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private OptionsMenu optionsMenu;

    private void Awake()
    {
        container.SetActive(false);
    }

    private void Update()
    {
        if (GameStateManager.Instance.BlockInput && !GameStateManager.Instance.gamePaused) return;
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.gameObject.SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleMenu()
    {
        container.SetActive(!container.activeSelf);

        Time.timeScale = container.activeSelf ? 0 : 1;
        GameStateManager.Instance.gamePaused = container.activeSelf;

        if (!container.activeSelf)
            optionsMenu.Close();
    }
}

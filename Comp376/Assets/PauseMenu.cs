using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private OptionsMenu optionsMenu;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip openMenuSound;

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
        AudioManager.Instance.PlaySFX(buttonClickSound);
        optionsMenu.gameObject.SetActive(true);
    }

    public void MainMenu()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        Application.Quit();
    }

    public void ToggleMenu()
    {
        AudioManager.Instance.PlaySFX(openMenuSound);
        container.SetActive(!container.activeSelf);

        Time.timeScale = container.activeSelf ? 0 : 1;
        GameStateManager.Instance.gamePaused = container.activeSelf;

        if (!container.activeSelf)
            optionsMenu.Close(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private OptionsMenu optionsMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.gameObject.SetActive(true);
    }

    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        Time.timeScale = gameObject.activeSelf ? 0 : 1;

        if (!gameObject.activeSelf)
            optionsMenu.Close();
    }
}

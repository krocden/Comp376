using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpMeMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private ToggleGroup infoPageToggleGroup;

    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject gameLogicMenu;
    [SerializeField] private GameObject turretsMenu;
    [SerializeField] private GameObject gunsMenu;
    [SerializeField] private GameObject enemiesMenu;

    [SerializeField] private AudioClip buttonClickSound;


    private void Start()
    {
        
    }


    public void OnToggleSelected(bool selected)
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);

        string subMenuTitle = infoPageToggleGroup.GetFirstActiveToggle().GetComponent<TextMeshProUGUI>().text;
        title.text = subMenuTitle;

        controlsMenu.SetActive(false);
        gameLogicMenu.SetActive(false);
        turretsMenu.SetActive(false);
        gunsMenu.SetActive(false);
        enemiesMenu.SetActive(false);

        switch (subMenuTitle)
        {            
            case "Controls":
                controlsMenu.SetActive(true);
                break;
            case "Game Logic":
                gameLogicMenu.SetActive(true);
                break;
            case "Turrets":
                turretsMenu.SetActive(true);
                break;
            case "Guns":
                gunsMenu.SetActive(true);
                break;
            case "Enemies":
                enemiesMenu.SetActive(true);
                break;
        }
    }

    public void Close()
    {
        AudioManager.Instance.PlaySFX(buttonClickSound);
        gameObject.SetActive(false);
    }
}

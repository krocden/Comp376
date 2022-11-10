using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoreUIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentPhaseUI;
    [SerializeField] private GameObject buildingPhaseUIGroup;
    [SerializeField] private GameObject shootingPhaseUIGroup;
    [SerializeField] private GameObject transitionUIGroup;
    [SerializeField] private GameObject additionalInfoUIGroup;

    [SerializeField] private TextMeshProUGUI buildingPhaseSecondsLeftUI;
    [SerializeField] private TextMeshProUGUI monstersLeftCountUI;
    [SerializeField] private TextMeshProUGUI monstersLeftTextUI;
    [SerializeField] private TextMeshProUGUI transitionTimeLeftTextUI;
    [SerializeField] private TextMeshProUGUI additionalInfoTextUI;

    [SerializeField] private Slider nexusHealthSlider;
    [SerializeField] private TextMeshProUGUI nexusHealthText;

    private void Start()
    {
        GameStateManager.Instance.onGameStateChanged.AddListener(UpdateUIGroup);
        GameStateManager.Instance.onGameStateTransitionStarted.AddListener(UpdateStateTransitionUI);
    }


    public void UpdateUIGroup(GameState state)
    {
        buildingPhaseUIGroup.SetActive(state == GameState.Planning);
        shootingPhaseUIGroup.SetActive(state == GameState.Shooting);
        transitionUIGroup.SetActive(false);

        StringBuilder currentPhaseText = new StringBuilder(Enum.GetName(typeof(GameState), state));
        currentPhaseText.Append(state == GameState.Planning || state == GameState.Shooting ? " Phase" : " Game");

        if (GameStateManager.Instance.GetCurrentGameState() == GameState.Completed && GameStateManager.Instance.levelFailed)
        {
            currentPhaseText.AppendLine("\n GAME OVER \n");
            additionalInfoUIGroup.SetActive(true);
            additionalInfoTextUI.text = "The enemies have crossed the dimensional portal and have destroyed the mortal realm.";
        }

        currentPhaseUI.text = currentPhaseText.ToString();
    }

    public void UpdateStateTransitionUI(GameState currentState, GameState nextState)
    {
        buildingPhaseUIGroup.SetActive(false);
        shootingPhaseUIGroup.SetActive(false);
        transitionUIGroup.SetActive(true);

        string text = "";
        if (currentState == GameState.Initialize)
        {
            text = "Starting game in ";
        }
        else
        {
            if (nextState == GameState.Planning)
            {
                text = "Next Round Starting in ";
            }
            else if (nextState == GameState.Shooting)
            {
                text = "Enemies Spawning in ";
            }
        }

        currentPhaseUI.text = text;
    }

    public void UpdateStateTransitionTimeLeft(float timeLeft)
    {
        transitionTimeLeftTextUI.text = timeLeft.ToString("0");
    }

    public void UpdateBuildingSecondsLeft(float timeLeft)
    {
        buildingPhaseSecondsLeftUI.text = timeLeft.ToString("0");
    }

    public void UpdateMonstersLeft(int monstersLeft)
    {
        bool showMonstersLeft = monstersLeft != 0;

        monstersLeftCountUI.gameObject.SetActive(showMonstersLeft);
        monstersLeftTextUI.gameObject.SetActive(showMonstersLeft);
        monstersLeftCountUI.text = monstersLeft.ToString();
    }

    public void UpdateNexusHealthSlider(float health, float maxHealth)
    {
        nexusHealthText.text = health.ToString();
        nexusHealthSlider.value = health / maxHealth;
    }
}

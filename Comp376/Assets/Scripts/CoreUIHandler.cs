using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class CoreUIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentPhaseUI;
    [SerializeField] private GameObject buildingPhaseUIGroup;
    [SerializeField] private GameObject shootingPhaseUIGroup;
    [SerializeField] private GameObject transitionUIGroup;

    [SerializeField] private TextMeshProUGUI buildingPhaseSecondsLeftUI;
    [SerializeField] private TextMeshProUGUI monstersLeftCountUI;
    [SerializeField] private TextMeshProUGUI monstersLeftTextUI;
    [SerializeField] private TextMeshProUGUI transitionTimeLeftTextUI;

    private void Start()
    {
        GameStateManager.gameStateManager.onGameStateChanged.AddListener(UpdateUIGroup);
        GameStateManager.gameStateManager.onGameStateTransitionStarted.AddListener(UpdateStateTransitionUI);
    }


    public void UpdateUIGroup(GameState state)
    {
        buildingPhaseUIGroup.SetActive(state == GameState.Building);
        shootingPhaseUIGroup.SetActive(state == GameState.Shooting);
        transitionUIGroup.SetActive(false);

        StringBuilder currentPhaseText = new StringBuilder(Enum.GetName(typeof(GameState), state));
        currentPhaseText.Append(state == GameState.Building || state == GameState.Shooting ? " Phase" : " Game");

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
            if (nextState == GameState.Building)
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
}

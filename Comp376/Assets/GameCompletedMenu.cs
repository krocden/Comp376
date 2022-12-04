using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCompletedMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuContainer;

    [SerializeField] private TextMeshProUGUI completedText;
    [SerializeField] private TextMeshProUGUI currencyScore;
    [SerializeField] private TextMeshProUGUI highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.Instance.onGameStateChanged.AddListener((state) => OnGameStateChanged(state));
        menuContainer.SetActive(false);
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Completed)
        {
            ShowCompletedMenu(!GameStateManager.Instance.levelFailed);
        }
    }

    private void ShowCompletedMenu(bool success)
    {
        menuContainer.SetActive(true);

        completedText.text = success ? "You Win" : "You Lose";

        int score = CurrencyManager.Instance.totalCurrencyEarned * GameStateManager.Instance.currentWave;
        currencyScore.text = score.ToString();
        int highScore = score;
        if (PlayerPrefs.HasKey("highscore"))
        {
            if (score < PlayerPrefs.GetInt("highscore"))
                highScore = PlayerPrefs.GetInt("highscore");
        }
        else
        {
            PlayerPrefs.SetInt("highscore", highScore);
        }
        PlayerPrefs.SetInt("highscore", highScore);
        PlayerPrefs.Save();

        highScoreText.text = highScore.ToString();

    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

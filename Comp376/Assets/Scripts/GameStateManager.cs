using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager gameStateManager;

    [SerializeField] private CoreUIHandler coreUIHandler;
    [SerializeField] private ArenaSetup arenaSetup;

    public UnityEvent<GameState> onGameStateChanged = new UnityEvent<GameState>();
    public UnityEvent<GameState, GameState> onGameStateTransitionStarted = new UnityEvent<GameState, GameState>();

    private GameState currentGameState;

    private int currentRound = 0;
    private int totalRounds = 30;

    [SerializeField] private float buildingPhaseTimer = 30;
    [SerializeField] private float gameStateTransitionTimer = 5f;
    [SerializeField] private float timeBetweenEnemySpawns = 0.5f;

    [SerializeField] private Monster monsterPrefab;

    private CancellationTokenSource timerCancellationTokenSource;
    private bool canSkipPhase = false;

    private Dictionary<int, List<GameObject>> roundEnemies = new Dictionary<int, List<GameObject>>()
    {
        {1, new List<GameObject>(100) { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null } },
        {2, new List<GameObject>(100) { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null }},
        {3, new List<GameObject>(100) { null, null, null, null, null, null, null, null, null, null, null,  }},
        {4, new List<GameObject>(100) { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null }},
        {5, new List<GameObject>(100) { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, }}
    };

    private void Awake()
    {
        gameStateManager = this;
    }

    private void Start()
    {
        GoToState(GameState.Initialize);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && canSkipPhase)
        {
            if(currentGameState == GameState.Building)
                timerCancellationTokenSource.Cancel();
            else
                GoToNextState();
        }
    }

    private async void TransitionToState(GameState state)
    {
        canSkipPhase = false;
        onGameStateTransitionStarted?.Invoke(currentGameState, state);

        timerCancellationTokenSource = new CancellationTokenSource();

        Progress<float> timeTicking = new Progress<float>();

        timeTicking.ProgressChanged += (object sender, float e) =>
        {
            coreUIHandler.UpdateStateTransitionTimeLeft(e);
        };

        await StartTimer(gameStateTransitionTimer, timerCancellationTokenSource.Token, timeTicking);

        GoToState(state);
    }


    private async void GoToState(GameState state)
    {
        currentGameState = state;
        onGameStateChanged?.Invoke(currentGameState);
        switch (state)
        {
            case GameState.Initialize:
                SetInitializeState();
                break;
            case GameState.Building:
                await SetBuildingPhase();
                break;
            case GameState.Shooting:
                SetShootingPhase();
                break;
            case GameState.Completed:
                break;
        }       
    }

    private void GoToNextState()
    {
        switch (currentGameState)
        {
            case GameState.Initialize:
                currentRound++;
                TransitionToState(GameState.Building);
                break;
            case GameState.Building:
                TransitionToState(GameState.Shooting);
                break;
            case GameState.Shooting:
                currentRound++;
                if (currentRound == roundEnemies.Count)
                    GoToState(GameState.Completed);
                else
                    TransitionToState(GameState.Building);
                break;
        }
    }

    public GameState GetCurrentGameState()
    {
        return currentGameState;
    }

    private void SetInitializeState()
    {
        canSkipPhase = true;
        arenaSetup.InitializeArena();
    }

    private async Task SetBuildingPhase()
    {
        canSkipPhase = true;
        timerCancellationTokenSource = new CancellationTokenSource();

        Progress<float> timeTicking = new Progress<float>();

        timeTicking.ProgressChanged += (object sender, float e) =>
        {
            coreUIHandler.UpdateBuildingSecondsLeft(e);
        };        

        float timeLeft = await StartTimer(buildingPhaseTimer, timerCancellationTokenSource.Token, timeTicking);

        if (timeLeft > 0) { } // add currency to the player

        GoToNextState();
    }

    private async void SetShootingPhase()
    {
        canSkipPhase = false;
        coreUIHandler.UpdateMonstersLeft(0);
        List<GameObject> enemies = roundEnemies[currentRound];
        foreach (GameObject enemy in enemies)
        {
            foreach (List<PathNode> path in arenaSetup.paths)
            {
                Vector3 spawnPoint = path[0].position + Vector3.up;
                Monster monster = Instantiate(monsterPrefab, spawnPoint, Quaternion.identity);
                monster.Initialize(path, arenaSetup.nexus.nexusBase);
                monster.health = 35;
            }

            float timeSinceLastSpawn = 0;
            while (timeSinceLastSpawn < timeBetweenEnemySpawns)
            {
                timeSinceLastSpawn += Time.deltaTime;
                await Task.Yield();
            }
        }

        int enemiesLeft = FindObjectsOfType<Monster>().Length;

        while (enemiesLeft > 0)
        {
            await Task.Yield();
            enemiesLeft = FindObjectsOfType<Monster>().Length;
            coreUIHandler.UpdateMonstersLeft(enemiesLeft);
        }

        GoToNextState();
    }

    private async Task<float> StartTimer(float timer, CancellationToken token, IProgress<float> progress = null)
    {
        float timeElapsed = 0f;
        while (timeElapsed <= timer)
        {
            if (token.IsCancellationRequested)
            {
                return timer - timeElapsed;
            }

            if(progress != null)
                progress.Report(timer - timeElapsed);

            timeElapsed += Time.deltaTime;
            await Task.Yield();
        }

        return 0f;
    }


}

public enum GameState
{ 
    Initialize,
    Building,
    Shooting,
    Completed,
}

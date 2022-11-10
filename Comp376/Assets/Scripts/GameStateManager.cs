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
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private CoreUIHandler coreUIHandler;
    [SerializeField] private ArenaSetup arenaSetup;

    public UnityEvent<GameState> onGameStateChanged = new UnityEvent<GameState>();
    public UnityEvent<GameState, GameState> onGameStateTransitionStarted = new UnityEvent<GameState, GameState>();

    private GameState currentGameState;

    private int currentLevel = 1;
    private int currentWave = 0;

    private Level level;
    private Wave[] waves;

    [SerializeField] private float buildingPhaseTimer = 30;
    [SerializeField] private float gameStateTransitionTimer = 5f;
    [SerializeField] private float timeBetweenEnemySpawns = 0.5f;

    private CancellationTokenSource timerCancellationTokenSource = new CancellationTokenSource();
    private CancellationTokenSource spawningCancellationTokenSource = new CancellationTokenSource();
    private bool canSkipPhase = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        level = LoadLevel();
        waves = LoadWaves();

        GoToState(GameState.Initialize);
    }

    private Level LoadLevel()
    {
        return Resources.Load<Level>($"Levels\\Level {currentLevel}\\Level {currentLevel}");
    }

    private Wave[] LoadWaves()
    {
        return Resources.LoadAll<Wave>($"Levels\\Level {currentLevel}\\Waves");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && canSkipPhase)
        {
            if (currentGameState == GameState.Building)
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
                TransitionToState(GameState.Building);
                break;
            case GameState.Building:
                TransitionToState(GameState.Shooting);
                break;
            case GameState.Shooting:
                currentWave++;
                if (currentWave == waves.Length)
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
        canSkipPhase = false;
        if (currentWave % level.addSpawnerWaveInterval == 0 && level.maximumSpawners > arenaSetup.pathStartEndCoordinatesList.Count)
        {
            for (int i = 0; i < level.numberOfSpawnersToAdd; i++)
                await arenaSetup.AddPath();
        }
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
        List<MonsterDetails> monsterDetails = waves[currentWave].GetMonsterDetails();
        for (int i = 0; i < monsterDetails.Count; i++)
        {
            bool repeatGroup = monsterDetails[i].commandType == CommandType.RepeatGroup;
            int repeatNextXRows = repeatGroup ? monsterDetails[i].repeatNextXRows : 1;
            int repeatTimes = repeatGroup ? monsterDetails[i].repeatTimes : 1;

            if (repeatGroup)
                i++;

            for (int y = 0; y < repeatTimes; y++)
            {
                for (int x = 0; x < repeatNextXRows; x++)
                {
                    // spawn the monsterQuantity
                    for (int j = 0; j < monsterDetails[i + x].monsterQuantity; j++)
                    {
                        foreach (List<PathNode> path in arenaSetup.paths)
                        {
                            if (spawningCancellationTokenSource.IsCancellationRequested)
                                return;
                            Vector3 spawnPoint = path[0].position + Vector3.up;
                            Monster monster = Instantiate(monsterDetails[i + x].monsterPrefab, spawnPoint, Quaternion.identity);
                            monster.Initialize(path, arenaSetup.nexus.nexusBase);                            
                        }

                        float timeSinceLastSpawn = 0;
                        while (timeSinceLastSpawn < timeBetweenEnemySpawns)
                        {
                            timeSinceLastSpawn += Time.deltaTime;
                            await Task.Yield();
                        }
                    }
                }
            }

            if (repeatGroup)
                i += repeatNextXRows - 1;            
        }

        int enemiesLeft = FindObjectsOfType<Monster>().Length;

        while (enemiesLeft > 0)
        {
            if (spawningCancellationTokenSource.IsCancellationRequested)
                return;

            enemiesLeft = FindObjectsOfType<Monster>().Length;
            coreUIHandler.UpdateMonstersLeft(enemiesLeft);

            await Task.Yield();
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

            if (progress != null)
                progress.Report(timer - timeElapsed);

            timeElapsed += Time.deltaTime;
            await Task.Yield();
        }

        return 0f;
    }

    private void OnApplicationQuit()
    {
        spawningCancellationTokenSource.Cancel();
        timerCancellationTokenSource.Cancel();
    }
}

public static class Helper
{
    public static int GetSpawnerNumberAtWave(int wave)
    {
        if (wave == 1)
        {
            return 100;
        }
        else if (wave == 2)
            return 200;
        else if (wave == 3)
            return 300;
        else if (wave == 4)
            return 400;
        else
            return 0;
    }
}

public enum GameState
{
    Initialize,
    Building,
    Shooting,
    Completed,
}
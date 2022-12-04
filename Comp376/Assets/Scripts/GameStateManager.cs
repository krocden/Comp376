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

    private CancellationTokenSource timerCancellationTokenSource = new CancellationTokenSource();
    private CancellationTokenSource spawningCancellationTokenSource = new CancellationTokenSource();
    private bool canSkipPhase = false;

    public bool levelFailed = false;
    public bool BlockInput { get { return levelFailed; } }

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
            if (currentGameState == GameState.Planning)
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
            case GameState.Planning:
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
                TransitionToState(GameState.Planning);
                break;
            case GameState.Planning:
                TransitionToState(GameState.Shooting);
                break;
            case GameState.Shooting:
                currentWave++;
                if (currentWave == waves.Length)
                    GoToState(GameState.Completed);
                else
                    TransitionToState(GameState.Planning);
                break;
        }
    }

    public GameState GetCurrentGameState()
    {
        return currentGameState;
    }

    private void SetInitializeState()
    {
        arenaSetup.InitializeArena();

        level = LoadLevel();
        waves = LoadWaves();

        arenaSetup.nexus.OnNexusExploded.AddListener(NexusExploded);

        canSkipPhase = true;
    }

    private async Task SetBuildingPhase()
    {
        if (spawningCancellationTokenSource.IsCancellationRequested)
            return;
        canSkipPhase = false;

        // add spawner
        if (currentWave % level.addSpawnerWaveInterval == 0 && level.maximumSpawners > arenaSetup.pathStartEndCoordinatesList.Count)
        {
            for (int i = 0; i < level.numberOfSpawnersToAdd; i++)
            {
                await arenaSetup.AddPath(false);
            }

            if (currentWave != 0)
                NotificationManager.Instance.PlayStandardNotification(NotificationType.NewSpawnerAdded, true);
        }

        // Add next wave spawner
        if ((currentWave + 1) % level.addSpawnerWaveInterval == 0 && level.maximumSpawners > arenaSetup.pathStartEndCoordinatesList.Count + arenaSetup.nextWavePathStartEndCoordinatesList.Count)
        {
            for (int i = 0; i < level.numberOfSpawnersToAdd; i++)
            {
                await arenaSetup.AddPath(true);
            }
            NotificationManager.Instance.PlayStandardNotification(NotificationType.NewSpawnerNextRound, true);
        }

        canSkipPhase = true;
        timerCancellationTokenSource = new CancellationTokenSource();

        Progress<float> timeTicking = new Progress<float>();

        timeTicking.ProgressChanged += (object sender, float e) =>
        {
            coreUIHandler.UpdateBuildingSecondsLeft(e);
        };

        float timeLeft = await StartTimer(buildingPhaseTimer, timerCancellationTokenSource.Token, timeTicking);

        if (timeLeft > 0) 
        {
            CurrencyManager.Instance.AddCurrency(Mathf.RoundToInt(timeLeft / buildingPhaseTimer * 100));
        }

        GoToNextState();
    }

    private async void SetShootingPhase()
    {
        if (spawningCancellationTokenSource.IsCancellationRequested)
            return;

        canSkipPhase = false;
        coreUIHandler.UpdateMonstersLeft(0);
        NotificationManager.Instance.PlayNotificationSFX(NotificationType.WaveIncoming);
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
                        float timeSinceLastSpawn = 0;
                        while (timeSinceLastSpawn < monsterDetails[i + x].monsterSpawnRate)
                        {
                            timeSinceLastSpawn += Time.deltaTime;
                            await Task.Yield();
                        }

                        foreach (List<PathNode> path in arenaSetup.paths)
                        {
                            if (spawningCancellationTokenSource.IsCancellationRequested)
                                return;

                            Vector3 spawnPoint = path[0].position + Vector3.up;
                            Monster monster = Instantiate(monsterDetails[i + x].monsterPrefab, spawnPoint, Quaternion.identity);
                            monster.Initialize(path, arenaSetup.nexus);                            
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

        NotificationManager.Instance.PlayNotificationSFX(NotificationType.WaveCompleted);

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

    public void UpdateNexusHealth(float health, float maxHealth)
    {
        coreUIHandler.UpdateNexusHealthSlider(health, maxHealth);
    }

    private void NexusExploded()
    {
        spawningCancellationTokenSource.Cancel();
        timerCancellationTokenSource.Cancel();

        levelFailed = true;

        NotificationManager.Instance.PlayNotificationSFX(NotificationType.GameOver);

        GoToState(GameState.Completed);
    }



    private void OnApplicationQuit()
    {
        spawningCancellationTokenSource.Cancel();
        timerCancellationTokenSource.Cancel();
    }
}

public enum GameState
{
    Initialize,
    Planning,
    Shooting,
    Completed,
}

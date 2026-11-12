using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zeke.GlobalEventsBus;

public class Run : GameService<Run>
{
    [Header("Dependency")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas screenCanvas;
    [SerializeField] private Canvas worldCanvas;

    [Header("Dependency")]
    [SerializeField] private GameDifficulty difficulty;

    [Header("Framerate")]
    [SerializeField, Min(1)] private float minCodeFrameRate = 60;

    public int Level { get; set; } = 0;
    public float Difficulty { get; set; } = 0f;
    public float CostMultiplier { get; set; } = 1f;

    public IReadOnlyList<Player> Players => players;

    public Player RandomPlayer
    {
        get
        {
            if (players.Count <= 0) return null;
            else
            {
                return players[UnityEngine.Random.Range(0, players.Count)];
            }
        }
    }

    public Action onPause;
    public Action onResume;

    public Action<Player> onPlayerSpawned;

    public Action<Player> onPlayerAdded;
    public Action<Player> onPlayerRemoved;

    public bool IsPaused => pauseIDs.Count > 0;
    public float RunTimer { get; private set; }

    public Camera MainCamera => Instance.mainCamera;
    public Vector3 MouseWorldPosition => mouseWorldPosition;

    public Canvas ScreenCanvas => Instance.screenCanvas;
    public Canvas WorldCanvas => Instance.worldCanvas;

    private Vector3 mouseWorldPosition = Vector3.zero;

    private float difficultyRamp = 0f;

    private int currentID = int.MinValue;

    private readonly List<Player> players = new List<Player>();

    private readonly List<GameSpeedModifier> timeScaleModifiers = new List<GameSpeedModifier>();
    private readonly HashSet<PauseID> pauseIDs = new HashSet<PauseID>();

    public class PauseID { }

    public class GameSpeedModifier
    {
        public float Modifier { get; private set; }

        public GameSpeedModifier(float modifier)
        {
            Modifier = modifier;
        }
    }

    public void PlayerSpawned(Player player)
    {
        onPlayerSpawned?.Invoke(player);
    }

    public void AddPlayer(Player player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            onPlayerAdded?.Invoke(player);
        }
    }

    public void RemovePlayer(Player player)
    {
        players.Remove(player);
        onPlayerRemoved?.Invoke(player);
    }

    public int GetUniqueID()
    {
        int uniqueID = currentID;
        currentID += 1;

        return uniqueID;
    }

    public void Pause(PauseID pauseID)
    {
        if (pauseID == null) return;
        if (pauseIDs.Contains(pauseID)) return;

        pauseIDs.Add(pauseID);
        UpdateTimeScale();

        if (pauseIDs.Count == 1)
        {
            onPause?.Invoke();
        }
    }

    public void Unpause(PauseID pauseID)
    {
        if (pauseID == null) return;
        if (!pauseIDs.Contains(pauseID)) return;

        pauseIDs.Remove(pauseID);
        UpdateTimeScale();

        if (pauseIDs.Count == 0)
        {
            onResume?.Invoke();
        }
    }

    public void AddTimeScaleModifier(GameSpeedModifier modifier)
    {
        timeScaleModifiers.Add(modifier);
        UpdateTimeScale();
    }

    public void RemoveTimeScaleModifier(GameSpeedModifier modifier)
    {
        timeScaleModifiers.Remove(modifier);
        UpdateTimeScale();
    }

    private void ClearTimeScaleModifiers()
    {
        timeScaleModifiers.Clear();
        UpdateTimeScale();
    }

    private void ClearPauseIDs()
    {
        pauseIDs.Clear();
        UpdateTimeScale();
    }

    private void UpdateTimeScale()
    {
        float timeScale = 1f;

        if (pauseIDs.Count <= 0)
        {
            for (int i = 0; i < timeScaleModifiers.Count; i++)
            {
                timeScale *= timeScaleModifiers[i].Modifier;
            }
        }
        else
        {
            timeScale = 0f;
        }

        Time.timeScale = timeScale;
    }

    protected override void InitializeService()
    {
        Level = 0;
        Time.maximumDeltaTime = 1f / minCodeFrameRate;

        UpdateMouseWorldPosition();
        ResetValuesToDefault();

        ClearPauseIDs();
        ClearTimeScaleModifiers();
    }

    private void OnEnable()
    {
        GlobalEventBus.Subscribe<RunLevelHandler.LevelLoadedEvent>(OnLevelLoaded);
    }

    private void OnDisable()
    {
        GlobalEventBus.Unsubscribe<RunLevelHandler.LevelLoadedEvent>(OnLevelLoaded);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void OnLevelLoaded(RunLevelHandler.LevelLoadedEvent @event)
    {
        Difficulty += difficulty.DifficultyPerLevel * Level;
        Level += 1;
    }

    private void Update()
    {
        RunTimer += Time.deltaTime;
        UpdateMouseWorldPosition();
        UpdateDifficulty();
    }

    private void UpdateMouseWorldPosition()
    {
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = 0;
    }

    private void UpdateDifficulty()
    {
        difficultyRamp += difficulty.DifficultyRampUp * Time.deltaTime;
        Difficulty += (difficulty.DifficultyScaleRate + difficultyRamp) * Time.deltaTime;

        CostMultiplier += difficulty.PriceScalePerSecond * Time.deltaTime;
    }

    private void ResetValuesToDefault()
    {
        Difficulty = difficulty.StartingDifficulty;
        difficultyRamp = 0f;
        CostMultiplier = 1;
        RunTimer = 0f;
    }
}
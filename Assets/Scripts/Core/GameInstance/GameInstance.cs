using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

public class GameInstance : Singleton<GameInstance>
{
    [Header("Dependency")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas screenCanvas;
    [SerializeField] private Canvas worldCanvas;

    [Space]

    [SerializeField] private GameDifficulty difficulty;

    public static float Difficulty { get; private set; } = 0f;
    public static float GoldMultiplier { get; private set; } = 1f;

    public static List<Player> players = new List<Player>();
    public static Player RandomPlayer
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

    public static Action onPause;
    public static Action onResume;

    public static bool IsPaused => pauseIDs.Count > 0;

    public static Camera MainCamera => Instance.mainCamera;
    public static Vector3 MouseWorldPosition => mouseWorldPosition;

    public static Canvas ScreenCanvas => Instance.screenCanvas;
    public static Canvas WorldCanvas => Instance.worldCanvas;

    private static Vector3 mouseWorldPosition = Vector3.zero;

    private static float difficultyRamp = 0f;
    private static int currentID = int.MinValue;

    private readonly static List<GameSpeedModifier> timeScaleModifiers = new List<GameSpeedModifier>();
    private readonly static HashSet<PauseID> pauseIDs = new HashSet<PauseID>();

    public class PauseID { }

    public class GameSpeedModifier
    {
        public float Modifier { get; private set; }

        public GameSpeedModifier(float modifier)
        {
            Modifier = modifier;
        }
    }

    public static void AddPlayer(Player player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    public static void RemovePlayer(Player player)
    {
        players.Remove(player);
    }

    public static int GetUniqueID()
    {
        int uniqueID = currentID;
        currentID += 1;

        return uniqueID;
    }

    public static void Pause(PauseID pauseID)
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

    public static void Unpause(PauseID pauseID)
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

    public static void AddTimeScaleModifier(GameSpeedModifier modifier)
    {
        timeScaleModifiers.Add(modifier);
        UpdateTimeScale();
    }

    public static void RemoveTimeScaleModifier(GameSpeedModifier modifier)
    {
        timeScaleModifiers.Remove(modifier);
        UpdateTimeScale();
    }

    private static void ClearTimeScaleModifiers()
    {
        timeScaleModifiers.Clear();
        UpdateTimeScale();
    }

    private static void ClearPauseIDs()
    {
        pauseIDs.Clear();
        UpdateTimeScale();
    }

    private static void UpdateTimeScale()
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

    protected override void OnInitialization()
    {
        UpdateMouseWorldPosition();
        ResetValuesToDefault();

        ClearPauseIDs();
        ClearTimeScaleModifiers();
    }

    private void Update()
    {
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

        GoldMultiplier += difficulty.PriceScalePerSecond * Time.deltaTime;
    }

    private void ResetValuesToDefault()
    {
        Difficulty = difficulty.StartingDifficulty;
        difficultyRamp = 0f;
        GoldMultiplier = 1;
    }
}
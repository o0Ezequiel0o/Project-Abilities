using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class GameInstance : Singleton<GameInstance>
{
    [Header("Dependency")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Canvas screenCanvas;
    [SerializeField] private Canvas worldCanvas;

    [Header("Settings")]
    [SerializeField] private float startingDifficulty = 0f;
    [SerializeField] private float difficultyScaleRate = 0f;

    public static float Difficulty { get; private set; } = 0f;

    public static List<Player> players = new List<Player>();
    public static Player RandomPlayer
    {
        get
        {
            if (players.Count <= 0) return null;
            else
            {
                return players[Random.Range(0, players.Count)];
            }
        }
    }

    public static Camera MainCamera => Instance.mainCamera;
    public static Vector3 MouseWorldPosition => mouseWorldPosition;

    public static Canvas ScreenCanvas => Instance.screenCanvas;
    public static Canvas WorldCanvas => Instance.worldCanvas;

    private static Vector3 mouseWorldPosition = Vector3.zero;

    private static int currentID = int.MinValue;

    protected override void OnInitialization()
    {
        UpdateMouseWorldPosition();
        Difficulty = startingDifficulty;
    }

    void Update()
    {
        UpdateMouseWorldPosition();
        Difficulty += (difficultyScaleRate * Time.deltaTime);
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

    void UpdateMouseWorldPosition()
    {
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = 0;
    }
}
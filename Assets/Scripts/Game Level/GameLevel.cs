using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Level", menuName = "new Game Level", order = 1)]
public class GameLevel : ScriptableObject
{
    [Header("Instance")]
    [field: SerializeField] public GameLevelInstance Prefab { get; private set; }

    [Header("Progression")]
    [SerializeField] private List<GameLevel> nextLevelPool = new List<GameLevel>();
    [field: SerializeField] public bool StartLevel { get; private set; } = false;

    [field: Space]

    [field: SerializeField] public string ID { get; private set; }

    public IReadOnlyList<GameLevel> NextLevelPool => nextLevelPool;

    public string GetNextLevel()
    {
        return nextLevelPool[Random.Range(0, nextLevelPool.Count)].ID;
    }

    public GameLevelInstance CreateInstance()
    {
        return Instantiate(Prefab, Vector3.zero, Quaternion.identity);
    }
}
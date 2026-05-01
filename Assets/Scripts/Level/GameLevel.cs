using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Level", menuName = "new Game Level", order = 1)]
public class GameLevel : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [SerializeField] private List<GameLevel> nextLevelPool = new List<GameLevel>();

    public GameLevel GetNextLevel()
    {
        return nextLevelPool[Random.Range(0, nextLevelPool.Count)];
    }
}
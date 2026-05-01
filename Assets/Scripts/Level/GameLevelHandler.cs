using System.Collections.Generic;
using UnityEngine;

public class GameLevelHandler : MonoBehaviour
{
    [SerializeField] private List<GameLevel> randomStartLevel;

    private GameLevel currentLevel;
    private GameObject levelInstance;

    private void Awake()
    {
        GlobalEventBus.Subscribe<BossSpawner.LevelBossDeathEvent>(OnLevelBossDeath);

        currentLevel = randomStartLevel[Random.Range(0, randomStartLevel.Count)];
        levelInstance = Instantiate(currentLevel.Prefab, Vector3.zero, Quaternion.identity);
    }

    private void OnLevelBossDeath(BossSpawner.LevelBossDeathEvent levelBossDeathEvent)
    {
        NextLevel();
    }

    private void NextLevel()
    {
        GlobalEventBus.Invoke(new LevelEndEvent());

        Destroy(levelInstance);

        currentLevel = currentLevel.GetNextLevel();
        levelInstance = Instantiate(currentLevel.Prefab, Vector3.zero, Quaternion.identity);

        GlobalEventBus.Invoke(new LevelStartEvent());
    }

    private class LevelEndEvent : IGlobalEvent { }
    private class LevelStartEvent : IGlobalEvent { }
}
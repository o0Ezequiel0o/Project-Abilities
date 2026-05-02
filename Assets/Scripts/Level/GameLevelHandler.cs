using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameLevelHandler : MonoBehaviour
{
    [SerializeField] private float difficultyPerLevel = 3f;
    [SerializeField] private List<GameLevel> randomStartLevel;

    private GameLevel currentLevel;
    private GameObject levelInstance;

    private int levelEndFrame = -1;

    private void Awake()
    {
        GameInstance.Level = 0;
        GlobalEventBus.Subscribe<LevelEndEvent>(OnLevelEnd);

        GameLevel startLevel = randomStartLevel[Random.Range(0, randomStartLevel.Count)];
        LoadLevel(startLevel);
    }

    private void OnLevelEnd(LevelEndEvent onLevelEnd)
    {
        if (levelEndFrame == Time.frameCount) return;

        levelEndFrame = Time.frameCount;

        GameLevel nextLevel = currentLevel.GetNextLevel();

        UnloadLevel(levelInstance);
        LoadLevel(nextLevel);
    }

    private void UnloadLevel(GameObject level)
    {
        DestroyLevelSpawnables();
        Destroy(level);
        ClearMinions();
    }

    private void LoadLevel(GameLevel level)
    {
        GameInstance.Difficulty += difficultyPerLevel * GameInstance.Level;
        levelInstance = Instantiate(level.Prefab, Vector3.zero, Quaternion.identity);

        GameInstance.Level += 1;
        currentLevel = level;

        GlobalEventBus.Invoke(new LevelStartEvent());
    }

    private void ClearMinions()
    {
        EntityTypeIdentifier[] entities = FindObjectsByType<EntityTypeIdentifier>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i].Type == EntityType.Minion)
            {
                Destroy(entities[i].gameObject);
            }
        }
    }

    private void DestroyLevelSpawnables()
    {
        SpawnableSpawner[] spawners = levelInstance.GetComponentsInChildren<SpawnableSpawner>();

        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].DestroySpawnedSpawnables();
        }
    }

    public class LevelEndEvent : IGlobalEvent
    {
        //forced level parameter
    }

    public class LevelStartEvent : IGlobalEvent { }
}
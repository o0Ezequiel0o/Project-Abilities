using System.Collections.Generic;
using UnityEngine;

public class GameLevelHandler : MonoBehaviour
{
    [SerializeField] private float difficultyPerLevel = 3f;
    [SerializeField] private List<GameLevel> randomStartLevel;

    private GameLevel currentLevel;
    private GameObject levelInstance;

    private int loadLevelFrame = -1;

    private void Awake()
    {
        GameInstance.Level = 0;

        GlobalEventBus.Subscribe<LoadLevelEvent>(@event => LoadNewLevel(@event.Level));
        GlobalEventBus.Subscribe<LoadNextLevelEvent>(_ => LoadNewLevel(currentLevel.GetNextLevel()));

        GameLevel startLevel = randomStartLevel[Random.Range(0, randomStartLevel.Count)];
        LoadLevel(startLevel);
    }

    private void LoadNewLevel(GameLevel newLevel)
    {
        if (loadLevelFrame == Time.frameCount) return;

        loadLevelFrame = Time.frameCount;
        UnloadLevel(levelInstance);
        LoadLevel(newLevel);
    }

    private void UnloadLevel(GameObject level)
    {
        DestroyLevelSpawnables();
        Destroy(level);
        ClearMinions();

        GlobalEventBus.Invoke(new LevelUnloadedEvent());
    }

    private void LoadLevel(GameLevel level)
    {
        GameInstance.Difficulty += difficultyPerLevel * GameInstance.Level;
        levelInstance = Instantiate(level.Prefab, Vector3.zero, Quaternion.identity);

        GameInstance.Level += 1;
        currentLevel = level;

        GlobalEventBus.Invoke(new LevelLoadedEvent());
        GlobalEventBus.Invoke(new LevelStartEvent());
    }

    private void ClearMinions()
    {
        EntityTypeIdentifier[] entities = FindObjectsByType<EntityTypeIdentifier>(FindObjectsInactive.Include);

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

    public struct LoadNextLevelEvent : IGlobalEvent { }

    public struct LoadLevelEvent : IGlobalEvent
    {
        public GameLevel Level { get; private set; }

        public LoadLevelEvent(GameLevel level)
        {
            Level = level;
        }
    }

    public struct LevelEndEvent : IGlobalEvent { }

    public struct LevelStartEvent : IGlobalEvent { }

    public struct LevelLoadedEvent : IGlobalEvent { }

    public struct LevelUnloadedEvent : IGlobalEvent { }
}
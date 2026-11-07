using System.Collections.Generic;
using UnityEngine;
using Zeke.GlobalEventsBus;

public class RunLevelHandler : GameService<RunLevelHandler>
{
    [SerializeField] private List<GameLevel> randomStartLevel;

    public GameLevel CurrentLevel { get; private set; }
    public GameLevelInstance CurrentLevelInstance { get; private set; }

    private void Awake()
    {
        GlobalEventBus.Subscribe<LoadLevelEvent>(OnLoadLevelEvent);
        GlobalEventBus.Subscribe<LoadNextLevelEvent>(LoadNextLevel);
    }

    private void Start()
    {
        GameLevel startLevel = randomStartLevel[Random.Range(0, randomStartLevel.Count)];
        LoadLevel(startLevel);
    }

    private void OnDestroy()
    {
        GlobalEventBus.Unsubscribe<LoadLevelEvent>(OnLoadLevelEvent);
        GlobalEventBus.Unsubscribe<LoadNextLevelEvent>(LoadNextLevel);

        CurrentLevel = null;
        CurrentLevelInstance = null;
    }

    private void OnLoadLevelEvent(LoadLevelEvent @event)
    {
        LoadNewLevel(@event.Level);
    }

    private void LoadNextLevel(LoadNextLevelEvent @event)
    {
        LoadNewLevel(CurrentLevel.GetNextLevel());
    }

    private void LoadNewLevel(GameLevel newLevel)
    {
        UnloadLevel(CurrentLevelInstance);
        LoadLevel(newLevel);
    }

    private void UnloadLevel(GameLevelInstance level)
    {
        if (level == null) return;

        DestroyLevelSpawnables();
        Destroy(level.gameObject);
        ClearMinions();

        GlobalEventBus.Invoke(new LevelUnloadedEvent(level));
    }

    private void LoadLevel(GameLevel level)
    {
        CurrentLevelInstance = Instantiate(level.Prefab, Vector3.zero, Quaternion.identity);
        CurrentLevel = level;

        GlobalEventBus.Invoke(new LevelLoadedEvent(CurrentLevelInstance));
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
        SpawnableSpawner[] spawners = CurrentLevelInstance.GetComponentsInChildren<SpawnableSpawner>();

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

    public readonly struct LevelEndEvent : IGlobalEvent { }

    public readonly struct LevelStartEvent : IGlobalEvent { }

    public readonly struct LevelLoadedEvent : IGlobalEvent
    {
        public readonly GameLevelInstance level;

        public LevelLoadedEvent(GameLevelInstance level)
        {
            this.level = level;
        }
    }

    public readonly struct LevelUnloadedEvent : IGlobalEvent
    {
        public readonly GameLevelInstance level;

        public LevelUnloadedEvent(GameLevelInstance level)
        {
            this.level = level;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zeke.GlobalEventsBus;

public class RunLevelHandler : GameService<RunLevelHandler>
{
    public GameLevel CurrentLevel { get; private set; }
    public GameLevelInstance CurrentLevelInstance { get; private set; }

    private readonly Dictionary<string, GameLevel> gameLevels = new Dictionary<string, GameLevel>();
    private readonly List<string> startLevels = new List<string>();

    private readonly List<GameObject> entitiesInLevel = new List<GameObject>(1024);
    private readonly List<GameObject> entitiesInLevelCopy = new List<GameObject>(1024);

    public void AddLevel(GameLevel level)
    {
        if (level.StartLevel) startLevels.Add(level.ID);
        gameLevels.Add(level.ID, level);
    }

    public void AddLevels(List<GameLevel> levels)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            AddLevel(levels[i]);
        }
    }

    public GameLevel GetLevel(string id)
    {
        return gameLevels.GetValueOrDefault(id);
    }

    private void Awake()
    {
        GlobalEventBus.Subscribe<SpawnableSpawnedEvent>(OnSpawnableSpawned);
        GlobalEventBus.Subscribe<SpawnableDestroyEvent>(OnSpawnableDestroyed);

        GlobalEventBus.Subscribe<LoadLevelEvent>(OnLoadLevelEvent);
        GlobalEventBus.Subscribe<LoadNextLevelEvent>(LoadNextLevel);
    }

    private void Start()
    {
        if (startLevels.Count == 0) Debug.LogWarning("No start levels loaded.");
        else
        {
            string randomStartLevelID = startLevels[Random.Range(0, startLevels.Count)];
            GameLevel startLevel = gameLevels[randomStartLevelID];
            LoadLevel(startLevel);
        }
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
        LoadNewLevel(@event.Level.ID);
    }

    private void LoadNextLevel(LoadNextLevelEvent @event)
    {
        LoadNewLevel(CurrentLevel.GetNextLevel());
    }

    private void LoadNewLevel(string levelID)
    {
        if (!gameLevels.ContainsKey(levelID))
        {
            Debug.LogWarning($"{levelID} is not registered in {nameof(gameLevels)}. (loaded from {CurrentLevel})");
        }
        else
        {
            UnloadLevel(CurrentLevelInstance);
            LoadLevel(gameLevels[levelID]);
        }
    }

    private void UnloadLevel(GameLevelInstance level)
    {
        if (level == null) return;

        DestroyNonPlayerSpawnables();
        Destroy(level.gameObject);

        GlobalEventBus.Invoke(new LevelUnloadedEvent(level));
    }

    private void LoadLevel(GameLevel level)
    {
        CurrentLevelInstance = level.CreateInstance();
        CurrentLevel = level;

        GlobalEventBus.Invoke(new LevelLoadedEvent(CurrentLevelInstance, CurrentLevel));
        GlobalEventBus.Invoke(new LevelStartEvent());
    }

    private void DestroyNonPlayerSpawnables()
    {
        entitiesInLevelCopy.AddRange(entitiesInLevel);

        for (int i = 0; i < entitiesInLevelCopy.Count; i++)
        {
            if (!entitiesInLevelCopy[i].TryGetComponent(out Player _))
            {
                Destroy(entitiesInLevelCopy[i]);
            }  
        }

        entitiesInLevelCopy.Clear();

        for (int i = 0; i < entitiesInLevel.Count; i++)
        {
            if (!entitiesInLevel[i].TryGetComponent(out Player _))
            {
                Debug.LogWarning($"{entitiesInLevel[i].name} failed to be cleared after level end.", entitiesInLevel[i]);
            }
        }
    }

    private void OnSpawnableSpawned(SpawnableSpawnedEvent @event)
    {
        entitiesInLevel.Add(@event.spawnable);
    }

    private void OnSpawnableDestroyed(SpawnableDestroyEvent @event)
    {
        entitiesInLevel.Remove(@event.spawnable);
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
        public readonly GameLevelInstance levelInstance;
        public readonly GameLevel gameLevel;

        public LevelLoadedEvent(GameLevelInstance levelInstance, GameLevel gameLevel)
        {
            this.levelInstance = levelInstance;
            this.gameLevel = gameLevel;
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
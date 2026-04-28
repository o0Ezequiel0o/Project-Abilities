using System.Collections.Generic;
using UnityEngine;
using Zeke.Collections;

public class SpawnableSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float startingPoints = 1f;

    [Header("Points per Second")]
    [SerializeField] private float basePointGeneration = 1f;
    [SerializeField] private float pointGenerationDifficultyRatio = 0f;

    [Header("Spawning")]
    [SerializeField] private int maxAliveSpawnables = 1000;
    [SerializeField] private List<Spawnpoint> spawnpoints;
    [SerializeField] private List<Spawnable> spawnables;

    public static OrderedAction<GameObject> onSpawnableSpawned = new OrderedAction<GameObject>();

    private readonly Dictionary<CachedSpawnableData, Spawnable> cachedSpawnablesRef = new Dictionary<CachedSpawnableData, Spawnable>();
    private readonly List<CachedSpawnableData> cachedSpawnables = new List<CachedSpawnableData>();
    private readonly List<GameObject> aliveSpawnables = new List<GameObject>();

    private readonly List<Spawnpoint> tempSpawnpoints = new List<Spawnpoint>();

    private float currentPoints = 0;

    private void Awake()
    {
        FilterSpawnablesByDifficulty();
    }

    private void Start()
    {
        currentPoints = startingPoints;
    }

    private void Update()
    {
        UpdatePoints();
        UpdateSpawning();
    }

    private void UpdatePoints()
    {
        float points = (basePointGeneration + pointGenerationDifficultyRatio * GameInstance.Difficulty) * Time.deltaTime;
        currentPoints += points;
    }

    private void UpdateSpawning()
    {
        if (aliveSpawnables.Count >= maxAliveSpawnables) return;

        float debt = Mathf.Max(currentPoints * -1f, 0f);

        if (debt > 0f) return;

        FilterSpawnablesByDifficulty();

        if (cachedSpawnables.Count <= 0)
        {
            if (spawnables.Count > 0)
            {
                Debug.LogWarning("No valid spawnables");
            }

            return;
        }

        SpawnSpawnable(WeightedSelect.SelectElement(cachedSpawnables));
    }

    private void SpawnSpawnable(CachedSpawnableData cachedSpawnable)
    {
        tempSpawnpoints.Clear();
        tempSpawnpoints.AddRange(cachedSpawnable.spawnpoints);

        Spawnpoint spawnPoint = null;

        while (tempSpawnpoints.Count > 0)
        {
            spawnPoint = WeightedSelect.SelectElement(tempSpawnpoints);

            if (!spawnPoint.IsBlocked())
            {
                break;
            }
            else
            {
                tempSpawnpoints.Remove(spawnPoint);
                spawnPoint = null;
            }
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("All spawnpoints are blocked by enemy entities, consider adding more");
            return;
        }

        GameObject spawnableInstance = spawnPoint.Spawn(cachedSpawnable.prefab);

        if (!spawnableInstance.TryGetComponent(out DestroyEventTracker destroyEventTracker))
        {
            destroyEventTracker = spawnableInstance.AddComponent<DestroyEventTracker>();
        }

        destroyEventTracker.onDestroy += OnSpawnableDeath;
        aliveSpawnables.Add(spawnableInstance);
        currentPoints -= cachedSpawnable.cost;

        onSpawnableSpawned?.Invoke(spawnableInstance);
    }

    private void OnSpawnableDeath(GameObject spawnableInstance)
    {
        aliveSpawnables.Remove(spawnableInstance);
    }

    private void FilterSpawnablesByDifficulty()
    {
        for (int i = cachedSpawnables.Count - 1; i >= 0; i--)
        {
            if (GameInstance.Difficulty > cachedSpawnables[i].maxDifficulty)
            {
                RemoveCachedSpawnable(cachedSpawnables[i]);
            }
        }

        int oldIndex = spawnables.Count;

        for (int i = 0; i < oldIndex; i++)
        {
            if (GameInstance.Difficulty >= spawnables[i].MinDifficulty && GameInstance.Difficulty < spawnables[i].MaxDifficulty)
            {
                if (!cachedSpawnablesRef.ContainsValue(spawnables[i]))
                {
                    CreateCachedSpawnable(spawnables[i]);
                }
            }
        }
    }

    private void CreateCachedSpawnable(Spawnable spawnable)
    {
        List<Spawnpoint> avaibleSpawnpoints = new List<Spawnpoint>();

        for (int i = 0; i < spawnpoints.Count; i++)
        {
            if (spawnpoints[i].Includes(spawnable))
            {
                avaibleSpawnpoints.Add(spawnpoints[i]);
            }
        }

        if (avaibleSpawnpoints.Count <= 0)
        {
            Debug.LogWarning($"{spawnable} does not have any valid spawnpoints", spawnable);
        }
        else
        {
            CachedSpawnableData cachedSpawnable = new CachedSpawnableData(spawnable, avaibleSpawnpoints);
            cachedSpawnablesRef.Add(cachedSpawnable, spawnable);
            cachedSpawnables.Add(cachedSpawnable);
        }
    }

    private void RemoveCachedSpawnable(CachedSpawnableData cachedSpawnable)
    {
        cachedSpawnablesRef.Remove(cachedSpawnable);
        cachedSpawnables.Remove(cachedSpawnable);
    }

    private class CachedSpawnableData : IWeighted
    {
        public readonly GameObject prefab;
        public readonly Spawnable spawnableRef;
        public readonly List<Spawnpoint> spawnpoints;

        public int Weight => weight;

        public readonly float minDifficulty;
        public readonly float maxDifficulty;

        public readonly int weight;
        public readonly int cost;

        private int maxSpawnRollWeight;

        public CachedSpawnableData(Spawnable waveSpawnable, List<Spawnpoint> spawnpoints)
        {
            prefab = waveSpawnable.Prefab;
            spawnableRef = waveSpawnable;

            minDifficulty = waveSpawnable.MinDifficulty;
            maxDifficulty = waveSpawnable.MaxDifficulty;

            weight = waveSpawnable.Weight;
            cost = waveSpawnable.Cost;

            this.spawnpoints = spawnpoints;
            GetMaxSpawnpointRollWeight();
        }

        public Spawnpoint GetRandomSpawnpoint()
        {
            int randomWeight = Random.Range(0, maxSpawnRollWeight);

            for (int i = 0; i < spawnpoints.Count; i++)
            {
                if (spawnpoints[i].Weight > randomWeight)
                {
                    return spawnpoints[i];
                }
                else
                {
                    randomWeight -= spawnpoints[i].Weight;
                }
            }

            return null;
        }

        private void GetMaxSpawnpointRollWeight()
        {
            maxSpawnRollWeight = 0;

            for (int i = 0; i < spawnpoints.Count; i++)
            {
                maxSpawnRollWeight += spawnpoints[i].Weight;
            }
        }
    }
}
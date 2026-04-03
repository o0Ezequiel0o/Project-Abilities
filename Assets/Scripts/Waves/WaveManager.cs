using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [field: Header("Settings")]
    [field: SerializeField] public float IntermissionLength { get; private set; }
    [field: SerializeField] private int startPoints;

    [Header("Spawning")]
    [SerializeField] public List<Spawnpoint> spawnpoints;
    [SerializeField] public List<WaveUpdate> waveUpdates;

    public WaveUpdate LoadedWaveUpdate { get; private set; }

    public int ActiveSpawnables { get; private set; } = 0;
    public int SpawnablesAlive { get; private set; } = 0;

    public int CurrentWave { get; private set; } = 0;
    public int MaxPoints { get; private set; } = 0;

    private WaveStateMachine stateMachine;
    private WaveStateContext context;

    public int UnactiveSpawnables => unactiveInstantiatedSpawnables.Count;
    public int SpawnablesLeftToSpawn => spawnableSpawnRequests.Count;

    private readonly Stack<SpawnableSpawnRequest> spawnableSpawnRequests = new Stack<SpawnableSpawnRequest>();
    private readonly Stack<GameObject> unactiveInstantiatedSpawnables = new Stack<GameObject>();

    private readonly List<FilteredSpawnableData> filteredSpawnablePool = new List<FilteredSpawnableData>();
    private readonly List<WaveSpawnable> currentSpawnablePool = new List<WaveSpawnable>();

    private int maxRollWeight;
    private int points;

    public void GenerateWave()
    {
        while (filteredSpawnablePool.Count > 0)
        {
            FilteredSpawnableData waveSpawnable = RollSpawnable();
            Spawnpoint spawnpoint = waveSpawnable?.GetRandomSpawnpoint();

            if (waveSpawnable != null)
            {
                spawnableSpawnRequests.Push(new SpawnableSpawnRequest(waveSpawnable.prefab, spawnpoint));
                points -= waveSpawnable.cost;
            }
        }
    }

    public void InstantiateUnactiveNextSpawnableInQueue()
    {
        if (spawnableSpawnRequests.Count == 0) return;

        SpawnableSpawnRequest spawnRequest = spawnableSpawnRequests.Pop();
        GameObject spawnableInstance = spawnRequest.Spawn();

        if (spawnableInstance.TryGetComponent(out Damageable damageable))
        {
            damageable.onDeath += ReduceSpawnablesAliveCounter;
        }

        unactiveInstantiatedSpawnables.Push(spawnableInstance);
        spawnableInstance.SetActive(false);
        SpawnablesAlive += 1;
    }

    public void ActivateNextUnactiveInstantiatedSpawnable()
    {
        if (unactiveInstantiatedSpawnables.Count > 0)
        {
            unactiveInstantiatedSpawnables.Pop().SetActive(true);
            ActiveSpawnables += 1;
        }
    }

    public void LoadNextWave()
    {
        CurrentWave += 1;

        if (LoadedWaveUpdate != null)
        {
            MaxPoints += LoadedWaveUpdate.PointsPerWave;
        }

        if (TryGetNextWaveUpdate(out WaveUpdate waveUpdate) && waveUpdate.Wave == CurrentWave)
        {
            LoadNextWaveData();
            UpdateCurrentSpawnablePool();
        }

        GenerateFilteredSpawnablePool();
        RecalculateMaxSpawnRollWeight();

        points = MaxPoints;
    }

    private void Awake()
    {
        MaxPoints = startPoints;
        OrderWaveUpdates();
    }

    private void Start()
    {
        InitializeStateMachine();
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void OnDestroy()
    {
        stateMachine.Destroy();
    }

    private FilteredSpawnableData RollSpawnable()
    {
        int randomWeight = Random.Range(0, maxRollWeight);

        for (int i = 0; i < filteredSpawnablePool.Count; i++)
        {
            if (filteredSpawnablePool[i].weight <= randomWeight)
            {
                randomWeight -= filteredSpawnablePool[i].weight;
                continue;
            }

            if (filteredSpawnablePool[i].cost <= points)
            {
                return filteredSpawnablePool[i];
            }
            else
            {
                filteredSpawnablePool.RemoveAt(i);
                RecalculateMaxSpawnRollWeight();

                return RollSpawnable();
            }
        }

        return null;
    }

    private void ReduceSpawnablesAliveCounter(Damageable.DamageEvent _)
    {
        SpawnablesAlive -= 1;
        ActiveSpawnables -= 1;
    }

    private void InitializeStateMachine()
    {
        context = new WaveStateContext(this);
        stateMachine = new WaveStateMachine(context);

        stateMachine.ChangeState(stateMachine.loadState);
    }

    private void OrderWaveUpdates()
    {
        waveUpdates.OrderByDescending(wave => wave.Wave);
    }

    private void GenerateFilteredSpawnablePool()
    {
        filteredSpawnablePool.Clear();

        List<Spawnpoint> possibleSpawnpoints;

        for (int i = 0; i < currentSpawnablePool.Count; i++)
        {
            possibleSpawnpoints = GetPossibleSpawnpoints(currentSpawnablePool[i]);

            if (currentSpawnablePool[i].Cost > MaxPoints)
            {
                continue;
            }

            if (possibleSpawnpoints.Count == 0)
            {
                continue;
            }

            filteredSpawnablePool.Add(new FilteredSpawnableData(currentSpawnablePool[i], possibleSpawnpoints));
        }
    }

    private bool TryGetNextWaveUpdate(out WaveUpdate waveUpdate)
    {
        waveUpdate = null;

        if (waveUpdates.Count != 0 && waveUpdates[^1] != null)
        {
            waveUpdate = waveUpdates[^1];
            return true;
        }

        return false;
    }

    private void LoadNextWaveData()
    {
        LoadedWaveUpdate = waveUpdates[^1];
        waveUpdates.RemoveAt(waveUpdates.Count - 1);
    }

    private void UpdateCurrentSpawnablePool()
    {
        for (int i = 0; i < LoadedWaveUpdate.RemoveFromPool.Count; i++)
        {
            currentSpawnablePool.Remove(LoadedWaveUpdate.RemoveFromPool[i]);
        }

        for (int i = 0; i < LoadedWaveUpdate.AddToPool.Count; i++)
        {
            if (!currentSpawnablePool.Contains(LoadedWaveUpdate.AddToPool[i]))
            {
                currentSpawnablePool.Add(LoadedWaveUpdate.AddToPool[i]);
            }
        }
    }

    private void RecalculateMaxSpawnRollWeight()
    {
        maxRollWeight = 0;

        for (int i = 0; i < filteredSpawnablePool.Count; i++)
        {
            maxRollWeight += filteredSpawnablePool[i].weight;
        }
    }

    private List<Spawnpoint> GetPossibleSpawnpoints(WaveSpawnable waveSpawnable)
    {
        List<Spawnpoint> possibleSpawnpoints = new List<Spawnpoint>();

        for (int i = 0; i < spawnpoints.Count; i++)
        {
            if (spawnpoints[i].Includes(waveSpawnable))
            {
                possibleSpawnpoints.Add(spawnpoints[i]);
            }
        }

        return possibleSpawnpoints;
    }

    private class FilteredSpawnableData
    {
        public readonly GameObject prefab;
        public readonly List<Spawnpoint> spawnpoints;

        public readonly int weight;
        public readonly int cost;

        private int maxSpawnRollWeight;

        public FilteredSpawnableData(WaveSpawnable waveSpawnable, List<Spawnpoint> spawnpoints)
        {
            prefab = waveSpawnable.Prefab;
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

        void GetMaxSpawnpointRollWeight()
        {
            maxSpawnRollWeight = 0;

            for (int i = 0; i < spawnpoints.Count; i++)
            {
                maxSpawnRollWeight += spawnpoints[i].Weight;
            }
        }
    }

    private readonly struct SpawnableSpawnRequest
    {
        private readonly GameObject prefab;
        private readonly Spawnpoint spawnpoint;

        public SpawnableSpawnRequest(GameObject prefab, Spawnpoint spawnpoint)
        {
            this.prefab = prefab;
            this.spawnpoint = spawnpoint;
        }

        public GameObject Spawn()
        {
            return spawnpoint.Spawn(prefab);
        }
    }
}

public class WaveStateMachine : StateMachine<WaveStateContext>
{
    public readonly WaveSpawnState spawnState;
    public readonly WaveLoadState loadState;
    public readonly WaveWaitState waitState;

    public readonly WaveStateContext context;

    public WaveStateMachine(WaveStateContext context)
    {
        this.context = context;

        spawnState = new WaveSpawnState(this);
        loadState = new WaveLoadState(this);
        waitState = new WaveWaitState(this);
    }

    public override void ChangeState(State<WaveStateContext> newState)
    {
        currentState?.EnterState(context);
        currentState = newState;
        currentState?.ExitState(context);
    }

    public override void Update()
    {
        currentState?.UpdateState(context);
    }

    public override void LateUpdate()
    {
        currentState?.LateUpdateState(context);
    }

    public override void Destroy()
    {
        spawnState.DestroyState(context);
        loadState.DestroyState(context);
        waitState.DestroyState(context);
    }
}

public class WaveStateContext
{
    public readonly WaveManager manager;

    public WaveStateContext(WaveManager manager)
    {
        this.manager = manager;
    }
}
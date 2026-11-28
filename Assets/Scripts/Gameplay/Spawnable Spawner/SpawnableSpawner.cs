using System.Collections.Generic;
using UnityEngine;
using Zeke.GlobalEventsBus;

public class SpawnableSpawner : MonoBehaviour
{
    [SerializeField] private List<Spawnable> spawnables;
    [SerializeField] private Spawnpoint spawnpoint;

    [Space]

    [SerializeField] private bool ignoreBlocked;

    private void Start()
    {
        for (int i = 0; i < spawnables.Count; i++)
        {
            SpawnSpawnable(spawnables[i]);
        }
    }

    private void SpawnSpawnable(Spawnable spawnable)
    {
        if (spawnpoint == null)
        {
            Debug.LogWarning("There are no avaible spawnpoints");
        }
        else
        {
            spawnpoint.ProcessBlockedState((blocked) => OnSpawnPointProcessed(spawnable, spawnpoint, blocked));
        }
    }

    private void OnSpawnPointProcessed(Spawnable spawnable, Spawnpoint spawnpoint, bool blocked)
    {
        if (blocked && !ignoreBlocked)
        {
            Debug.LogWarning("Spawnpoint is blocked");
        }
        else
        {
            SpawnSpawnable(spawnable, spawnpoint);
        }
    }

    private void SpawnSpawnable(Spawnable spawnable, Spawnpoint spawnpoint)
    {
        GameObject spawnableInstance = spawnpoint.Spawn(spawnable.Prefab);

        if (!spawnableInstance.TryGetComponent(out DestroyEventTracker destroyEventTracker))
        {
            destroyEventTracker = spawnableInstance.AddComponent<DestroyEventTracker>();
        }

        destroyEventTracker.onDestroy += (_) => GlobalEventBus.Invoke(new SpawnableDestroyEvent(spawnableInstance));
        GlobalEventBus.Invoke(new SpawnableSpawnedEvent(spawnableInstance));
    }
}
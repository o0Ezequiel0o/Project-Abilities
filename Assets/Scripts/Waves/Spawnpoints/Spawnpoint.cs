using System.Collections.Generic;
using UnityEngine;

public abstract class Spawnpoint : MonoBehaviour
{
    [Header("Filter")]
    [SerializeField] private List<WaveSpawnable> exclude;
    [SerializeField] private List<WaveSpawnable> include;
    [SerializeField] private bool fillInclude = true;
    [field: Header("Settings")]
    [field: SerializeField] public int Weight = 100;

    public abstract GameObject Spawn(GameObject prefab);

    public bool Includes(WaveSpawnable waveSpawnable)
    {
        return fillInclude && !exclude.Contains(waveSpawnable) || !fillInclude && include.Contains(waveSpawnable);
    }

    public bool Excludes(WaveSpawnable waveSpawnable)
    {
        return !Includes(waveSpawnable);
    }
}
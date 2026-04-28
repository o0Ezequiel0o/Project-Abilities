using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public abstract class Spawnpoint : MonoBehaviour, IWeighted
{
    [Header("Filter")]
    [SerializeField] private List<Spawnable> exclude;
    [SerializeField] private List<Spawnable> include;
    [SerializeField] private bool fillInclude = true;

    [field: Header("Settings")]
    [field: SerializeField] public int Weight { get; private set; } = 100;

    [Header("Spawning")]
    [SerializeField] protected Teams team;
    [SerializeField] private LayerMask blockLayers;

    protected readonly List<Collider2D> hits = new List<Collider2D>();

    public abstract GameObject Spawn(GameObject prefab);

    public bool IsBlocked()
    {
        hits.Clear();
        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = blockLayers, useLayerMask = true };
        return IsBlocked(contactFilter);
    }

    protected abstract bool IsBlocked(ContactFilter2D contactFIlter);

    public bool Includes(Spawnable waveSpawnable)
    {
        return fillInclude && !exclude.Contains(waveSpawnable) || !fillInclude && include.Contains(waveSpawnable);
    }

    public bool Excludes(Spawnable waveSpawnable)
    {
        return !Includes(waveSpawnable);
    }
}
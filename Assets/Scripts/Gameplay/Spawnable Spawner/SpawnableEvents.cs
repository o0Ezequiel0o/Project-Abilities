using UnityEngine;
using Zeke.GlobalEventsBus;

public readonly struct SpawnableSpawnedEvent : IGlobalEvent
{
    public readonly GameObject spawnable;

    public SpawnableSpawnedEvent(GameObject spawnable)
    {
        this.spawnable = spawnable;
    }
}

public readonly struct SpawnableDestroyEvent : IGlobalEvent
{
    public readonly GameObject spawnable;

    public SpawnableDestroyEvent(GameObject spawnable)
    {
        this.spawnable = spawnable;
    }
}
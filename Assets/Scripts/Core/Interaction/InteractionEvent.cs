using UnityEngine;
using Zeke.GlobalEventsBus;

public readonly struct InteractionEvent : IGlobalEvent
{
    public readonly GameObject gameObject;
    public readonly bool success;

    public InteractionEvent(GameObject gameObject, bool success)
    {
        this.gameObject = gameObject;
        this.success = success;
    }
}
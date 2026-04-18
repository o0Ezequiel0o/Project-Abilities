using UnityEngine;

public readonly struct InteractionResult
{
    public readonly GameObject gameObject;
    public readonly bool success;

    public InteractionResult(GameObject gameObject, bool success)
    {
        this.gameObject = gameObject;
        this.success = success;
    }
}
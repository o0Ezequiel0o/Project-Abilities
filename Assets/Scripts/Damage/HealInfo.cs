using UnityEngine;

public struct HealInfo
{
    public float baseHealing;
    public float procCoefficient;

    public bool hit;

    public Vector3 direction;

    public HealInfo(Damageable.HealEvent healEvent)
    {
        baseHealing = healEvent.BaseHealing;
        procCoefficient = healEvent.ProcCoefficient;

        hit = healEvent.IsHit;

        direction = healEvent.Direction;
    }

    public HealInfo(float baseHealing, float procCoefficient)
    {
        this.baseHealing = baseHealing;
        this.procCoefficient = procCoefficient;

        hit = true;

        direction = Vector3.zero;
    }
}
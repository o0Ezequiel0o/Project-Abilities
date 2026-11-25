using UnityEngine;

[CreateAssetMenu(fileName = "New Health Type", menuName = "New Health Type", order = 0)]
public abstract class HealthTypeData : ScriptableObject
{
    [field: Header("Visual")]
    [field: SerializeField] public Color Color { get; private set; }
    [field: SerializeField] public int DamagePriority { get; private set; }

    [field: Header("Settings")]
    [field: SerializeField] public float RegenInterval { get; private set; }
    [field: SerializeField] public float StartRatio { get; private set; } = 1f;

    public abstract HealthType CreateHealthType(Stat max, Stat regen, Stat receivedMultiplier);

    public abstract float UpdateRegeneration(HealthType health, Damageable damageable, float regenTimer);
}
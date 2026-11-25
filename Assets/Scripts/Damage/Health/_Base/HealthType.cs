using System;
using UnityEngine;
using static Damageable;

[Serializable]
public class HealthType
{
    [field: SerializeField] public HealthTypeData Type { get; private set; }
    [field: SerializeField] public Stat Max { get; private set; } = new Stat(100f, 0f, 0f, float.PositiveInfinity);
    [field: SerializeField] public Stat Regen { get; private set; } = new Stat(100f, 0f, float.NegativeInfinity, float.PositiveInfinity);
    [field: SerializeField] public Stat ReceivedMultiplier { get; private set; } = new Stat(1f, 0f, 0f, float.PositiveInfinity);

    public float Value
    {
        get => value;
        set => this.value = Mathf.Clamp(value, 0f, Max.Value);
    }

    public int Priority => Type.DamagePriority;

    private float value = 0f;
    private Damageable source;

    private float regenTimer = 0f;

    public HealthType() { }

    public float CalculateHealing(float amount)
    {
        return amount * ReceivedMultiplier.Value;
    }

    public float CalculateHealing(HealEvent healingEvent)
    {
        float healing = Mathf.Max(0f, (healingEvent.Healing + healingEvent.FlatAccumulator) * healingEvent.Multiplier.Value);
        return CalculateHealing(healing);
    }

    public HealthType Initialize(HealthTypeData healthType, Stat max, Stat regen, Stat receivedMultiplier)
    {
        Type = healthType;

        Max = max;
        Regen = regen;
        ReceivedMultiplier = receivedMultiplier;

        return this;
    }

    public void SetSource(Damageable source)
    {
        this.source = source;

        value = Max.Value * Type.StartRatio;

        Max.onValueChanged += SyncHealth;
        Max.onNonDerivedValueChanged += SyncHealth;
    }

    public void Update()
    {
        regenTimer += Time.deltaTime;

        if (regenTimer >= Type.RegenInterval)
        {
            Value = Type.UpdateRegeneration(this, source, regenTimer);
            regenTimer = 0f;
        }
    }

    public void Upgrade(int levels)
    {
        Max.Upgrade(levels);
        Regen.Upgrade(levels);
        ReceivedMultiplier.Upgrade(levels);
    }

    private void SyncHealth(Stat.StatUpdate statUpdate)
    {
        float healthRatio = Mathf.Clamp01(Value / statUpdate.oldValue);
        float change = Max.Value - statUpdate.oldValue;

        if (change == 0f) return;

        if (change > 0f)
        {
            Value += change * healthRatio;
        }
        else if (change < 0f && value > Max.Value)
        {
            value = Max.Value;
        }

        source.onAnyHealthUpdate?.Invoke();
    }
}
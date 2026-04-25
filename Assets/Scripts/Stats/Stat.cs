using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue = 1f;
    [SerializeField] private float increase = 0f;
    [SerializeField] private Limits valueLimits;

    public static Stat Zero => new(0f, 0f, 0f, 0f);

    private float flatModifier = 0f;
    private float multiplier = 1f;

    public float Value => Mathf.Clamp((baseValue + flatModifier) * multiplier, valueLimits.Min, valueLimits.Max);
    public int ValueInt => Mathf.FloorToInt(Value);

    public float ExtraValue => (Value - baseValue);

    public int Level { get; private set; }

    public Stat() { }

    private readonly List<Multiplier> multipliers = new List<Multiplier>();

    public Stat(float baseValue, float increase, float min, float max)
    {
        this.baseValue = baseValue;
        this.increase = increase;

        valueLimits = new Limits(min, max);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public Multiplier Multiply(float value)
    {
        Multiplier multiplier = new Multiplier(value);
        AddMultiplier(multiplier);
        return multiplier;
    }

    public void AddMultiplier(Multiplier statMultiplier)
    {
        if (!multipliers.Contains(statMultiplier))
        {
            multipliers.Add(statMultiplier);
            statMultiplier.LinkToStat(this);
            RecalculateMultiplierValue();
        }
    }

    public void RemoveMultiplier(Multiplier statMultiplier)
    {
        multipliers.Remove(statMultiplier);
        statMultiplier.UnlinkStat();
        RecalculateMultiplierValue();
    }

    public void ApplyFlatModifier(float increase)
    {
        flatModifier += increase;
    }

    public void Upgrade()
    {
        baseValue += increase;
        Level += 1;
    }

    private void RecalculateMultiplierValue()
    {
        multiplier = 1f;

        for (int i = 0; i < multipliers.Count; i++)
        {
            multiplier *= multipliers[i].Value;
        }
    }

    public Stat(Stat original)
    {
        baseValue = original.baseValue;
        increase = original.increase;

        valueLimits = original.valueLimits;
    }

    public Stat DeepCopy()
    {
        return new Stat(baseValue, increase, valueLimits.Min, valueLimits.Max);
    }

    public class Multiplier
    {
        public float Value { get; private set; } = 1f;
        private Stat linkedStat = null;

        public Multiplier() { }

        public Multiplier(float multiplier)
        {
            Value = multiplier;
        }

        public void LinkToStat(Stat stat)
        {
            linkedStat = stat;
        }

        public void UnlinkStat()
        {
            linkedStat = null;
        }

        public void UpdateMultiplier(float newMultiplierValue)
        {
            if (Value != newMultiplierValue && linkedStat != null)
            {
                Value = newMultiplierValue;
                linkedStat.RecalculateMultiplierValue();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat : IUpgradable
{
    [SerializeField] private float baseValue = 1f;
    [SerializeField] private float increase = 0f;
    [SerializeField] private Limits valueLimits;

    public static Stat Zero => new(0f, 0f, 0f, 0f);

    public float Value => Mathf.Clamp(((ScaledBaseValue + standardFlatModifier + derivedFlatModifier) * standardMultiplier) * derivedMultiplier, valueLimits.Min, valueLimits.Max);
    
    public float ValueWithoutDerived => Mathf.Clamp((ScaledBaseValue + standardFlatModifier) * standardMultiplier, valueLimits.Min, valueLimits.Max);

    public int ValueInt => Mathf.FloorToInt(Value);
    public float ExtraValue => (Value - ScaledBaseValue);

    public int Level { get; private set; } = 1;

    ///<summary>For use when you want to read the real value of a stat.</summary>
    public Action<StatUpdate> onValueChanged;

    ///<summary>For use when you update a Derived value to avoid circular dependency (like a stat scaling based on another). Does not calculate other derived values. </summary>
    public Action<StatUpdate> onNonDerivedValueChanged;

    private float standardFlatModifier = 0f;
    private float derivedFlatModifier = 0f;

    private float standardMultiplier = 1f;
    private float derivedMultiplier = 1f;

    private float ScaledBaseValue => baseValue + (increase * (Level - 1));

    private readonly List<Multiplier> standardMultipliers = new List<Multiplier>();
    private readonly List<Multiplier> derivedMultipliers = new List<Multiplier>();

    public Stat() { }

    public Stat(Stat original) : this(original.baseValue, original.increase, original.valueLimits.Min, original.valueLimits.Max) { }

    public Stat(float baseValue, float increase, float min, float max)
    {
        this.baseValue = baseValue;
        this.increase = increase;
        valueLimits = new Limits(min, max);
    }

    public void ApplyFlatModifier(params float[] increase)
    {
        float oldValue = Value;
        bool updated = false;

        for (int i = 0; i < increase.Length; i++)
        {
            standardFlatModifier += increase[i];

            if (increase[i] != 0f)
            {
                updated = true;
            }
        }

        if (updated)
        {
            onValueChanged?.Invoke(new StatUpdate(oldValue));
            onNonDerivedValueChanged?.Invoke(new StatUpdate(oldValue));
        }
    }

    public void ApplyDerivedFlatModifier(params float[] increase)
    {
        float oldValue = Value;
        bool updated = false;

        for (int i = 0; i < increase.Length; i++)
        {
            derivedFlatModifier += increase[i];

            if (increase[i] != 0f)
            {
                updated = true;
            }
        }

        if (updated)
        {
            onValueChanged?.Invoke(new StatUpdate(oldValue));
        }
    }

    public Multiplier Multiply(float value)
    {
        Multiplier multiplier = new Multiplier(value);
        AddMultiplier(multiplier);
        return multiplier;
    }

    public void AddMultiplier(Multiplier statMultiplier)
    {
        if (!standardMultipliers.Contains(statMultiplier))
        {
            standardMultipliers.Add(statMultiplier);
            statMultiplier.onValueChanged += RecalculateStandardMultiplierValue;

            RecalculateStandardMultiplierValue();
        }
    }

    public void RemoveMultiplier(Multiplier statMultiplier)
    {
        standardMultipliers.Remove(statMultiplier);
        statMultiplier.onValueChanged -= RecalculateStandardMultiplierValue;

        RecalculateStandardMultiplierValue();
    }

    public Multiplier MultiplyDerived(float value)
    {
        Multiplier multiplier = new Multiplier(value);
        AddDerivedMultiplier(multiplier);
        return multiplier;
    }

    public void AddDerivedMultiplier(Multiplier statMultiplier)
    {
        if (!derivedMultipliers.Contains(statMultiplier))
        {
            derivedMultipliers.Add(statMultiplier);
            statMultiplier.onValueChanged += RecalculateDerivedMultiplierValue;

            RecalculateDerivedMultiplierValue();
        }
    }

    public void RemoveDerivedMultiplier(Multiplier statMultiplier)
    {
        derivedMultipliers.Remove(statMultiplier);
        statMultiplier.onValueChanged -= RecalculateDerivedMultiplierValue;

        RecalculateDerivedMultiplierValue();
    }

    private void RecalculateStandardMultiplierValue()
    {
        float oldValue = Value;
        standardMultiplier = 1f;

        for (int i = 0; i < standardMultipliers.Count; i++)
        {
            standardMultiplier *= standardMultipliers[i].Value;
        }

        onValueChanged?.Invoke(new StatUpdate(oldValue));
        onNonDerivedValueChanged?.Invoke(new StatUpdate(oldValue));
    }

    private void RecalculateDerivedMultiplierValue()
    {
        float oldValue = Value;
        derivedMultiplier = 1f;

        for (int i = 0; i < derivedMultipliers.Count; i++)
        {
            derivedMultiplier *= derivedMultipliers[i].Value;
        }

        onValueChanged?.Invoke(new StatUpdate(oldValue));
    }

    public void Upgrade()
    {
        float oldValue = Value;
        Level += 1;

        if (increase != 0f)
        {
            onValueChanged?.Invoke(new StatUpdate(oldValue));
            onNonDerivedValueChanged?.Invoke(new StatUpdate(oldValue));
        }
    }

    public void Reset()
    {
        onValueChanged = null;
        onNonDerivedValueChanged = null;
        standardMultipliers.Clear();
        derivedMultipliers.Clear();
        standardFlatModifier = 0f;
        derivedFlatModifier = 0f;
        standardMultiplier = 1f;
        derivedMultiplier = 1f;
        Level = 1;
    }

    public Stat DeepCopy()
    {
        return new Stat(baseValue, increase, valueLimits.Min, valueLimits.Max);
    }

    public class Multiplier
    {
        public float Value { get; protected set; } = 1f;

        public Action onValueChanged;

        public Multiplier() : this(1f) { }

        public Multiplier(float multiplier)
        {
            Value = multiplier;
        }

        public virtual void UpdateMultiplier(float newMultiplierValue)
        {
            if (Value != newMultiplierValue)
            {
                Value = newMultiplierValue;
                onValueChanged?.Invoke();
            }
        }
    }

    public readonly struct StatUpdate
    {
        public readonly float oldValue;

        public StatUpdate(float oldValue)
        {
            this.oldValue = oldValue;
        }
    }
}
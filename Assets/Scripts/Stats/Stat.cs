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

    /// <summary> Internal gameplay events (excludes derived changes to prevent infinite calculation loops). </summary>
    public Action<StatUpdate> onStatUpdated;

    /// <summary> General notification event for UI, achievements, audio, etc. Fires whenever Value changes by ANY means. </summary>
    public Action<StatValueChange> onStatValueChanged;

    private float standardFlatModifier = 0f;
    private float derivedFlatModifier = 0f;

    private float standardMultiplier = 1f;
    private float derivedMultiplier = 1f;

    private float ScaledBaseValue => baseValue + (increase * (Level - 1));

    private readonly List<Multiplier> standardMultipliers = new List<Multiplier>();
    private readonly List<Multiplier> derivedMultipliers = new List<Multiplier>();

    public Stat() { }

    public Stat(float baseValue, float increase, float min, float max)
    {
        this.baseValue = baseValue;
        this.increase = increase;
        valueLimits = new Limits(min, max);
    }

    public void ApplyFlatModifier(params float[] increase)
    {
        float oldStandardValue = ValueWithoutDerived;
        float oldDisplayValue = Value;
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
            NotifyStatUpdated(oldStandardValue, oldDisplayValue);
        }
    }

    public void ApplyDerivedFlatModifier(params float[] increase)
    {
        float oldDisplayValue = Value;

        for (int i = 0; i < increase.Length; i++)
        {
            derivedFlatModifier += increase[i];
        }

        NotifyValueOnlyChanged(oldDisplayValue);
    }

    public void SetDerivedFlatModifier(float totalValue)
    {
        if (derivedFlatModifier == totalValue) return;

        float oldDisplayValue = Value;
        derivedFlatModifier = totalValue;

        NotifyValueOnlyChanged(oldDisplayValue);
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
            statMultiplier.LinkToStat(this, isDerived: false);
            RecalculateStandardMultiplierValue();
        }
    }

    public void RemoveMultiplier(Multiplier statMultiplier)
    {
        standardMultipliers.Remove(statMultiplier);
        statMultiplier.UnlinkStat();
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
            statMultiplier.LinkToStat(this, isDerived: true);
            RecalculateDerivedMultiplierValue();
        }
    }

    public void RemoveDerivedMultiplier(Multiplier statMultiplier)
    {
        derivedMultipliers.Remove(statMultiplier);
        statMultiplier.UnlinkStat();
        RecalculateDerivedMultiplierValue();
    }

    private void RecalculateStandardMultiplierValue()
    {
        float oldStandardValue = ValueWithoutDerived;
        float oldDisplayValue = Value;

        standardMultiplier = 1f;

        for (int i = 0; i < standardMultipliers.Count; i++)
        {
            standardMultiplier *= standardMultipliers[i].Value;
        }

        NotifyStatUpdated(oldStandardValue, oldDisplayValue);
    }

    private void RecalculateDerivedMultiplierValue()
    {
        float oldDisplayValue = Value;

        derivedMultiplier = 1f;

        for (int i = 0; i < derivedMultipliers.Count; i++)
        {
            derivedMultiplier *= derivedMultipliers[i].Value;
        }

        NotifyValueOnlyChanged(oldDisplayValue);
    }

    public void Upgrade()
    {
        float oldStandardValue = ValueWithoutDerived;
        float oldDisplayValue = Value;

        Level += 1;

        if (increase != 0f)
        {
            NotifyStatUpdated(oldStandardValue, oldDisplayValue);
        }
    }

    public void Reset()
    {
        onStatUpdated = null;
        onStatValueChanged = null;

        standardMultipliers.Clear();
        derivedMultipliers.Clear();

        standardFlatModifier = 0f;
        derivedFlatModifier = 0f;

        standardMultiplier = 1f;
        derivedMultiplier = 1f;

        Level = 1;
    }

    private void NotifyStatUpdated(float oldStandardValue, float oldDisplayValue)
    {
        onStatUpdated?.Invoke(new StatUpdate(oldStandardValue));
        NotifyValueOnlyChanged(oldDisplayValue);
    }

    private void NotifyValueOnlyChanged(float oldDisplayValue)
    {
        float newDisplayValue = Value;
        if (Mathf.Approximately(oldDisplayValue, newDisplayValue)) return;

        onStatValueChanged?.Invoke(new StatValueChange(oldDisplayValue, newDisplayValue));
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
        private bool isDerived = false;

        public Multiplier() { }

        public Multiplier(float multiplier)
        {
            Value = multiplier;
        }

        public void LinkToStat(Stat stat, bool isDerived)
        {
            linkedStat = stat;
            this.isDerived = isDerived;
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

                if (isDerived)
                {
                    linkedStat.RecalculateDerivedMultiplierValue();
                }
                else
                {
                    linkedStat.RecalculateStandardMultiplierValue();
                }
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

    public readonly struct StatValueChange
    {
        public readonly float oldValue;
        public readonly float newValue;

        public StatValueChange(float oldValue, float newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatusEffectHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<StatusEffectData> statusEffectsImmunityConfig = new List<StatusEffectData>();

    public bool Immune => immunitySources.Count > 0;

    public List<StatusEffect> StatusEffects => statusEffects;

    /// <summary> Called before any effect is applied. Returns the data of the status effect, source and stacks to apply. </summary>
    public Action<StatusEffectData, GameObject, int> onApplyEffect;

    public Action<StatusEffect> onEffectApplied;
    public Action<StatusEffect> onStacksApplied;
    public Action<StatusEffect> onEffectRemoved;
    public Action<StatusEffect> onStacksRemoved;

    private readonly HashSet<StatusEffectData> statusEffectsImmunity = new HashSet<StatusEffectData>();
    private readonly List<StatusEffect> statusEffects = new List<StatusEffect>();

    private readonly HashSet<int> immunitySources = new HashSet<int>();

    public void ApplyEffect(StatusEffectData statusEffectData, GameObject source)
    {
        ApplyEffect(statusEffectData, source, 1);
    }

    public void ApplyEffect(StatusEffectData statusEffectData, GameObject source, int stacks)
    {
        onApplyEffect?.Invoke(statusEffectData, source, stacks);

        if (statusEffectsImmunity.Contains(statusEffectData) || Immune)
        {
            return;
        }

        if (TryGetActiveStatusEffect(statusEffectData, out StatusEffect statusEffect))
        {
            int stacksToApply = Mathf.Min(stacks, statusEffect.Data.MaxStacks - statusEffect.stacks);

            if (stacksToApply > 0)
            {
                ApplyStacks(statusEffect, stacksToApply);
            }
        }
        else
        {
            AddNewStatusEffect(statusEffectData, source);
        }
    }

    public void RemoveEffect(StatusEffect statusEffect)
    {
        statusEffect.OnRemove();
        statusEffects.Remove(statusEffect);

        onEffectRemoved?.Invoke(statusEffect);
    }

    public void RemoveOneEffectStack(StatusEffect statusEffect)
    {
        if (TryGetActiveStatusEffect(statusEffect.Data, out StatusEffect foundStatusEffect))
        {
            foundStatusEffect.stacks -= 1;

            if (foundStatusEffect.stacks > 0)
            {
                onStacksRemoved?.Invoke(statusEffect);
            }
            else
            {
                RemoveEffect(foundStatusEffect);
            }
        }
    }

    public bool TryGetActiveStatusEffect(StatusEffectData statusEffect, out StatusEffect activeStatusEffect)
    {
        activeStatusEffect = null;

        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].Data == statusEffect)
            {
                activeStatusEffect = statusEffects[i];
                return true;
            }
        }

        return false;
    }

    public bool HasStatusEffect(StatusEffectData statusEffect)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].Data == statusEffect)
            {
                return true;
            }
        }

        return false;
    }

    public void ApplyImmunityToStatusEffect(StatusEffectData statusEffect)
    {
        statusEffectsImmunity.Add(statusEffect);
    }

    public void AddImmunitySource(int ID)
    {
        immunitySources.Add(ID);
    }

    public void RemoveImmunitySource(int ID)
    {
        immunitySources.Remove(ID);
    }

    void Awake()
    {
        for (int i = 0; i < statusEffectsImmunityConfig.Count; i++)
        {
            ApplyImmunityToStatusEffect(statusEffectsImmunityConfig[i]);
        }
    }

    void Update()
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            statusEffects[i].OnUpdate();
        }
    }

    void AddNewStatusEffect(StatusEffectData statusEffectData, GameObject source)
    {
        StatusEffect statusEffect = statusEffectData.CreateEffect(this, gameObject, source);

        statusEffects.Add(statusEffect);
        statusEffect.stacks = 1;
        statusEffect.OnApply();

        onEffectApplied?.Invoke(statusEffect);
    }

    void ApplyStacks(StatusEffect statusEffect, int stacks)
    {
        statusEffect.stacks += stacks;
        statusEffect.OnStackApply();

        onStacksApplied?.Invoke(statusEffect);
    }
}
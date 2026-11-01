using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities;
using Zeke.Items;

public class EngineerExosuit : Exosuit
{
    [Header("Engineer Exosuit Dependency")]
    [SerializeField] private ItemHandler itemHandler;

    [Header("Abilities")]
    [SerializeField] private Dictionary<AbilityType, AbilityData> suitAbilities;
    [SerializeField] private List<AbilityLock> abilityLocks;

    [Header("Stats")]
    [SerializeField] private float flatMoveSpeedBoost = 0f;

    [SerializeField] private float moveSpeedMultiplier = 1f;
    [SerializeField] private float damageMultiplier = 1f;

    private Stat.Multiplier moveSpeedStatMultiplier;
    private Stat.Multiplier damageStatMultiplier;

    private readonly List<IAbility> abilitiesInstance = new List<IAbility>();

    protected override void OnEnterVehicle(GameObject source)
    {
        ApplyExoBuffs(source);
        AddSuitSkills(source);
        LockAbilities(source);
        base.OnEnterVehicle(source);
    }

    protected override void OnExitVehicle(GameObject source)
    {
        RemoveExoBuffs(source);
        RemoveSuitSkills(source);
        UnlockAbilities(source);
        base.OnExitVehicle(source);
    }

    private void Awake()
    {
        moveSpeedStatMultiplier = new Stat.Multiplier(moveSpeedMultiplier);
        damageStatMultiplier = new Stat.Multiplier(damageMultiplier);
    }

    private void ApplyExoBuffs(GameObject source)
    {
        if (source.TryGetComponent(out EntityMove entityMove))
        {
            entityMove.MoveSpeed.AddMultiplier(moveSpeedStatMultiplier);
            entityMove.MoveSpeed.ApplyFlatModifier(flatMoveSpeedBoost);
        }

        Damageable.DamageEvent.onDealDamage.Subscribe(source, IncreaseDamage);
    }

    private void RemoveExoBuffs(GameObject source)
    {
        if (source.TryGetComponent(out EntityMove entityMove))
        {
            entityMove.MoveSpeed.RemoveMultiplier(moveSpeedStatMultiplier);
            entityMove.MoveSpeed.ApplyFlatModifier(-flatMoveSpeedBoost);
        }

        Damageable.DamageEvent.onDealDamage.Unsubscribe(source, IncreaseDamage);
    }

    private void AddSuitSkills(GameObject source)
    {
        if (source.TryGetComponent(out AbilityController abilityController))
        {
            foreach (AbilityType abilityType in suitAbilities.Keys)
            {
                abilitiesInstance.Add(abilityController.AddAbility(suitAbilities[abilityType], abilityType));
            }
        }
    }

    private void RemoveSuitSkills(GameObject source)
    {
        if (source.TryGetComponent(out AbilityController abilityController))
        {
            for (int i = 0; i < abilitiesInstance.Count; i++)
            {
                abilityController.RemoveAbility(abilitiesInstance[i]);
            }

            abilitiesInstance.Clear();
        }
    }

    private void LockAbilities(GameObject source) 
    {
        if (source.TryGetComponent(out AbilityController abilityController))
        {
            for (int i = 0; i < abilityLocks.Count; i++)
            {
                abilityController.AddAbilityLock(abilityLocks[i]);
            }
        }
    }

    private void UnlockAbilities(GameObject source)
    {
        if (source.TryGetComponent(out AbilityController abilityController))
        {
            for (int i = 0; i < abilityLocks.Count; i++)
            {
                abilityController.RemoveAbilityLock(abilityLocks[i]);
            }
        }
    }

    private void IncreaseDamage(Damageable.DamageEvent damageEvent)
    {
        damageEvent.Multiplier.Multiply(damageStatMultiplier.Value);
    }
}
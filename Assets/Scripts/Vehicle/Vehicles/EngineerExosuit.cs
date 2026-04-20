using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities;
using Zeke.Items;

public class EngineerExosuit : Exosuit
{
    [Header("Engineer Exosuit Dependency")]
    [SerializeField] private ItemHandler itemHandler;

    [Header("Abilities")]
    [SerializeField] private List<AbilityData> suitAbilities;
    [SerializeField] private List<AbilityLock> abilityLocks;

    [Header("Stats")]
    [SerializeField] private float flatMoveSpeedBoost = 0f;

    [SerializeField] private float moveSpeedMultiplier = 1f;
    [SerializeField] private float damageMultiplier = 1f;

    private Stat.Multiplier moveSpeedStatMultiplier;
    private Stat.Multiplier damageStatMultiplier;

    private readonly List<SwitchedAbilityData> switchedAbilities = new List<SwitchedAbilityData>();

    protected override void OnEnterVehicle(GameObject source)
    {
        ApplyExoBuffs(source);
        InheritItems(source);
        AddSuitSkills(source);
        LockAbilities(source);
        base.OnEnterVehicle(source);
    }

    protected override void OnExitVehicle(GameObject source)
    {
        RemoveExoBuffs(source);
        ClearInheritItems(source);
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

    private void InheritItems(GameObject source)
    {
        if (source.TryGetComponent(out ItemHandler itemHandler))
        {
            this.itemHandler.AddItems(itemHandler.ItemsData);
        }
    }

    private void AddSuitSkills(GameObject source)
    {
        if (source.TryGetComponent(out AbilityController abilityController))
        {
            for (int i = 0; i < suitAbilities.Count; i++)
            {
                switchedAbilities.Add(new SwitchedAbilityData(suitAbilities[i].AbilityType, abilityController.SwitchAbility(suitAbilities[i])));
            }
        }
    }

    private void RemoveSuitSkills(GameObject source)
    {
        if (source.TryGetComponent(out AbilityController abilityController))
        {
            for (int i = 0; i < switchedAbilities.Count; i++)
            {
                if (switchedAbilities[i].storedAbility != null)
                {
                    abilityController.SwitchAbility(switchedAbilities[i].storedAbility, false);
                }
                else
                {
                    abilityController.RemoveAbility(switchedAbilities[i].abilityType);
                }
            }
        }

        switchedAbilities.Clear();
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

    private void ClearInheritItems(GameObject source)
    {
        itemHandler.ClearItems();
    }

    private void IncreaseDamage(Damageable.DamageEvent damageEvent)
    {
        damageEvent.damageMultiplier *= damageStatMultiplier.Value;
    }

    private readonly struct SwitchedAbilityData
    {
        public readonly AbilityType abilityType;
        public readonly IAbility storedAbility;

        public SwitchedAbilityData(AbilityType abilityType, IAbility storedAbility)
        {
            this.abilityType = abilityType;
            this.storedAbility = storedAbility;
        }
    }
}
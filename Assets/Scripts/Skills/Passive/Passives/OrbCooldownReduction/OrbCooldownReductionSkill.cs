using UnityEngine;
using Zeke.Abilities;

public class OrbCooldownReductionSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly OrbCooldownReductionSkillData data;

    private readonly GameObject source;

    private bool hasRequiredComponents = false;

    private AbilityController abilityController;

    private readonly Stat flatCooldownDecrease;

    public OrbCooldownReductionSkill(GameObject source, PassiveController passiveController, OrbCooldownReductionSkillData data, Stat flatCooldownDecrease) : base(passiveController)
    {
        this.source = source;
        this.data = data;

        this.flatCooldownDecrease = flatCooldownDecrease;
    }

    public override void Awake()
    {
        LookForComponents();

        if (hasRequiredComponents)
        {
            SubscribeToEvents();
        }
    }

    public override void OnRemove()
    {
        if (hasRequiredComponents)
        {
            UnsubscribeFromEvents();
        }
    }

    protected override void UpgradeInternal()
    {
        flatCooldownDecrease.Upgrade();
    }

    private void LookForComponents()
    {
        hasRequiredComponents = source.TryGetComponent(out abilityController);
    }

    private void SubscribeToEvents()
    {
        Damageable.DamageEvent.onDamageDealt.Subscribe(source, OnDamageDealt);
    }

    private void UnsubscribeFromEvents()
    {
        Damageable.DamageEvent.onDamageDealt.Unsubscribe(source, OnDamageDealt);
    }

    private void OnDamageDealt(Damageable.DamageEvent damageEvent)
    {
        if (damageEvent.SourceObject == null) return;

        if (damageEvent.SourceObject.TryGetComponent(out OrbIdentifier _))
        {
            ReduceCooldowns();
        }
    }

    private void ReduceCooldowns()
    {
        foreach(AbilityController.AbilitySlot abilitySlot in abilityController.Abilities.Values)
        {
            IAbility ability = abilitySlot.Ability;

            if (ability == null) continue;

            if (!ability.DurationActive)
            {
                ability.SetCooldownTimer(ability.CooldownTimer + flatCooldownDecrease.Value);
            }
        }
    }
}
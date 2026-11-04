using UnityEngine;
using Zeke.Abilities;
using Zeke.GlobalEventsBus;

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

    protected override void UpgradeInternal(int levels)
    {
        flatCooldownDecrease.Upgrade(levels);
    }

    private void LookForComponents()
    {
        hasRequiredComponents = source.TryGetComponent(out abilityController);
    }

    private void SubscribeToEvents()
    {
        if (source.TryGetComponent(out DamageEventListener eventListener))
        {
            eventListener.OnDealtDamage.Subscribe(OnDealtDamage);
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (source.TryGetComponent(out DamageEventListener eventListener))
        {
            eventListener.OnDealtDamage.Unsubscribe(OnDealtDamage);
        }
    }

    private void OnDealtDamage(Damageable.DamageEvent.OnDealtDamageEvent damageDealtEvent)
    {
        Damageable.DamageEvent damageEvent = damageDealtEvent.damageEvent;

        if (damageEvent.SourceObject == null) return;

        if (damageEvent.SourceObject.TryGetComponent(out OrbIdentifier _))
        {
            ReduceCooldowns();
        }
    }

    private void ReduceCooldowns()
    {
        foreach(AbilityController.AbilityTypeSlot abilityTypeSlot in abilityController.AbilityTypeSlots.Values)
        {
            if (abilityTypeSlot.CurrentAbilitySlot == null) continue;

            IAbility ability = abilityTypeSlot.CurrentAbilitySlot;

            if (!ability.DurationActive)
            {
                ability.SetCooldownTimer(ability.CooldownTimer + flatCooldownDecrease.Value);
            }
        }
    }
}
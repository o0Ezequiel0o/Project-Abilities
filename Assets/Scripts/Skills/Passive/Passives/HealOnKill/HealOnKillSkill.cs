using UnityEngine;

public class HealOnKillSkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly HealOnKillSkillData data;

    private readonly GameObject source;

    private Damageable damageable;
    private bool hasRequiredComponents = false;

    private readonly Stat healAmount;

    public HealOnKillSkill(GameObject source, PassiveController passiveController, HealOnKillSkillData data, Stat healAmount) : base(passiveController)
    {
        this.source = source;
        this.data = data;

        this.healAmount = healAmount;
    }

    public override void Awake()
    {
        LookForComponents();
        Damageable.DamageEvent.onKill.Subscribe(source, OnKill);
    }

    public override void OnRemove()
    {
        Damageable.DamageEvent.onKill.Unsubscribe(source, OnKill);
    }

    public override void UpgradeInternal()
    {
        healAmount.Upgrade();
    }

    private void LookForComponents()
    {
        hasRequiredComponents = source.TryGetComponent(out damageable);
    }

    private void OnKill(Damageable.DamageEvent _)
    {
        if (!hasRequiredComponents) return;
        damageable.GiveHealing(healAmount.Value, source, null);
    }
}
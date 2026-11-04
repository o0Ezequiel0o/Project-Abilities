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

        if (source.TryGetComponent(out DamageEventListener eventListener))
        {
            eventListener.OnKill.Subscribe(OnKill);
        }
    }

    public override void OnRemove()
    {
        if (source.TryGetComponent(out DamageEventListener eventListener))
        {
            eventListener.OnKill.Unsubscribe(OnKill);
        }
    }

    protected override void UpgradeInternal(int levels)
    {
        healAmount.Upgrade(levels);
    }

    private void LookForComponents()
    {
        hasRequiredComponents = source.TryGetComponent(out damageable);
    }

    private void OnKill(Damageable.DamageEvent.OnKillEvent killEvent)
    {
        Damageable.DamageEvent damageEvent = killEvent.damageEvent;

        if (!hasRequiredComponents) return;
        if (source == damageEvent.Receiver) return;

        HealInfo heal = new HealInfo(healAmount.Value, data.ProcCoefficient);
        damageable.GiveHealing(heal, source, null);
    }
}
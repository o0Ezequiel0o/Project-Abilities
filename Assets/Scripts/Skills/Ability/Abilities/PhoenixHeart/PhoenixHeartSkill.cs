using UnityEngine;

public class PhoenixHeartSkill : AbilityBase
{
    public override AbilityData Data => data;

    private bool hasRequiredComponents = true;

    private readonly PhoenixHeartSkillData data;
    private readonly GameObject source;

    private readonly Stat healingPerStack;
    private readonly Stat hitRadius;
    private Collider2D[] hits;

    private Damageable damageable;

    public PhoenixHeartSkill(GameObject source, AbilityController controller, PhoenixHeartSkillData data, Stat cooldownTime, Stat hitRadius, Stat healingPerStack) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.healingPerStack = healingPerStack;
        this.hitRadius = hitRadius;
    }

    public override bool CanActivate()
    {
        return !DurationActive && hasRequiredComponents;
    }

    public override bool CanDeactivate()
    {
        return DurationActive && hasRequiredComponents;
    }

    protected override void Awake()
    {
        LookForComponents();
    }

    protected override void OnActivation()
    {
        hits = Physics2D.OverlapCircleAll(source.transform.position, hitRadius.Value, data.HitLayer);
        float healingToReceive = 0f;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out StatusEffectHandler statusEffectHandler))
            {
                if (statusEffectHandler.TryGetActiveStatusEffect(data.EffectToConsume, out StatusEffect statusEffect))
                {
                    healingToReceive += healingPerStack.Value * statusEffect.stacks;
                    statusEffectHandler.RemoveEffect(statusEffect);
                }
            }
        }

        damageable.GiveHealing(healingToReceive, source, source);
    }

    protected override void OnDeactivation() { }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive && hasRequiredComponents;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    private void LookForComponents()
    {
        if (source.TryGetComponent(out damageable))
        {
            hasRequiredComponents = true;
        }
        else
        {
            hasRequiredComponents = false;
        }
    }

    private void UpgradeStats()
    {
        healingPerStack.Upgrade();
        cooldownTime.Upgrade();
        hitRadius.Upgrade();
    }
}
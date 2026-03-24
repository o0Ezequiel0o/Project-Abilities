using UnityEngine;

public class ExplodeSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly ExplodeSkillData data;
    private readonly GameObject source;

    private readonly Stat radius;
    private readonly Stat damage;

    public ExplodeSkill(GameObject source, AbilityController controller, ExplodeSkillData data, Stat cooldownTime, Stat radius, Stat damage) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.radius = radius;
        this.damage = damage;
    }

    public override bool CanActivate()
    {
        return !DurationActive;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    protected override void OnActivation()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(source.transform.position, radius.Value, data.HitLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            bool damageRejected = false;

            if (hits[i].gameObject == source || TeamManager.IsAlly(hits[i].gameObject, source)) continue;

            if (hits[i].TryGetComponent(out Damageable damageable))
            {
                damageRejected = damageable.DealDamage(new DamageInfo(damage.Value, 0f, 1f), source, source).damageRejected;
            }

            if (damageRejected) continue;

            Vector3 knockBackDirection = (hits[i].transform.position - source.transform.position).normalized;

            if (hits[i].TryGetComponent(out Physics physics))
            {
                physics.AddForce(data.Knockback * knockBackDirection);
            }
        }

        if (source.TryGetComponent(out Damageable sourceDamageable))
        {
            sourceDamageable.DealDamage(new DamageInfo(damage.Value, 0f, 1f), source, source);
        }
    }

    protected override void OnDeactivation() { }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    private void UpgradeStats()
    {
        radius.Upgrade();
        damage.Upgrade();
    }
}
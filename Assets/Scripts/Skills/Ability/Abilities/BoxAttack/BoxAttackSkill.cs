using UnityEngine;

public class BoxAttackSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly BoxAttackSkillData data;
    private readonly GameObject source;

    private readonly Stat damage;

    public BoxAttackSkill(GameObject source, AbilityController controller, BoxAttackSkillData data, Stat cooldownTime, Stat damage) : base(controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

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

    protected override void OnActivation()
    {
        Attack(damage.Value);
    }

    protected override void OnDeactivation() { }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeStats();
    }

    protected virtual void OnDamageDealt(Collider2D hit) { }

    protected void Attack(float damage)
    {
        PerformAttack(damage);
        SpawnVisualEffect();
    }

    private void PerformAttack(float damage)
    {
        Vector3 distanceFromCenter = 0.5f * data.Range.y * controller.CastDirection;
        Vector2 position = controller.CastWorldPosition + distanceFromCenter;

        Collider2D[] hits = Physics2D.OverlapBoxAll(position, data.Range, data.HitLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            if (TryDealDamage(hits[i], damage))
            {
                OnDamageDealt(hits[i]);
                ApplyKnockBack(hits[i]);
            }
        }
    }

    private bool TryDealDamage(Collider2D target, float damage)
    {
        if (TeamManager.IsEnemy(source, target.gameObject))
        {
            if (target.TryGetComponent(out Damageable damageable))
            {
                Damageable.DamageEvent damageEvent = damageable.DealDamage(new DamageInfo(damage, 0f, 1f), source, source);

                if (damageEvent.DamageDealt > 0f && !damageEvent.damageRejected)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void ApplyKnockBack(Collider2D target)
    {
        if (target.TryGetComponent(out Physics physics))
        {
            physics.AddForce(data.Knockback, controller.CastDirection);
        }
    }

    private void SpawnVisualEffect()
    {
        float randomRotation = Random.Range(-data.AttackEffectRandomRotationOffset, data.AttackEffectRandomRotationOffset);

        Vector3 spawnPosition = controller.CastWorldPosition + controller.CastDirection * data.AttackEffectCenterDistance;
        Quaternion spawnRotation = Quaternion.Euler(0f, 0f, controller.CastWorldRotation.eulerAngles.z + randomRotation);

        Object.Destroy(Object.Instantiate(data.AttackEffect, spawnPosition, spawnRotation), data.AttackEffectDuration);
    }

    private void UpgradeStats()
    {
        damage.Upgrade();
    }
}
using UnityEngine;

public class KnifeSwingSkill : BoxAttackSkill
{
    public override AbilityData Data => data;

    private readonly KnifeSwingSkillData data;
    private readonly GameObject source;

    private readonly Stat swingCooldown;
    private readonly Stat extraDamage;

    private float timer = 0f;

    public KnifeSwingSkill(GameObject source, AbilityController controller, KnifeSwingSkillData data, Stat cooldownTime, Stat damage, Stat swingCooldown, Stat extraDamage) : base(source, controller, data, cooldownTime, damage)
    {
        this.data = data;
        this.source = source;

        this.extraDamage = extraDamage;
        this.swingCooldown = swingCooldown;
    }

    protected override void Awake()
    {
        base.Awake();
        SetMaxCharges(data.Charges);
    }

    public override bool CanActivate()
    {
        return base.CanActivate() && timer >= swingCooldown.Value;
    }

    protected override void OnDamageDealt(Collider2D hit)
    {
        if (data.EffectToApply == null) return;
        if (TeamManager.IsAlly(source, hit.gameObject)) return;

        if (hit.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            if (statusEffectHandler.HasStatusEffect(data.EffectToApply))
            {
                DealExtraDamage(hit.gameObject);
            }

            statusEffectHandler.ApplyEffect(data.EffectToApply, source);
        }
    }

    protected override void OnActivation()
    {
        base.OnActivation();
        timer = 0f;
    }

    protected override void UpdateUnactive()
    {
        timer += Time.deltaTime;
    }

    protected override void UpgradeInternal()
    {
        base.UpgradeInternal();
        swingCooldown.Upgrade();
    }

    private void DealExtraDamage(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Damageable damageable))
        {
            damageable.DealDamage(new DamageInfo(extraDamage.Value, 0f, data.ExtraDamageProcCoefficient), source, source);
        }
    }
}
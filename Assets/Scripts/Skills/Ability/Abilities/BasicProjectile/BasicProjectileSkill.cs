using UnityEngine;

public class BasicProjectileSkill : ProjectileSkillBase<Projectile>
{
    public override AbilityData Data => data;

    private readonly bool hasRequiredComponents = true;

    private readonly BasicProjectileSkillData data;
    private readonly GameObject source;

    public BasicProjectileSkill(GameObject source, AbilityController controller, BasicProjectileSkillData data, Stat cooldownTime) : base(data, controller, cooldownTime)
    {
        this.source = source;
        this.data = data;
    }

    public override bool CanActivate()
    {
        return !DurationActive || hasRequiredComponents;
    }

    public override bool CanDeactivate()
    {
        return DurationActive || hasRequiredComponents;
    }

    protected override void OnActivation()
    {
        LaunchProjectile(controller.CastWorldPosition, controller.CastDirection, source);
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
        UpgradeProjectileStats();
    }
}
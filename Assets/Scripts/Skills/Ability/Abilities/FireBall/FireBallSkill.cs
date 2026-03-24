using UnityEngine;

public class FireBallSkill : ProjectileSkillBase<FireBallProjectile>
{
    public override AbilityData Data => data;

    private readonly FireBallSkillData data;
    private readonly GameObject source;

    private readonly Stat damageRadius;

    public FireBallSkill(GameObject source, AbilityController controller, FireBallSkillData data, Stat cooldownTime, Stat damageRadius) : base(data, controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.damageRadius = damageRadius;
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
        FireBallProjectile fireballProjectile = LaunchAndGetProjectile(controller.CastWorldPosition, controller.CastDirection, source);
        fireballProjectile.SetDamageRadius(damageRadius.Value);
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
        UpgradeProjectileStats();
    }

    private void UpgradeStats()
    {
        cooldownTime.Upgrade();
        damageRadius.Upgrade();
    }
}
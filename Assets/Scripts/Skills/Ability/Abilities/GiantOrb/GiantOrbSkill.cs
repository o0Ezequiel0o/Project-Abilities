using UnityEngine;

public class GiantOrbSkill : ProjectileSkillBase<GiantOrbProjectile>
{
    public override AbilityData Data => data;

    private readonly GiantOrbSkillData data;
    private readonly GameObject source;

    private readonly Stat homingOrbDamage;
    private readonly Stat homingOrbSpeed;
    private readonly Stat homingOrbRange;

    public GiantOrbSkill(GameObject source, AbilityController controller, GiantOrbSkillData data, Stat cooldownTime, Stat homingOrbDamage, Stat homingOrbSpeed, Stat homingOrbRange) : base(data, controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.homingOrbDamage = homingOrbDamage;
        this.homingOrbSpeed = homingOrbSpeed;
        this.homingOrbRange = homingOrbRange;
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
        GiantOrbProjectile giantOrb = LaunchAndGetProjectile(controller.CastWorldPosition, controller.CastDirection, source);
        giantOrb.SetHomingOrbsValues(homingOrbDamage.Value, homingOrbSpeed.Value, homingOrbRange.Value);
    }

    protected override bool CanUpgrade()
    {
        return !DurationActive;
    }

    protected override void UpgradeInternal()
    {
        UpgradeProjectileStats();
        UpgradeStats();
    }

    private void UpgradeStats()
    {
        homingOrbDamage.Upgrade();
        homingOrbSpeed.Upgrade();
        homingOrbRange.Upgrade();
    }
}
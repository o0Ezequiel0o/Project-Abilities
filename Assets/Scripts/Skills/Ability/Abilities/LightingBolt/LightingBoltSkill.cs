using UnityEngine;

public class LightingBoltSkill : ProjectileSkillBase<LightingBoltProjectile>
{
    public override AbilityData Data => data;

    private readonly LightingBoltSkillData data;
    private readonly GameObject source;

    private readonly Stat spreadTargets;

    public LightingBoltSkill(GameObject source, AbilityController controller, LightingBoltSkillData data, Stat cooldownTime, Stat spreadTargets) : base(data, controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.spreadTargets = spreadTargets;
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
        LightingBoltProjectile lightingBolt = LaunchAndGetProjectile(controller.CastWorldPosition, controller.CastDirection, source);
        lightingBolt.SetSpreadTargets(spreadTargets.Value);
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

    void UpgradeStats()
    {
        cooldownTime.Upgrade();
        spreadTargets.Upgrade();
    }
}
using UnityEngine;

public class MachineGunSkill : GunSkillBase<BasicProjectile>
{
    public override AbilityData Data => data;

    private readonly MachineGunSkillData data;
    private readonly GameObject source;

    public MachineGunSkill(GameObject source, AbilityController controller, MachineGunSkillData data, Stat cooldownTime) : base(controller, data, cooldownTime)
    {
        this.source = source;
        this.data = data;
    }

    public override bool CanActivate()
    {
        return !DurationActive && base.CanActivate();
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    protected override void OnActivation()
    {
        LaunchProjectile(controller.CastWorldPosition, controller.CastDirection, source);
        base.OnActivation();
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
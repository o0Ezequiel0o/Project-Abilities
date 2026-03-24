using UnityEngine;

public class MegaFireballSkill : ProjectileSkillBase<MegaFireballProjectile>
{
    public override AbilityData Data => data;

    private readonly MegaFireballSkillData data;
    private readonly GameObject source;

    private readonly Stat damageRadius;

    public MegaFireballSkill(GameObject source, AbilityController controller, MegaFireballSkillData data, Stat cooldownTime, Stat damageRadius) : base(data, controller, cooldownTime)
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
        MegaFireballProjectile megaFireballProjectile = LaunchAndGetProjectile(controller.CastWorldPosition, controller.CastDirection, source);
        megaFireballProjectile.SetDamageRadiusAndFireballsAmount(damageRadius.Value, data.FireballsAmount);
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
        damageRadius.Upgrade();
    }
}
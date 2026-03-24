using UnityEngine;

public class BoomerangSkill : ProjectileSkillBase<Projectile>
{
    public override AbilityData Data => data;

    private readonly BoomerangSkillData data;
    private readonly GameObject source;

    private int currentProjectiles = 0;
    private readonly Stat maxBoomerangs;

    public BoomerangSkill(GameObject source, AbilityController controller, BoomerangSkillData data, Stat cooldownTime, Stat maxBoomerangs) : base(data, controller, cooldownTime)
    {
        this.source = source;
        this.data = data;

        this.maxBoomerangs = maxBoomerangs;
    }

    public override bool CanActivate()
    {
        return !DurationActive && currentProjectiles < maxBoomerangs.Value;
    }

    public override bool CanDeactivate()
    {
        return DurationActive;
    }

    protected override void OnActivation()
    {
        Projectile projectile = LaunchAndGetProjectile(controller.CastWorldPosition, controller.CastDirection, source);
        projectile.onDespawn += OnProjectileDespawn;

        currentProjectiles += 1;
    }

    protected override void OnDeactivation() { }

    protected override void UpdateActive()
    {
        TryDeactivate();
    }

    protected override bool CanUpgrade()
    {
        return true;
    }

    protected override void UpgradeInternal()
    {
        UpgradeProjectileStats();
        maxBoomerangs.Upgrade();
    }

    private void OnProjectileDespawn(Projectile projectile)
    {
        projectile.onDespawn -= OnProjectileDespawn;
        currentProjectiles -= 1;
    }
}
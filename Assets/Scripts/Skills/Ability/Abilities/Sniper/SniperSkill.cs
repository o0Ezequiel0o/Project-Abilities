using UnityEngine;

public class SniperSkill : AbilityBase
{
    public override AbilityData Data => data;

    private readonly SniperSkillData data;
    private readonly GameObject source;

    private readonly Stat speed;
    private readonly Stat damage;
    private readonly Stat maxRange;

    private readonly Stat doubleDamageChance;

    private readonly GameObjectPool<Projectile> projectilePool = new GameObjectPool<Projectile>();

    public SniperSkill(GameObject source, AbilityController controller, SniperSkillData data, Stat cooldownTime, Stat damage, Stat speed, Stat maxRange, Stat doubleDamageChance) : base(controller, cooldownTime)
    {
        this.data = data;
        this.source = source;

        this.maxRange = maxRange;
        this.damage = damage;
        this.speed = speed;

        this.doubleDamageChance = doubleDamageChance;
    }

    public override void OnDestroy()
    {
        projectilePool.Clear();
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
        float damage = this.damage.Value;

        if (doubleDamageChance.Value >= 1f)
        {
            damage *= 2f;
        }
        else
        {
            float randomNum = Random.Range(0f, 1f);

            if (randomNum < doubleDamageChance.Value)
            {
                damage *= 2;
            }
        }

        LaunchProjectile(controller.CastWorldPosition, controller.CastDirection, source, damage);
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
        speed.Upgrade();
        damage.Upgrade();
        maxRange.Upgrade();

        doubleDamageChance.Upgrade();
    }

    private void LaunchProjectile(Vector3 castWorldPosition, Vector3 castDirection, GameObject source, float damage)
    {
        Projectile projectile = projectilePool.Get(data.Prefab);

        Vector3 spawnPosition = castWorldPosition + data.CastDistanceAway * castDirection;
        projectile.Launch(spawnPosition, speed.Value, castDirection, maxRange.Value, damage, source);

        projectile.gameObject.SetActive(true);
    }
}
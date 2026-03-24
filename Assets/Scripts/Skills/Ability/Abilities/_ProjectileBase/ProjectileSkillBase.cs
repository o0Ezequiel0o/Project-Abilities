using UnityEngine;

public abstract class ProjectileSkillBase<T> : AbilityBase where T : Projectile
{
    private readonly GameObject prefab;

    private readonly Stat speed;
    private readonly Stat damage;
    private readonly Stat maxRange;

    private readonly float castDistanceAway;

    private readonly GameObjectPool<T> projectilePool = new GameObjectPool<T>();

    public ProjectileSkillBase(ProjectileSkillBaseData projectileSkillBaseData, AbilityController controller, Stat cooldownTime) : base(controller, cooldownTime)
    {
        castDistanceAway = projectileSkillBaseData.CastDistanceAway;
        prefab = projectileSkillBaseData.ProjectilePrefab;

        maxRange = projectileSkillBaseData.MaxRange;
        damage = projectileSkillBaseData.Damage;
        speed = projectileSkillBaseData.Speed;
    }

    protected void LaunchProjectile(Vector3 castWorldPosition, Vector3 castDirection, GameObject source)
    {
        LaunchAndGetProjectile(castWorldPosition, castDirection, source);
    }

    protected T LaunchAndGetProjectile(Vector3 castWorldPosition, Vector3 castDirection, GameObject source)
    {
        Projectile projectile = projectilePool.Get(prefab);

        Vector3 spawnPosition = castWorldPosition + castDistanceAway * castDirection;
        projectile.Launch(spawnPosition, speed.Value, castDirection, maxRange.Value, damage.Value, source);

        projectile.gameObject.SetActive(true);
        return (T)projectile;
    }

    protected void UpgradeProjectileStats()
    {
        speed.Upgrade();
        damage.Upgrade();
        maxRange.Upgrade();
    }

    public override void OnDestroy()
    {
        projectilePool.Clear();
    }
}
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    public class FirePiercingProjectile : FireDamageProjectile<PiercingProjectile>
    {
        private readonly FirePiercingProjectileData data;

        private readonly Stat pierce;

        public FirePiercingProjectile(FirePiercingProjectileData data, Stat damage, Stat pierce) : base(data, damage)
        {
            this.data = data;
            this.pierce = pierce;
        }

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            PiercingProjectile projectile = projectilePool.Get(data.Prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, pierce.ValueInt, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            pierce.Upgrade();
        }
    }
}
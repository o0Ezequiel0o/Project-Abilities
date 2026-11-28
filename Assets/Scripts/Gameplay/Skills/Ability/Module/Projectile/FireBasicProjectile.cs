using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    public class FireBasicProjectile : FireDamageProjectile<BasicProjectile>
    {
        private readonly FireBasicProjectileData data;

        public FireBasicProjectile(FireBasicProjectileData data, Stat damage) : base(data, damage)
        {
            this.data = data;
        }

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            BasicProjectile projectile = projectilePool.Get(data.Prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, source, team);
            projectile.gameObject.SetActive(true);
        }
    }
}
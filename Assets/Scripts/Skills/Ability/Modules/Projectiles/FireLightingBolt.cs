using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    public class FireLightingBolt : FireDamageProjectile<LightingBoltProjectile>
    {
        private readonly FireLightingBoltData data;

        private readonly Stat spreadTargets;

        public FireLightingBolt(FireLightingBoltData data, Stat damage, Stat spreadTargets) : base(data, damage)
        {
            this.data = data;
            this.spreadTargets = spreadTargets;
        }

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            LightingBoltProjectile projectile = projectilePool.Get(data.Prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, spreadTargets.Value, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            spreadTargets.Upgrade();
        }
    }
}
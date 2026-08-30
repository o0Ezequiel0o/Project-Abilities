using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireFireball : FireDamageProjectile<FireBallProjectile>
    {
        private readonly FireFireballData data;

        private readonly Stat damageRadius;

        public FireFireball(FireFireballData data, Stat damage, Stat damageRadius) : base(data, damage)
        {
            this.data = data;
            this.damageRadius = damageRadius;
        }

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            FireBallProjectile projectile = projectilePool.Get(data.Prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, damageRadius.Value, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            damageRadius.Upgrade();
        }
    }
}
using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireMegaFireball : FireDamageProjectile<MegaFireballProjectile>
    {
        private readonly FireMegaFireballData data;

        private readonly Stat fireballsAmount;

        public FireMegaFireball(FireMegaFireballData data, Stat damage, Stat fireballsAmount) : base(data, damage)
        {
            this.data = data;
            this.fireballsAmount = fireballsAmount;
        }

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            MegaFireballProjectile projectile = projectilePool.Get(data.Prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, data.ExplosionRadius, fireballsAmount.ValueInt, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            fireballsAmount.Upgrade();
        }
    }
}
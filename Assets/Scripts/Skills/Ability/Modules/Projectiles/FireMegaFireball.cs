using UnityEngine;
using System;
using Zeke.TeamSystem;
using static DamageProjectileBase;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireMegaFireball : FireDamageProjectile<MegaFireballProjectile>
    {
        [SerializeField] private Stat fireballsAmount;
        [SerializeField] private float explosionRadius;

        public FireMegaFireball() { }

        public FireMegaFireball(FireMegaFireball original) : base(original)
        {
            explosionRadius = original.explosionRadius;
            fireballsAmount = original.fireballsAmount.DeepCopy();
        }

        public override FireProjectileType DeepCopy() => new FireMegaFireball(this);

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            MegaFireballProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, explosionRadius, fireballsAmount.ValueInt, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            fireballsAmount.Upgrade();
        }
    }
}
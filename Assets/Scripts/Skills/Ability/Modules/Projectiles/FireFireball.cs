using UnityEngine;
using System;
using Zeke.TeamSystem;
using static DamageProjectileBase;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireFireball : FireDamageProjectile<FireBallProjectile>
    {
        [SerializeField] private Stat damageRadius;

        public FireFireball() { }

        public FireFireball(FireFireball original) : base(original)
        {
            damageRadius = original.damageRadius.DeepCopy();
        }

        public override FireProjectileType DeepCopy() => new FireFireball(this);

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            FireBallProjectile projectile = projectilePool.Get(prefab);
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
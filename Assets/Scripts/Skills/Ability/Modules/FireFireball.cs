using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
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

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team)
        {
            FireBallProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damage, damageRadius.Value, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            damageRadius.Upgrade();
            base.Upgrade();
        }
    }
}
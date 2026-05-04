using UnityEngine;
using System;
using Zeke.TeamSystem;
using static DamageProjectileBase;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireBasicProjectile : FireDamageProjectile<BasicProjectile>
    {
        public FireBasicProjectile() { }

        public FireBasicProjectile(FireBasicProjectile original) : base(original) { }

        public override FireProjectileType DeepCopy() => new FireBasicProjectile(this);

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            BasicProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, source, team);
            projectile.gameObject.SetActive(true);
        }
    }
}
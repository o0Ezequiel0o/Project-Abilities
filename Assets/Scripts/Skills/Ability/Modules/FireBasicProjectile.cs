using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireBasicProjectile : FireDamageProjectile<BasicProjectile>
    {
        public FireBasicProjectile() { }

        public FireBasicProjectile(FireBasicProjectile original) : base(original) { }

        public override FireProjectileType DeepCopy() => new FireBasicProjectile(this);

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team)
        {
            BasicProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damage, source, team);
            projectile.gameObject.SetActive(true);
        }
    }
}
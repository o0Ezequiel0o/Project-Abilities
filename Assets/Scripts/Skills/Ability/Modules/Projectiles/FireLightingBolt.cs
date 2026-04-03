using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireLightingBolt : FireDamageProjectile<LightingBoltProjectile>
    {
        [SerializeField] private Stat spreadTargets;

        public FireLightingBolt() { }

        public FireLightingBolt(FireLightingBolt original) : base(original)
        {
            spreadTargets = original.spreadTargets.DeepCopy();
        }

        public override FireProjectileType DeepCopy() => new FireLightingBolt(this);

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team)
        {
            LightingBoltProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damage, spreadTargets.Value, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            spreadTargets.Upgrade();
        }
    }
}
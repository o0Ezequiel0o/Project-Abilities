using System;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FirePiercingProjectile : FireDamageProjectile<PiercingProjectile>
    {
        [SerializeField] private Stat pierce;

        public FirePiercingProjectile() { }

        public FirePiercingProjectile(FirePiercingProjectile original) : base(original)
        {
            pierce = original.pierce.DeepCopy();
        }

        public override FireProjectileType DeepCopy() => new FirePiercingProjectile(this);

        public override bool CanLaunchProjectile() => true;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team)
        {
            PiercingProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damage, pierce.ValueInt, source, team);
            projectile.gameObject.SetActive(true);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            pierce.Upgrade();
        }
    }
}
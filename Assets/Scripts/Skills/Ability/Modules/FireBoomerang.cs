using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireBoomerang : FireDamageProjectile<BoomerangProjectile>
    {
        [SerializeField] private Stat maxBoomerangs;

        private int currentProjectiles = 0;

        public FireBoomerang() { }

        public FireBoomerang(FireBoomerang original) : base(original)
        {
            maxBoomerangs = original.maxBoomerangs.DeepCopy();
        }

        public override FireProjectileType DeepCopy() => new FireBoomerang(this);

        public override bool CanLaunchProjectile() => currentProjectiles < maxBoomerangs.Value;

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team)
        {
            BoomerangProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damage, source, team);
            projectile.gameObject.SetActive(true);

            projectile.onDespawn += OnProjectileDespawn;
            currentProjectiles += 1;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            maxBoomerangs.Upgrade();
        }

        private void OnProjectileDespawn(Projectile projectile)
        {
            projectile.onDespawn -= OnProjectileDespawn;
            currentProjectiles -= 1;
        }
    }
}
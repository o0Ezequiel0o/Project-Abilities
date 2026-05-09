using UnityEngine;
using System;
using Zeke.TeamSystem;
using static DamageProjectileBase;

namespace Zeke.Abilities.Modules.Projectiles
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

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            BoomerangProjectile projectile = projectilePool.Get(prefab);
            projectile.Launch(position, speed, direction, maxRange, damageData, knockback, source, team);
            projectile.gameObject.SetActive(true);

            projectile.OnDespawn.AddListener(OnProjectileDespawn);
            currentProjectiles += 1;
        }

        public override void Upgrade()
        {
            base.Upgrade();
            maxBoomerangs.Upgrade();
        }

        private void OnProjectileDespawn(Projectile projectile)
        {
            projectile.OnDespawn.RemoveListener(OnProjectileDespawn);
            currentProjectiles -= 1;
        }
    }
}
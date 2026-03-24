using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireBoomerang : GenericFireProjectile<BoomerangProjectile>
    {
        [SerializeField] private Stat maxBoomerangs;

        private int currentProjectiles = 0;

        public FireBoomerang(FireBoomerang original) : base(original)
        {
            maxBoomerangs = original.maxBoomerangs.DeepCopy();
        }

        public override AbilityModule CreateDeepCopy() => new FireBoomerang(this);

        public override bool CanActivate()
        {
            return currentProjectiles < maxBoomerangs.Value;
        }

        public override bool CanDeactivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            Projectile projectile = LaunchAndGetProjectile(spawn.position, spawn.up, source);
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
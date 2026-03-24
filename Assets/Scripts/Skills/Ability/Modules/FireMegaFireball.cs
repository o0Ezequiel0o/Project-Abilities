using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireMegaFireball : GenericFireProjectile<MegaFireballProjectile>
    {
        [SerializeField] private Stat fireballsAmount;
        [SerializeField] private float explosionRadius;

        public FireMegaFireball(FireMegaFireball original) : base(original)
        {
            explosionRadius = original.explosionRadius;
            fireballsAmount = original.fireballsAmount.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new FireMegaFireball(this);

        public override void Activate(bool holding)
        {
            MegaFireballProjectile megaFireballProjectile = LaunchAndGetProjectile(spawn.position, spawn.up, source);
            megaFireballProjectile.SetDamageRadiusAndFireballsAmount(explosionRadius, fireballsAmount.ValueInt);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            fireballsAmount.Upgrade();
        }
    }
}
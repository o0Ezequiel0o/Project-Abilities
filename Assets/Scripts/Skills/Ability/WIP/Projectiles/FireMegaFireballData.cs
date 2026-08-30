using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireMegaFireballData : FireDamageProjectileData<MegaFireballProjectile>
    {
        [SerializeField] private Stat fireballsAmount;
        [field: SerializeField] public float ExplosionRadius { get; private set; }

        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FireMegaFireball(this, Damage.DeepCopy(), fireballsAmount.DeepCopy());
        }
    }
}
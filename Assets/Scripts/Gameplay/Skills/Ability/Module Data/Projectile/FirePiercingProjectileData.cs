using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FirePiercingProjectileData : FireDamageProjectileData<PiercingProjectile>
    {
        [SerializeField] private Stat pierce;

        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FirePiercingProjectile(this, Damage.DeepCopy(), pierce.DeepCopy());
        }
    }
}
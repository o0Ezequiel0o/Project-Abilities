using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireLightingBoltData : FireDamageProjectileData<LightingBoltProjectile>
    {
        [SerializeField] private Stat spreadTargets;

        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FireLightingBolt(this, Damage.DeepCopy(), spreadTargets.DeepCopy());
        }
    }
}
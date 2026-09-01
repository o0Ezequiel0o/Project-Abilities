using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireFireballData : FireDamageProjectileData<FireBallProjectile>
    {
        [SerializeField] private Stat damageRadius;

        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FireFireball(this, Damage.DeepCopy(), damageRadius.DeepCopy());
        }
    }
}
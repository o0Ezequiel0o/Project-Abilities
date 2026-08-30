using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireBoomerangData : FireDamageProjectileData<BoomerangProjectile>
    {
        [SerializeField] private Stat maxBoomerangs;

        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FireBoomerang(this, Damage.DeepCopy(), maxBoomerangs.DeepCopy());
        }
    }
}
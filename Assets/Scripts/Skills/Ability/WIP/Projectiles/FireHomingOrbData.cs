using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireHomingOrbData : FireDamageProjectileData<HomingOrbProjectile>
    {
        [SerializeField] private Stat pierce;

        [field: Header("Find Target")]
        [field: SerializeField] public float TargetRange;
        [field: SerializeField] public LayerMask TargetLayer;
        [field: SerializeField] public LayerMask BlockLayer;

        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FireHomingOrb(this, Damage.DeepCopy(), pierce.DeepCopy());
        }
    }
}
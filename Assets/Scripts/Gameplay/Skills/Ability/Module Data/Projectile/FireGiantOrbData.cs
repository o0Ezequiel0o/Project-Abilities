using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireGiantOrbData : FireDamageProjectileData<GiantOrbProjectile>
    {
        [SerializeField] private Stat smallOrbDamage;
        [SerializeField] private Stat smallOrbSpeed;
        [SerializeField] private Stat smallOrbRange;
        [SerializeField] private Stat smallOrbPierce;

        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FireGiantOrb(this, Damage.DeepCopy(), smallOrbDamage.DeepCopy(), smallOrbSpeed.DeepCopy(), smallOrbRange.DeepCopy(), smallOrbPierce.DeepCopy());
        }
    }
}
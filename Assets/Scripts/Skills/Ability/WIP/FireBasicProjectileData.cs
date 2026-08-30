using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireBasicProjectileData : FireDamageProjectileData<BasicProjectile>
    {
        public override FireProjectileType CreateProjectileTypeModule()
        {
            return new FireBasicProjectile(this, Damage.DeepCopy());
        }
    }
}
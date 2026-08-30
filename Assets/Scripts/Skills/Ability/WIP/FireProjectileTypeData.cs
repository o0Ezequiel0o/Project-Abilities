using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public abstract class FireProjectileTypeData
    {
        public abstract FireProjectileType CreateProjectileTypeModule();
    }
}
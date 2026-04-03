using UnityEngine;

namespace Zeke.Abilities.Modules.Projectiles
{
    public abstract class FireProjectileType
    {
        public abstract FireProjectileType DeepCopy();

        public abstract bool CanLaunchProjectile();

        public abstract void LaunchProjectile(Vector3 position, Vector3 direction, float speed, float maxRange, GameObject source);

        public abstract void Upgrade();

        public abstract void Destroy();
    }
}
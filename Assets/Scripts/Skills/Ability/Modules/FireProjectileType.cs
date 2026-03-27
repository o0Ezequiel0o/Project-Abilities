using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public abstract class FireProjectileType
    {
        public abstract FireProjectileType DeepCopy();

        public abstract void Upgrade();

        public abstract bool CanLaunchProjectile();

        public abstract void LaunchProjectile(Vector3 position, Vector3 direction, float speed, float maxRange, GameObject source);
    }
}
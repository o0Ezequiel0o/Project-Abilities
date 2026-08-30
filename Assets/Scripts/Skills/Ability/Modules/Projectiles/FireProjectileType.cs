using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public abstract class FireProjectileType
    {
        public virtual FireProjectileType DeepCopy() { return null; } //TODO: Remove this

        public abstract bool CanLaunchProjectile();

        public abstract void LaunchProjectile(Vector3 position, Vector3 direction, float speed, float maxRange, GameObject source);

        public abstract void Upgrade();

        public abstract void Destroy();
    }
}
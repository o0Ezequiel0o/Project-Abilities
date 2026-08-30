using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireProjectile : AbilityModule
    {
        private readonly FireProjectileData data;
        private readonly FireProjectileType projectile;

        private readonly Stat speed;
        private readonly Stat maxRange;

        private Transform spawn;
        private GameObject source;

        public FireProjectile(FireProjectileData data, FireProjectileType projectile, Stat speed, Stat maxRange)
        {
            this.projectile = projectile;
            this.speed = speed;
            this.maxRange = maxRange;
            this.data = data;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.spawn = spawn;
            this.source = source;
        }

        public override bool CanActivate() => projectile.CanLaunchProjectile();

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            LaunchProjectile(spawn.position, spawn.up, speed.Value, maxRange.Value, source);
        }

        public override void Upgrade()
        {
            speed.Upgrade();
            maxRange.Upgrade();
            projectile.Upgrade();
        }

        public override void Destroy()
        {
            projectile.Destroy();
        }

        private void LaunchProjectile(Vector3 position, Vector3 direction, float speed, float maxRange, GameObject source)
        {
            position += data.FireDistance * direction;
            direction = ApplySpreadToDirection(direction);
            projectile.LaunchProjectile(position, direction, speed, maxRange, source);
        }

        private float DirectionToAngle(Vector2 direction)
        {
            return (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
        }

        private float GetRandomSpreadAngle()
        {
            return UnityEngine.Random.Range(data.Spread.Min, data.Spread.Max);
        }

        private Vector2 ApplySpreadToDirection(Vector2 direction)
        {
            return Quaternion.Euler(0f, 0f, DirectionToAngle(direction) - GetRandomSpreadAngle()) * Vector2.up;
        }
    }
}
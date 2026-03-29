using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireProjectile : AbilityModule
    {
        [Header("Casting")]
        [SerializeField] private float fireDistance;
        [SerializeField] private Limits spread = Limits.Zero;

        [Space]

        [SerializeReferenceDropdown, SerializeReference] private FireProjectileType projectile;

        [SerializeField] private Stat speed;
        [SerializeField] private Stat maxRange;

        private Transform spawn;
        private GameObject source;

        public FireProjectile() { }

        public FireProjectile(FireProjectile original)
        {
            fireDistance = original.fireDistance;

            speed = original.speed.DeepCopy();
            maxRange = original.maxRange.DeepCopy();
            projectile = original.projectile.DeepCopy();

            spread = new Limits(original.spread.Min, original.spread.Max);
        }

        public override AbilityModule DeepCopy() => new FireProjectile(this);

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
            position += fireDistance * direction;
            direction = ApplySpreadToDirection(direction);
            projectile.LaunchProjectile(position, direction, speed, maxRange, source);
        }

        private float DirectionToAngle(Vector2 direction)
        {
            return (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
        }

        private float GetRandomSpreadAngle()
        {
            return UnityEngine.Random.Range(spread.Min, spread.Max);
        }

        private Vector2 ApplySpreadToDirection(Vector2 direction)
        {
            return Quaternion.Euler(0f, 0f, DirectionToAngle(direction) + GetRandomSpreadAngle()) * Vector2.up;
        }
    }
}
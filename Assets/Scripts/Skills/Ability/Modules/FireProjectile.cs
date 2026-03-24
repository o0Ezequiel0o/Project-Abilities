using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public abstract class FireProjectile : AbilityModule
    {
        [Header("Casting")]
        [SerializeField] protected float fireDistance;
        [SerializeField] protected Limits spread = Limits.Zero;

        protected Transform spawn;
        protected GameObject source;
        protected Ability ability;
        protected AbilityController controller;

        public FireProjectile(FireProjectile original)
        {
            fireDistance = original.fireDistance;
            spread = new Limits(original.spread.Min, original.spread.Max);
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.spawn = spawn;
            this.source = source;
            this.ability = ability;
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding) { }

        protected abstract void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source);

        protected float DirectionToAngle(Vector2 direction)
        {
            return (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
        }

        protected float GetRandomSpreadAngle()
        {
            return UnityEngine.Random.Range(spread.Min, spread.Max);
        }

        protected Vector2 ApplySpreadToDirection(Vector2 direction)
        {
            return Quaternion.Euler(0f, 0f, DirectionToAngle(direction) + GetRandomSpreadAngle()) * Vector2.up;
        }
    }
}
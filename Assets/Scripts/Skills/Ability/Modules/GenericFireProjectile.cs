using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class GenericFireProjectile<T> : FireProjectile where T : Projectile
    {
        [Header("Projectile")]
        [SerializeField] private T prefab;

        [Space]

        [SerializeField] protected Stat speed;
        [SerializeField] protected Stat damage;
        [SerializeField] protected Stat maxRange;

        private readonly GameObjectPool<T> projectilePool = new GameObjectPool<T>();

        public GenericFireProjectile(GenericFireProjectile<T> original) : base(original)
        {
            prefab = original.prefab;

            speed = original.speed.DeepCopy();
            damage = original.damage.DeepCopy();
            maxRange = original.maxRange.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new GenericFireProjectile<T>(this);

        public override void Activate(bool holding)
        {
            LaunchProjectile(spawn.position, spawn.up, damage.Value, speed.Value, maxRange.Value, source);
        }

        public override void Upgrade()
        {
            speed.Upgrade();
            damage.Upgrade();
            maxRange.Upgrade();
        }

        public override void Destroy()
        {
            projectilePool.Clear();
        }

        protected void LaunchProjectile(Vector3 position, Vector3 direction, GameObject source)
        {
            LaunchProjectile(position, direction, damage.Value, speed.Value, maxRange.Value, source);
        }

        protected override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source)
        {
            LaunchAndGetProjectile(position, direction, damage, speed, maxRange, source);
        }

        protected T LaunchAndGetProjectile(Vector3 position, Vector3 direction, GameObject source)
        {
            return LaunchAndGetProjectile(position, direction, damage.Value, speed.Value, maxRange.Value, source);
        }

        protected T LaunchAndGetProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source)
        {
            T projectile = projectilePool.Get(prefab);

            position += fireDistance * direction;
            direction = ApplySpreadToDirection(direction);

            projectile.Launch(position, speed, direction, maxRange, damage, source);

            projectile.gameObject.SetActive(true);
            return projectile;
        }
    }
}
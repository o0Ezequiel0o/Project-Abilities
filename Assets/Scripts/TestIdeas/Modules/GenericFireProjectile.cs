using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class GenericFireProjectile<T> : FireProjectile where T : Projectile
    {
        [SerializeField] private T prefab;

        private readonly GameObjectPool<T> projectilePool = new GameObjectPool<T>();

        public GenericFireProjectile(GenericFireProjectile<T> original) : base(original)
        {
            prefab = original.prefab;
        }

        public override AbilityModule CreateDeepCopy() => new GenericFireProjectile<T>(this);

        public override void Activate(bool holding)
        {
            LaunchProjectile(spawn.position, spawn.up, source);
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

        protected override void LaunchProjectile(Vector3 castWorldPosition, Vector3 castDirection, GameObject source)
        {
            Projectile projectile = projectilePool.Get(prefab);

            Vector3 spawnPosition = castWorldPosition + fireDistance * castDirection;
            projectile.Launch(spawnPosition, speed.Value, castDirection, maxRange.Value, damage.Value, source);

            projectile.gameObject.SetActive(true);
        }

        protected T LaunchAndGetProjectile(Vector3 castWorldPosition, Vector3 castDirection, GameObject source)
        {
            T projectile = projectilePool.Get(prefab);

            Vector3 spawnPosition = castWorldPosition + fireDistance * castDirection;
            projectile.Launch(spawnPosition, speed.Value, castDirection, maxRange.Value, damage.Value, source);

            projectile.gameObject.SetActive(true);
            return projectile;
        }
    }
}
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    public abstract class FireDamageProjectile<T> : FireProjectileType where T : DamageProjectileBase
    {
        [SerializeField] protected T prefab;
        [SerializeField] private Stat damage;

        protected readonly GameObjectPool<T> projectilePool = new GameObjectPool<T>();

        public FireDamageProjectile() { }

        public FireDamageProjectile(FireDamageProjectile<T> original)
        {
            prefab = original.prefab;
            damage = original.damage.DeepCopy();
        }

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float speed, float maxRange, GameObject source)
        {
            LaunchProjectile(position, direction, damage.Value, speed, maxRange, source, TeamManager.GetTeam(source));
        }

        public abstract void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team);

        public override void Upgrade()
        {
            damage.Upgrade();
        }

        public override void Destroy()
        {
            projectilePool.Clear();
        }
    }
}
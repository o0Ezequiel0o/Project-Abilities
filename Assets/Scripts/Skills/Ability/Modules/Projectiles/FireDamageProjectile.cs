using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;
using System;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public abstract class FireDamageProjectile<T> : FireProjectileType where T : DamageProjectileBase
    {
        private readonly FireDamageProjectileData<T> data;
        private readonly Stat damage;

        protected readonly GameObjectPool<T> projectilePool = new GameObjectPool<T>();

        public FireDamageProjectile(FireDamageProjectileData<T> data, Stat damage)
        {
            this.data = data;
            this.damage = damage;
        }

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float speed, float maxRange, GameObject source)
        {
            DamageData damageData = new DamageData(damage.Value, data.ArmorPenetration, data.ProcCoefficient);
            LaunchProjectile(position, direction, damageData, data.Knockback, speed, maxRange, source, TeamManager.GetTeam(source));
        }

        public abstract void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team);

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
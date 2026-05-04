using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;
using System;
using static DamageProjectileBase;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public abstract class FireDamageProjectile<T> : FireProjectileType where T : DamageProjectileBase
    {
        [SerializeField] protected T prefab;

        [Space]

        [SerializeField] private Stat damage;
        [SerializeField] private float armorPenetration = 0f;
        [SerializeField] private float procCoefficient = 1f;
        [SerializeField] private float knockback = 1f;

        protected readonly GameObjectPool<T> projectilePool = new GameObjectPool<T>();

        public FireDamageProjectile() { }

        public FireDamageProjectile(FireDamageProjectile<T> original)
        {
            prefab = original.prefab;

            armorPenetration = original.armorPenetration;
            procCoefficient = original.procCoefficient;

            damage = original.damage.DeepCopy();
        }

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float speed, float maxRange, GameObject source)
        {
            DamageData damageData = new DamageData(damage.Value, armorPenetration, procCoefficient);
            LaunchProjectile(position, direction, damageData, knockback, speed, maxRange, source, TeamManager.GetTeam(source));
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
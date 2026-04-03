using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    [Serializable]
    public class FireSniper : FireBasicProjectile
    {
        [SerializeField] private Stat doubleDamageChance;

        public FireSniper() { }

        public FireSniper(FireSniper original) : base(original)
        {
            doubleDamageChance = original.doubleDamageChance.DeepCopy();
        }

        public override FireProjectileType DeepCopy() => new FireSniper(this);

        public override void LaunchProjectile(Vector3 position, Vector3 direction, float damage, float speed, float maxRange, GameObject source, Teams team)
        {
            float randomNum = UnityEngine.Random.Range(0, 99);

            if (randomNum < doubleDamageChance.ValueInt)
            {
                damage *= 2;
            }

            base.LaunchProjectile(position, direction, damage, speed, maxRange, source, team);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            doubleDamageChance.Upgrade();
        }
    }
}
using UnityEngine;
using System;
using Zeke.TeamSystem;
using static DamageProjectileBase;

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

        public override void LaunchProjectile(Vector3 position, Vector3 direction, DamageData damageData, float knockback, float speed, float maxRange, GameObject source, Teams team)
        {
            float randomNum = UnityEngine.Random.Range(0, 99);

            if (randomNum < doubleDamageChance.ValueInt)
            {
                damageData = new DamageData(damageData.damage * 2f, damageData.armorPenetration, damageData.procCoefficient);
            }

            base.LaunchProjectile(position, direction, damageData, knockback, speed, maxRange, source, team);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            doubleDamageChance.Upgrade();
        }
    }
}
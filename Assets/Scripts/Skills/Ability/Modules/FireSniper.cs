using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireSniper : FireBasicProjectile
    {
        [SerializeField] private Stat doubleDamageChance;

        public FireSniper(FireSniper original) : base(original)
        {
            doubleDamageChance = original.doubleDamageChance.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new FireSniper(this);

        public override void Activate(bool holding)
        {
            float damage = this.damage.Value;

            float randomNum = UnityEngine.Random.Range(0, 99);

            if (randomNum < doubleDamageChance.ValueInt)
            {
                damage *= 2;
            }

            LaunchProjectile(spawn.position, spawn.up, damage, speed.Value, maxRange.Value, source);
        }

        public override void Upgrade()
        {
            base.Upgrade();
            doubleDamageChance.Upgrade();
        }
    }
}
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules.Projectiles
{
    public class FireSniper : FireBasicProjectile
    {
        private readonly FireSniperData data;
        private readonly Stat doubleDamageChance;

        public FireSniper(FireSniperData data, Stat damage, Stat doubleDamageChance) : base(data, damage)
        {
            this.data = data;
            this.doubleDamageChance = doubleDamageChance;
        }

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
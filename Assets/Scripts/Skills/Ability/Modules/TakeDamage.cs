using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class TakeDamage : AbilityModule
    {
        private readonly TakeDamageData data;

        private readonly Stat damage;

        private GameObject source;
        private Damageable damageable;

        private bool hasRequiredComponents = true;

        public TakeDamage(TakeDamageData data, Stat damage)
        {
            this.data = data;
            this.damage = damage;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
            if (!source.TryGetComponent(out damageable)) hasRequiredComponents = false;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (!hasRequiredComponents) return;
            float damageLocal = GetDamage(damage.Value, data.ValueType);

            DamageInfo damageInfo = new DamageInfo(damageLocal, data.ArmorPenetration, 0f) 
            { 
                hit = false, 
                lethal = data.Lethal,
                ignoresShield = data.IgnoresShield
            };
            damageable.DealDamage(damageInfo, source, source);
        }

        public override void Upgrade()
        {
            damage.Upgrade();
        }

        private float GetDamage(float amount, ValueType valueType)
        {
            return valueType switch
            {
                ValueType.Flat => amount,
                ValueType.Ratio => damageable.MaxHealth.Value * amount,
                _ => 0f,
            };
        }
    }
}
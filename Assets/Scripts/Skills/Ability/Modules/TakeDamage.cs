using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class TakeDamage : AbilityModule
    {
        [SerializeField] private Stat damage;
        [SerializeField] private ValueType valueType;
        [SerializeField] private float armorPenetration;

        [SerializeField] private bool lethal = true;
        [SerializeField] private bool ignoresShield = false;

        private GameObject source;
        private Damageable damageable;

        private bool hasRequiredComponents = true;

        public TakeDamage() { }

        public TakeDamage(TakeDamage original)
        {
            valueType = original.valueType;
            armorPenetration = original.armorPenetration;

            lethal = original.lethal;
            ignoresShield = original.ignoresShield;

            damage = original.damage.DeepCopy();
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
            if (!source.TryGetComponent(out damageable)) hasRequiredComponents = false;
        }

        public override AbilityModule DeepCopy() => new TakeDamage(this);

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (!hasRequiredComponents) return;
            float damageLocal = GetDamage(damage.Value, valueType);

            DamageInfo damageInfo = new DamageInfo(damageLocal, armorPenetration, 0f) 
            { 
                hit = false, 
                lethal = lethal,
                ignoresShield = ignoresShield
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
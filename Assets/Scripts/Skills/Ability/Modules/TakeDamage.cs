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

        private GameObject source;
        private Damageable damageable;

        private bool hasRequiredComponents = true;

        public TakeDamage() { }

        public TakeDamage(TakeDamage original)
        {
            valueType = original.valueType;
            damage = original.damage.DeepCopy();
            armorPenetration = original.armorPenetration;
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
            damageable.DealDamage(new DamageInfo(damageLocal, armorPenetration, 0f), source, source);
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
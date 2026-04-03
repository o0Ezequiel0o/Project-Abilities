using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public abstract class RechargeType
    {
        protected GameObject source;
        protected AbilityController controller;
        
        private Ability ability;

        public virtual void OnInitialization(AbilityController controller, GameObject source, Ability ability)
        {
            this.source = source;
            this.ability = ability;
            this.controller = controller;
        }

        public RechargeType() { }

        public abstract RechargeType DeepCopy();

        public virtual void Activate() { }
        public virtual void Deactivate() { }

        public abstract bool CanActivate();
        public abstract bool CanUpgrade();

        public virtual void UpdateDuration() { }

        public virtual void Destroy() { }
        public virtual void Upgrade() { }

        protected void UpdateCooldown(float value, ValueType valueType)
        {
            value = GetCooldownReductionAmount(value, valueType);
            ability.SetCooldownTimer(ability.CooldownTimer + value);
        }

        private float GetCooldownReductionAmount(float amount, ValueType valueType)
        {
            return valueType switch
            {
                ValueType.Flat => amount,
                ValueType.Ratio => ability.CooldownTime * amount,
                _ => 0f,
            };
        }
    }
}
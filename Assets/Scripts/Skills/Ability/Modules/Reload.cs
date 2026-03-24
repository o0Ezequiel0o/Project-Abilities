using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class Reload : AbilityModule
    {
        [SerializeReferenceDropdown, SerializeReference] private ReloadStrategy reload;

        private AbilityController controller;

        public Reload(Reload original)
        {
            reload = original.reload.CreateDeepCopy();
        }

        public override AbilityModule DeepCopy() => new Reload(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            reload.Activate(controller);
        }

        public override void Deactivate()
        {
            reload.Deactivate(controller);
        }

        public override void UpdateActive()
        {
            reload.UpdateActive(controller);
        }

        public override void UpdateUnactive()
        {
            reload.UpdateUnactive(controller);
        }

        public override void Upgrade()
        {
            reload.Upgrade(controller);
        }

        [Serializable]
        private abstract class ReloadStrategy
        {
            [SerializeReferenceDropdown, SerializeReference] protected GetAbilityStrategy strategy = new GetAbilityType();
            [SerializeField] protected Stat chargesAmount;

            public ReloadStrategy(ReloadStrategy original)
            {
                chargesAmount = original.chargesAmount.DeepCopy();
                strategy = original.strategy.GetDeepCopy();
            }

            public abstract ReloadStrategy CreateDeepCopy();

            public abstract void Activate(AbilityController controller);
            public abstract void Deactivate(AbilityController controller);
            public abstract void UpdateActive(AbilityController controller);
            public abstract void UpdateUnactive(AbilityController controller);

            public virtual void Upgrade(AbilityController controller)
            {
                chargesAmount.Upgrade();
            }
        }

        [Serializable]
        private class OnCast : ReloadStrategy
        {
            public OnCast(ReloadStrategy original) : base(original) { }
            public override ReloadStrategy CreateDeepCopy() => new OnCast(this);

            public override void Activate(AbilityController controller)
            {
                IAbility ability = strategy.GetAbility(controller);
                ability?.SetCharges(ability.Charges + chargesAmount.ValueInt);
            }

            public override void Deactivate(AbilityController controller) { }
            public override void UpdateActive(AbilityController controller) { }
            public override void UpdateUnactive(AbilityController controller) { }
        }

        [Serializable]
        private class OnDurationEnd : ReloadStrategy
        {
            public OnDurationEnd(ReloadStrategy original) : base(original) { }
            public override ReloadStrategy CreateDeepCopy() => new OnDurationEnd(this);

            public override void Activate(AbilityController controller) { }

            public override void Deactivate(AbilityController controller)
            {
                IAbility ability = strategy.GetAbility(controller);
                ability?.SetCharges(ability.Charges + chargesAmount.ValueInt);
            }

            public override void UpdateActive(AbilityController controller) { }
            public override void UpdateUnactive(AbilityController controller) { }
        }

        [Serializable]
        private class WhileActive : ReloadStrategy //TODO test
        {
            private float timePerCharge = 0f;
            private float timer = 0f;

            public WhileActive(ReloadStrategy original) : base(original) { }
            public override ReloadStrategy CreateDeepCopy() => new WhileActive(this);

            public override void Activate(AbilityController controller)
            {
                IAbility ability = strategy.GetAbility(controller);

                if (ability != null)
                {
                    timePerCharge = ability.DurationTime / ability.MaxCharges;
                }

                timer = 0f;
            }

            public override void Deactivate(AbilityController controller) { }

            public override void UpdateActive(AbilityController controller)
            {
                timer += Time.deltaTime;

                if (timer >= timePerCharge)
                {
                    IAbility ability = strategy.GetAbility(controller);
                    ability?.SetCharges(ability.Charges + chargesAmount.ValueInt);
                    timer = 0f;
                }
            }

            public override void UpdateUnactive(AbilityController controller) { }
        }
    }
}
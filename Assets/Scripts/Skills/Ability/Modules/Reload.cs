using UnityEngine;
using System;

using static Zeke.Abilities.Modules.ReloadData;

namespace Zeke.Abilities.Modules
{
    public class Reload : AbilityModule
    {
        private readonly ReloadData data;

        private readonly ReloadStrategy strategy;

        private AbilityController controller;

        public Reload(ReloadData data, ReloadStrategy strategy)
        {
            this.data = data;
            this.strategy = strategy;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.controller = controller;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            strategy.Activate(controller);
        }

        public override void Deactivate()
        {
            strategy.Deactivate(controller);
        }

        public override void UpdateActive()
        {
            strategy.UpdateActive(controller);
        }

        public override void UpdateInactive()
        {
            strategy.UpdateInactive(controller);
        }

        public override void Upgrade()
        {
            strategy.Upgrade(controller);
        }

        public abstract class ReloadStrategy
        {
            private readonly ReloadStrategyData data;

            protected readonly GetAbilityStrategy strategy;
            protected readonly Stat chargesAmount;

            public ReloadStrategy(ReloadStrategyData data, GetAbilityStrategy strategy, Stat chargesAmount)
            {
                this.data = data;
                this.strategy = strategy;
                this.chargesAmount = chargesAmount;
            }

            public abstract void Activate(AbilityController controller);
            public abstract void Deactivate(AbilityController controller);
            public abstract void UpdateActive(AbilityController controller);
            public abstract void UpdateInactive(AbilityController controller);

            public virtual void Upgrade(AbilityController controller)
            {
                chargesAmount.Upgrade();
            }
        }

        public class OnCast : ReloadStrategy
        {
            private readonly OnCastData data;

            public OnCast(OnCastData data, GetAbilityStrategy strategy, Stat chargesAmount) : base(data, strategy, chargesAmount)
            {
                this.data = data;
            }

            public override void Activate(AbilityController controller)
            {
                IAbility ability = strategy.GetAbility(controller);
                ability?.SetCharges(ability.Charges + chargesAmount.ValueInt);
            }

            public override void Deactivate(AbilityController controller) { }
            public override void UpdateActive(AbilityController controller) { }
            public override void UpdateInactive(AbilityController controller) { }
        }

        public class OnDurationEnd : ReloadStrategy
        {
            private readonly OnDurationEndData data;

            public OnDurationEnd(OnDurationEndData data, GetAbilityStrategy strategy, Stat chargesAmount) : base(data, strategy, chargesAmount)
            {
                this.data = data;
            }

            public override void Activate(AbilityController controller) { }

            public override void Deactivate(AbilityController controller)
            {
                IAbility ability = strategy.GetAbility(controller);
                ability?.SetCharges(ability.Charges + chargesAmount.ValueInt);
            }

            public override void UpdateActive(AbilityController controller) { }
            public override void UpdateInactive(AbilityController controller) { }
        }

        public class WhileActive : ReloadStrategy //TODO test
        {
            private readonly WhileActiveData data;

            public WhileActive(WhileActiveData data, GetAbilityStrategy strategy, Stat chargesAmount) : base(data, strategy, chargesAmount)
            {
                this.data = data;
            }

            private float timePerCharge = 0f;
            private float timer = 0f;

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

            public override void UpdateInactive(AbilityController controller) { }
        }
    }
}
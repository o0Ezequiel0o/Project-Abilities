using UnityEngine;

namespace Zeke.Abilities.Modules.Characters
{
    public class Overclock : AbilityModule
    {
        private readonly OverclockData data;

        private Ability ability;
        private GameObject source;
        private Damageable damageable;
        private AbilityController controller;

        private readonly Stat shieldUsageRatio;
        private readonly Stat baseRechargeRatioBoost;
        private readonly Stat rechargeRatioPerShield;

        private float timer = 0f;
        private int currentTick = 0;

        private bool disableNextTick = false;
        private bool hasRequiredComponents = false;

        private readonly Stat.Multiplier multiplier = new Stat.Multiplier(1f);

        public Overclock(OverclockData data, Stat shieldUsageRatio, Stat baseRechargeRatioBoost, Stat rechargeRatioPerShieldRatio)
        {
            this.data = data;
            this.shieldUsageRatio = shieldUsageRatio;
            this.baseRechargeRatioBoost = baseRechargeRatioBoost;
            this.rechargeRatioPerShield = rechargeRatioPerShieldRatio;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.ability = ability;
            this.source = source;
            this.controller = controller;

            if (!source.TryGetComponent(out damageable)) return;

            for (int i = 0; i < data.AffectedAbilities.Count; i++)
            {
                controller.GetAbilityTypeSlot(data.AffectedAbilities[i]).RechargeSpeed.AddMultiplier(multiplier);
            }

            hasRequiredComponents = true;
        }

        public override bool CanActivate() => true;

        public override void Activate(bool holding)
        {
            multiplier.UpdateMultiplier(1f);
            disableNextTick = false;

            currentTick = 0;
            timer = 1f;
        }

        public override void UpdateActive()
        {
            if (!hasRequiredComponents) return;

            if (disableNextTick || damageable.MaxShield.Value <= 0f)
            {
                ability.TryDeactivate();
                return;
            }

            timer += Time.deltaTime;

            if (timer >= data.TickDuration)
            {
                Tick();
                timer = 0f;
                currentTick += 1;
            }
        }

        public override void Deactivate()
        {
            multiplier.UpdateMultiplier(1f);
        }

        public override void Destroy()
        {
            for (int i = 0; i < data.AffectedAbilities.Count; i++)
            {
                controller.GetAbilityTypeSlot(data.AffectedAbilities[i]).RechargeSpeed.RemoveMultiplier(multiplier);
            }
        }

        public override bool CanUpgrade() => true;

        public override void Upgrade(int levels)
        {
            baseRechargeRatioBoost.Upgrade(levels);
            shieldUsageRatio.Upgrade(levels);
            rechargeRatioPerShield.Upgrade(levels);
        }

        private void Tick()
        {
            float currentShieldUsageRampUp = Mathf.Max(0f, currentTick - data.RampUpStartTick) * data.UsageRampUpRatio;
            float currentShieldUsageRatio = shieldUsageRatio.Value + currentShieldUsageRampUp;

            float targetShieldConsumption = damageable.MaxShield.Value * currentShieldUsageRatio;

            float shieldDamage = Mathf.Min(damageable.Shield, targetShieldConsumption);
            bool usedRemainingShield = Mathf.Approximately(shieldDamage, damageable.Shield);

            DamageInfo damageInfo = new DamageInfo(shieldDamage, data.Penetration, data.ProcCoefficient) { hit = false };
            damageable.DealDamage(damageInfo, source, source);

            float shieldConsumedRatio = shieldDamage / targetShieldConsumption;
            float consumptionRechargeBoost = shieldDamage * shieldConsumedRatio * rechargeRatioPerShield.Value;

            multiplier.UpdateMultiplier(1f + consumptionRechargeBoost);

            if (usedRemainingShield)
            {
                disableNextTick = true;
            }
        }
    }
}
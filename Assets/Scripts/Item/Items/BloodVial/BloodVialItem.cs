using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class BloodVialItem : Item
    {
        public override ItemData Data => data;
        private readonly BloodVialItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private readonly Stat.Multiplier multiplier = new Stat.Multiplier(1f);

        private float timer = 0f;

        private bool hasRequiredComponents = false;

        public BloodVialItem(BloodVialItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            hasRequiredComponents = source.TryGetComponent(out damageable);

            if (hasRequiredComponents)
            {
                damageable.onTakenDamage.Subscribe(OnTakenDamage);
                damageable.HealingReceivedMultiplier.AddMultiplier(multiplier);
            }
        }

        public override void OnUpdate()
        {
            if (!hasRequiredComponents) return;

            timer += Time.deltaTime;

            if (timer > data.Cooldown)
            {
                HealInfo heal = new HealInfo(data.Healing.GetValue(stacks), data.ProcCoefficient);
                damageable.GiveHealing(heal, source, source);

                timer = 0f;
            }
        }

        private void OnTakenDamage(DamageEvent damageEvent)
        {
            float missingHealthRatio = 1f - (damageable.Health / damageable.MaxHealth.Value);
            int effectStacks = Mathf.FloorToInt(missingHealthRatio / data.MissingHealthRatioForStack);

            multiplier.UpdateMultiplier(1f + (data.HealReceivedExtraMultPerStack.GetValue(stacks) * effectStacks));
        }
    }
}
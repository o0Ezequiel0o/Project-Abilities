using UnityEngine;

namespace Zeke.Items
{
    public class MagneticPlatingItem : Item
    {
        public override ItemData Data => data;
        private readonly MagneticPlatingItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private bool buffApplied = false;
        private bool hasRequiredComponents = false;

        private float shieldFlatModifier = 0f;
        private float armorFlatModifier = 0f;

        public MagneticPlatingItem(MagneticPlatingItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            hasRequiredComponents = source.TryGetComponent(out damageable);
        }

        public override void OnRemoved()
        {
            if (buffApplied)
            {
                RemoveArmorBuff();
            }
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateShield();

            if (buffApplied)
            {
                UpdateArmorBuff();
            }
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateShield();

            if (buffApplied)
            {
                UpdateArmorBuff();
            }
        }

        public override void OnUpdate()
        {
            if (!hasRequiredComponents) return;
            if (damageable.MaxShield.Value <= 0f) return;

            float shieldRatio = damageable.Shield / damageable.MaxShield.Value;

            if (damageable.MaxShield.Value > 0f && shieldRatio > data.ShieldRatioRequired)
            {
                if (!buffApplied)
                {
                    ApplyArmorBuff();
                }
            }
            else
            {
                if (buffApplied)
                {
                    RemoveArmorBuff();
                }
            }
        }

        private void ApplyArmorBuff()
        {
            float oldFlatModifier = armorFlatModifier;
            armorFlatModifier = data.ExtraArmor.GetValue(stacks);
            damageable.Armor.ApplyFlatModifier(-oldFlatModifier, armorFlatModifier);

            buffApplied = true;
        }

        private void RemoveArmorBuff()
        {
            damageable.Armor.ApplyFlatModifier(-data.ExtraArmor.GetValue(stacks));
            armorFlatModifier = 0f;

            buffApplied = false;
        }

        private void UpdateArmorBuff()
        {
            float oldFlatModifier = armorFlatModifier;
            armorFlatModifier = data.ExtraArmor.GetValue(stacks);
            damageable.Armor.ApplyFlatModifier(-oldFlatModifier, armorFlatModifier);
        }

        private void UpdateShield()
        {
            float oldFlatModifier = shieldFlatModifier;
            shieldFlatModifier = data.ExtraShield.GetValue(stacks);
            damageable.MaxShield.ApplyFlatModifier(-oldFlatModifier, shieldFlatModifier);
        }
    }
}
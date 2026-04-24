using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class LayeredArmorItem : Item
    {
        public override ItemData Data => data;
        private readonly LayeredArmorItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public LayeredArmorItem(LayeredArmorItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.onTakeDamage.Subscribe(OnTakeDamage, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.onTakeDamage.Unsubscribe(OnTakeDamage);
            }
        }

        private void OnTakeDamage(DamageEvent damageEvent)
        {
            if (damageEvent.SourceUser != null && damageEvent.SourceUser == source) return;
            damageEvent.FlatAccumulator -= data.FlatDamageReduction.GetValue(stacks);
        }
    }
}
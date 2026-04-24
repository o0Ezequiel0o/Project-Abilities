using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class ThornsItem : Item
    {
        public override ItemData Data => data;
        private readonly ThornsItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public ThornsItem(ThornsItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.onTakenDamage.Subscribe(OnTakenDamage, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out Damageable damageable))
            {
                damageable.onTakenDamage.Unsubscribe(OnTakenDamage);
            }
        }

        private void OnTakenDamage(DamageEvent damageEvent)
        {
            if (damageEvent.SourceUser != null && damageEvent.SourceUser == source) return;

            if (damageEvent.SourceUser.TryGetComponent(out StatusEffectHandler statusEffectHandler))
            {
                statusEffectHandler.ApplyEffect(data.Effect, source, Mathf.FloorToInt(data.Stacks.GetValue(stacks)));
            }
        }
    }
}
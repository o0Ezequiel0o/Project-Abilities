using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class BulletItem : Item
    {
        public override ItemData Data => data;
        private readonly BulletItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public BulletItem(BulletItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            DamageEvent.onHit.Subscribe(source, OnHit, data.TriggerOrder);
        }

        public override void OnRemoved()
        {
            DamageEvent.onHit.Unsubscribe(source, OnHit);
        }

        private void OnHit(DamageEvent damageEvent)
        {
            damageEvent.FlatAccumulator += data.FlatDamage.GetValue(stacks);
        }
    }
}
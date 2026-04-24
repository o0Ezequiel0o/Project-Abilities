using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class SniperScopeItem : Item
    {
        public override ItemData Data => data;
        private readonly SniperScopeItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public SniperScopeItem(SniperScopeItemData data, ItemHandler itemHandler, GameObject source)
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
            Vector3 closestPoint;

            if (damageEvent.Receiver.TryGetComponent(out Collider2D collider))
            {
                closestPoint = collider.ClosestPoint(source.transform.position);
            }
            else
            {
                closestPoint = damageEvent.Receiver.transform.position;
            }

            float distance = Vector2.Distance(source.transform.position, closestPoint);
            damageEvent.Multiplier.Multiply(data.DamageMultPerMeter.GetValue(stacks) * distance);
        }
    }
}
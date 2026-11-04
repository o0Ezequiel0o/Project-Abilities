using UnityEngine;

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
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnHit.Subscribe(OnHit, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (source.TryGetComponent(out DamageEventListener eventListener))
            {
                eventListener.OnHit.Unsubscribe(OnHit);
            }
        }

        private void OnHit(Damageable.DamageEvent.OnHitEvent hitEvent)
        {
            Damageable.DamageEvent damageEvent = hitEvent.damageEvent;

            if (damageEvent.Receiver.gameObject == source) return;

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
            damageEvent.Multiplier.Multiply(data.BaseDamageMultValue + (data.DamageMultIncreasePerMeter.GetValue(stacks) * distance));
        }
    }
}
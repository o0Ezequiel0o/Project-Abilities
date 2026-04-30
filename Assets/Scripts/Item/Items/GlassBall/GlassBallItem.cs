using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class GlassBallItem : Item
    {
        public override ItemData Data => data;
        private readonly GlassBallItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private GameObject visualInstance;

        private float flatModifier = 0f;
        private bool active = false;
        private float timer = 0f;

        public GlassBallItem(GlassBallItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            visualInstance = GameObject.Instantiate(data.ActiveVisual, source.transform);
            visualInstance.SetActive(false);

            if (source.TryGetComponent(out damageable))
            {
                damageable.onTakenDamage.Subscribe(OnTakenDamage, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (damageable != null)
            {
                damageable.onTakenDamage.Unsubscribe(OnTakenDamage);

                if (active)
                {
                    damageable.Armor.ApplyFlatModifier(-flatModifier);
                }
            }

            GameObject.Destroy(visualInstance);
        }

        public override void OnStacksAdded(int amount)
        {
            UpdateStacks();
        }

        public override void OnStacksRemoved(int amount)
        {
            UpdateStacks();
        }

        public override void OnUpdate()
        {
            if (active || damageable == null) return;

            timer += Time.deltaTime;

            if (timer > data.Cooldown)
            {
                Activate();
                timer = 0f;
            }
        }

        private void Activate()
        {
            flatModifier = data.Armor.GetValue(stacks);
            damageable.Armor.ApplyFlatModifier(flatModifier);

            visualInstance.SetActive(true);
            active = true;
        }

        private void Deactivate()
        {
            damageable.Armor.ApplyFlatModifier(-flatModifier);

            visualInstance.SetActive(false);
            active = false;
        }

        private void UpdateStacks()
        {
            if (!active) return;

            float oldModifier = flatModifier;
            flatModifier = data.Armor.GetValue(stacks);
            damageable.Armor.ApplyFlatModifier(-oldModifier, flatModifier);
        }

        private void OnTakenDamage(DamageEvent _)
        {
            if (!active) return;
            Deactivate();
        }
    }
}
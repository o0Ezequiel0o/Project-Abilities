using System.Collections.Generic;
using UnityEngine;
using static Damageable;

namespace Zeke.Items
{
    public class WarpClockItem : Item
    {
        public override ItemData Data => data;
        private readonly WarpClockItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Damageable damageable;

        private float tickTimer = 0f;
        private float cooldownTimer = 0f;

        private float currentTime = 0f;
        private float damageStored = 0f;

        private int currentTick = 0;
        private bool active = false;

        private bool hasRequiredComponents = false;

        private readonly List<DamageInstance> storedDamageInstances = new List<DamageInstance>();

        public WarpClockItem(WarpClockItemData data, ItemHandler itemHandler, GameObject source)
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
                damageable.onTakenDamage.Subscribe(OnTakenDamage, data.TriggerOrder);
            }
        }

        public override void OnRemoved()
        {
            if (hasRequiredComponents)
            {
                damageable.onTakenDamage.Unsubscribe(OnTakenDamage);
            }
        }

        public override void OnUpdate()
        {
            if (!hasRequiredComponents) return;

            currentTime += Time.deltaTime;

            if (active)
            {
                UpdateHealingState();
            }
            else
            {
                UpdateStoringState();
            }
        }

        private void OnTakenDamage(DamageEvent damageEvent)
        {
            if (active || !hasRequiredComponents) return;

            RemoveExpiredDamageInstances();
            cooldownTimer = 0f;

            storedDamageInstances.Add(new DamageInstance(damageEvent.Damage, currentTime));
        }

        private void RemoveExpiredDamageInstances()
        {
            for (int i = storedDamageInstances.Count - 1; i >= 0; i--)
            {
                float expireTime = storedDamageInstances[i].time + data.StoreTime;

                if (currentTime > expireTime)
                {
                    storedDamageInstances.RemoveAt(i);
                }
            }
        }

        private void StartHealingState()
        {
            active = true;

            for (int i = 0; i < storedDamageInstances.Count; i++)
            {
                damageStored += storedDamageInstances[i].damage;
            }

            storedDamageInstances.Clear();
        }

        private void StopHealingState()
        {
            active = false;
            currentTick = 0;
            damageStored = 0f;
            cooldownTimer = 0f;
        }

        private void UpdateHealingState()
        {
            tickTimer += Time.deltaTime;

            if (tickTimer > data.HealTickTime)
            {
                float healing = damageStored * data.DamageHealRatio.GetValue(stacks);
                damageable.GiveHealing(healing, source, source);

                tickTimer = 0f;
                currentTick += 1;
            }

            if (currentTick >= data.HealTicks)
            {
                StopHealingState();
            }
        }

        private void UpdateStoringState()
        {
            if (cooldownTimer >= data.TriggerTimeRequired)
            {
                StartHealingState();
            }
            else if (storedDamageInstances.Count > 0)
            {
                cooldownTimer += Time.deltaTime;
            }
        }

        private readonly struct DamageInstance
        {
            public readonly float damage;
            public readonly float time;

            public DamageInstance(float damage, float time)
            {
                this.damage = damage;
                this.time = time;
            }
        }
    }
}
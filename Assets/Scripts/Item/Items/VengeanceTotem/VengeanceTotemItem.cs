using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Items
{
    public class VengeanceTotemItem : Item
    {
        public override ItemData Data => data;
        private readonly VengeanceTotemItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        public VengeanceTotemItem(VengeanceTotemItemData data, ItemHandler itemHandler, GameObject source)
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
                damageable.onTakeDamage.Unsubscribe(OnTakenDamage);
            }
        }

        private void OnTakenDamage(Damageable.DamageEvent damageEvent)
        {
            if (damageEvent.SourceUser == source) return;
            if (damageEvent.ProcChainBranch.Contains(Data)) return;

            if (damageEvent.SourceUser.TryGetComponent(out Damageable damageable))
            {
                List<ItemData> newProcChainBranch = new List<ItemData>(damageEvent.ProcChainBranch) { Data };

                DamageInfo damageInfo = new DamageInfo(data.Damage.GetValue(stacks), data.ProcCoefficient, data.ArmorPenetration)
                {
                    direction = (damageEvent.SourceUser.transform.position - source.transform.position).normalized
                };

                damageable.DealDamage(damageInfo, source, source, newProcChainBranch);
            }
        }
    }
}
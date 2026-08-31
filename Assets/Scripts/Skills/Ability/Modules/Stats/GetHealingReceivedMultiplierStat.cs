using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetHealingReceivedMultiplierStat : GetStatStrategy
    {
        private readonly GetHealingReceivedMultiplierStatData data;

        public GetHealingReceivedMultiplierStat(GetHealingReceivedMultiplierStatData data)
        {
            this.data = data;
        }

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out Damageable damageable))
            {
                stat = damageable.HealingReceivedMultiplier;
            }

            return stat;
        }
    }
}
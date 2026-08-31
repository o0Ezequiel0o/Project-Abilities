using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetDamageReceivedMultiplierStat : GetStatStrategy
    {
        private readonly GetDamageReceivedMultiplierStatData data;

        public GetDamageReceivedMultiplierStat(GetDamageReceivedMultiplierStatData data)
        {
            this.data = data;
        }

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out Damageable damageable))
            {
                stat = damageable.DamageReceivedMultiplier;
            }

            return stat;
        }
    }
}
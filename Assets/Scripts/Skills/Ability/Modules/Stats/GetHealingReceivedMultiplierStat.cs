using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetHealingReceivedMultiplierStat : GetStatStrategy
    {
        public GetHealingReceivedMultiplierStat() { }

        public override GetStatStrategy DeepCopy() => new GetHealingReceivedMultiplierStat();

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
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GetDamageReceivedMultiplierStat : GetStatStrategy
    {
        public GetDamageReceivedMultiplierStat() { }

        public override GetStatStrategy DeepCopy() => new GetDamageReceivedMultiplierStat();

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
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetMaxHealthStat : GetStatStrategy
    {
        public GetMaxHealthStat() { }

        public override GetStatStrategy DeepCopy() => new GetMaxHealthStat();

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out Damageable damageable))
            {
                stat = damageable.MaxHealth;
            }

            return stat;
        }
    }
}
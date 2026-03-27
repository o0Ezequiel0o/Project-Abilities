using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GetArmorStat : GetStatStrategy
    {
        public GetArmorStat() { }

        public override GetStatStrategy DeepCopy() => new GetArmorStat();

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out Damageable damageable))
            {
                stat = damageable.Armor;
            }

            return stat;
        }
    }
}
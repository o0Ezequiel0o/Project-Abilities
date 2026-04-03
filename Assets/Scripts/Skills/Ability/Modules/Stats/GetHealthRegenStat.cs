using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetHealthRegenStat : GetStatStrategy
    {
        public GetHealthRegenStat() { }

        public override GetStatStrategy DeepCopy() => new GetHealthRegenStat();

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out Damageable damageable))
            {
                stat = damageable.HealthRegen;
            }

            return stat;
        }
    }
}
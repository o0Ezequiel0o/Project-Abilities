using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
    public class GetHealthRegenStat : GetStatStrategy
    {
        private readonly GetHealthRegenStatData data;

        public GetHealthRegenStat(GetHealthRegenStatData data)
        {
            this.data = data;
        }

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
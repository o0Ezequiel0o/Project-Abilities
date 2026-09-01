using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
    public class GetMaxHealthStat : GetStatStrategy
    {
        private readonly GetMaxHealthStatData data;

        public GetMaxHealthStat(GetMaxHealthStatData data)
        {
            this.data = data;
        }

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
using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
    public class GetArmorStat : GetStatStrategy
    {
        private readonly GetArmorStatData data;

        public GetArmorStat(GetArmorStatData data)
        {
            this.data = data;
        }

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
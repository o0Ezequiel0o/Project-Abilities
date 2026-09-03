using UnityEngine;

namespace Zeke.Abilities.Modules.Stats
{
    public class GetCooldownMultiplierStat : GetStatStrategy
    {
        private readonly GetCooldownMultiplierStatData data;

        public GetCooldownMultiplierStat(GetCooldownMultiplierStatData data)
        {
            this.data = data;
        }

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out AbilityController abilityController))
            {
                stat = abilityController.Abilities[data.AbilityType].CooldownMultiplier;
            }

            return stat;
        }
    }
}
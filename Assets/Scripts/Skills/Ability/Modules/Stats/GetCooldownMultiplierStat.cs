using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
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
                stat = abilityController.cooldownMultiplier[data.AbilityType];
            }

            return stat;
        }
    }
}
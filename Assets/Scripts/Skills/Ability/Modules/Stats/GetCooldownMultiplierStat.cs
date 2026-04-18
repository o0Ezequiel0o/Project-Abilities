using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetCooldownMultiplierStat : GetStatStrategy
    {
        [SerializeField] private AbilityType abilityType;

        public GetCooldownMultiplierStat() { }

        public GetCooldownMultiplierStat(GetCooldownMultiplierStat original)
        {
            abilityType = original.abilityType;
        }

        public override GetStatStrategy DeepCopy() => new GetCooldownMultiplierStat(this);

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out AbilityController abilityController))
            {
                stat = abilityController.cooldownMultiplier[abilityType];
            }

            return stat;
        }
    }
}
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetAbilityCooldownMultiplierStat : GetStatStrategy
    {
        [SerializeField] private AbilityType abilityType;

        public GetAbilityCooldownMultiplierStat() { }

        public GetAbilityCooldownMultiplierStat(GetAbilityCooldownMultiplierStat original)
        {
            abilityType = original.abilityType;
        }

        public override GetStatStrategy DeepCopy() => new GetAbilityCooldownMultiplierStat(this);

        public override Stat GetStat(GameObject source)
        {
            Stat stat = null;

            if (source.TryGetComponent(out AbilityController abilityController))
            {
                stat = abilityController.abilityCooldownMultiplier[abilityType];
            }

            return stat;
        }
    }
}

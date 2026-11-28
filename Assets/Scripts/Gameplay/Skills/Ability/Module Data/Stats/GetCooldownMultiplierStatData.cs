using UnityEngine;
using System;

namespace Zeke.Abilities.Modules.Stats
{
    [Serializable]
    public class GetCooldownMultiplierStatData : GetStatStrategyData
    {
        [field: SerializeField] public AbilityType AbilityType { get; private set; }

        public override GetStatStrategy CreateStatStrategy()
        {
            return new GetCooldownMultiplierStat(this);
        }
    }
}
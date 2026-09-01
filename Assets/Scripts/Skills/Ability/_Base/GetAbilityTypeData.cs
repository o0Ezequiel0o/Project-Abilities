using UnityEngine;
using System;

namespace Zeke.Abilities
{
    [Serializable]
    public class GetAbilityTypeData : GetAbilityStrategyData
    {
        [field: SerializeField] public AbilityType Type { get; private set; }

        public override GetAbilityStrategy CreateStrategy()
        {
            return new GetAbilityType(this);
        }
    }
}
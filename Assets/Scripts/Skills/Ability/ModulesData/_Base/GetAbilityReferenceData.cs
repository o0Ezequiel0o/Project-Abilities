using UnityEngine;
using System;

namespace Zeke.Abilities
{
    [Serializable]
    public class GetAbilityReferenceData : GetAbilityStrategyData
    {
        [field: SerializeField] public AbilityData Reference { get; private set; }

        public override GetAbilityStrategy CreateStrategy()
        {
            return new GetAbilityReference(this);
        }
    }
}
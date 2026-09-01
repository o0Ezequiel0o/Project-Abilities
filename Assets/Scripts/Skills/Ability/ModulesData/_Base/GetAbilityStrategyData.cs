using System;

namespace Zeke.Abilities
{
    [Serializable]
    public abstract class GetAbilityStrategyData
    {
        public abstract GetAbilityStrategy CreateStrategy();
    }
}
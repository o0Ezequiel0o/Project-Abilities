using System;

namespace Zeke.Abilities
{
    [Serializable]
    public abstract class GetAbilityStrategy
    {
        public abstract IAbility GetAbility(AbilityController controller);

        public abstract GetAbilityStrategy GetDeepCopy();
    }
}
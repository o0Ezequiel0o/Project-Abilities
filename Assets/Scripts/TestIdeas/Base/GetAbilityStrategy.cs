using System;

namespace Zeke.Abilities
{
    [Serializable]
    public abstract class GetAbilityStrategy
    {
        public abstract IModularAbility GetAbility(ModularAbilityController controller);

        public abstract GetAbilityStrategy GetDeepCopy();
    }
}
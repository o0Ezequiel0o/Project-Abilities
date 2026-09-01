using System;

namespace Zeke.Abilities
{
    [Serializable]
    public class GetAbilityType : GetAbilityStrategy
    {
        private readonly GetAbilityTypeData data;

        public GetAbilityType(GetAbilityTypeData data)
        {
            this.data = data;
        }

        public override IAbility GetAbility(AbilityController controller)
        {
            if (controller.TryGetAbility(data.Type, out IAbility ability))
            {
                return ability;
            }

            return null;
        }
    }
}
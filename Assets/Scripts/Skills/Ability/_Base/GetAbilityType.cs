using static Zeke.Abilities.AbilityController;

namespace Zeke.Abilities
{
    public class GetAbilityType : GetAbilityStrategy
    {
        private readonly GetAbilityTypeData data;

        public GetAbilityType(GetAbilityTypeData data)
        {
            this.data = data;
        }

        public override AbilitySlot GetAbilitySlot(AbilityController controller)
        {
            if (controller.TryGetAbility(data.Type, out IAbility ability))
            {
                return controller.GetAbilitySlot(ability.Type);
            }

            return null;
        }
    }
}
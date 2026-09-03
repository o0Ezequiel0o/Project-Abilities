using static Zeke.Abilities.AbilityController;

namespace Zeke.Abilities
{
    public class GetAbilityReference : GetAbilityStrategy
    {
        private readonly GetAbilityReferenceData data;

        public GetAbilityReference(GetAbilityReferenceData data)
        {
            this.data = data;
        }

        public override AbilitySlot GetAbilitySlot(AbilityController controller)
        {
            if (controller.TryGetAbility(data.Reference, out IAbility ability))
            {
                if (ability.Data == data.Reference)
                {
                    return controller.GetAbilitySlot(ability.Type);
                }
            }

            return null;
        }
    }
}
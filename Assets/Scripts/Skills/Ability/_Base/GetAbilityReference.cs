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

        public override bool TryGetAbilitySlot(AbilityController controller, out AbilitySlotInfo abilitySlotInfo)
        {
            abilitySlotInfo = default;

            if (controller.TryGetAbility(data.Reference, out IAbility ability))
            {
                AbilityTypeSlot abilityTypeSlot = controller.GetAbilityTypeSlot(ability.Type);

                for (int i = 0; i < abilityTypeSlot.Abilities.Count; i++)
                {
                    if (abilityTypeSlot.Abilities[i].Data == data.Reference)
                    {
                        abilitySlotInfo = new AbilitySlotInfo(abilityTypeSlot, i);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
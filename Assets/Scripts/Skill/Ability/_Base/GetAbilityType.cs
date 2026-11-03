namespace Zeke.Abilities
{
    public class GetAbilityType : GetAbilityStrategy
    {
        private readonly GetAbilityTypeData data;

        public GetAbilityType(GetAbilityTypeData data)
        {
            this.data = data;
        }

        public override bool TryGetAbilitySlot(AbilityController controller, out AbilitySlotInfo abilitySlotInfo)
        {
            abilitySlotInfo = default;

            AbilityController.AbilityTypeSlot abilityTypeSlot = controller.GetAbilityTypeSlot(data.Type);

            if (abilityTypeSlot.CurrentAbilitySlot != null)
            {
                abilitySlotInfo = new AbilitySlotInfo(abilityTypeSlot, 0);
                return true;
            }

            return false;
        }
    }
}
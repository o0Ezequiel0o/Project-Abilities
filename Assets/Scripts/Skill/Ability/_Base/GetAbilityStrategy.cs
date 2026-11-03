using static Zeke.Abilities.AbilityController;

namespace Zeke.Abilities
{
    public abstract class GetAbilityStrategy
    {
        public abstract bool TryGetAbilitySlot(AbilityController controller, out AbilitySlotInfo abilitySlotInfo);
    }

    public readonly struct AbilitySlotInfo
    {
        public readonly AbilityTypeSlot abilityTypeSlot;
        public readonly int abilityIndex;

        public AbilitySlotInfo(AbilityTypeSlot abilityTypeSlot, int abilityIndex)
        {
            this.abilityTypeSlot = abilityTypeSlot;
            this.abilityIndex = abilityIndex;
        }
    }
}
using static Zeke.Abilities.AbilityController;

namespace Zeke.Abilities
{
    public abstract class GetAbilityStrategy
    {
        public abstract AbilitySlot GetAbilitySlot(AbilityController controller);
    }
}
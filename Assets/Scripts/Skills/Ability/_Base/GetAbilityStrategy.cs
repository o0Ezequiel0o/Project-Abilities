namespace Zeke.Abilities
{
    public abstract class GetAbilityStrategy
    {
        public abstract IAbility GetAbility(AbilityController controller);
    }
}
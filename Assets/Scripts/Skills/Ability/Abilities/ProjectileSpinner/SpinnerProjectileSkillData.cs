using UnityEngine;

[CreateAssetMenu(fileName = "Basic Spinner Projectiles", menuName = "ScriptableObjects/Abilities/BasicSpinnerProjectiles", order = 1)]
public class SpinnerProjectileSkillData : SpinnerBaseSkillData
{
    [SerializeField] private Stat damage;

    private Stat Damage => damage;

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new SpinnerProjectileSkill(source, controller, this, CooldownTime, Duration, Distance, Speed, Amount, Damage);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Fire Basic Projectile", menuName = "ScriptableObjects/Abilities/FireBasicProjectile", order = 1)]
public class BasicProjectileSkillData : ProjectileSkillBaseData
{
    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new BasicProjectileSkill(source, controller, this, CooldownTime);
    }
}
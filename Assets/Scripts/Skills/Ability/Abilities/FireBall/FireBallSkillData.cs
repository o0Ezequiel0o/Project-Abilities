using UnityEngine;

[CreateAssetMenu(fileName = "Fire Ball", menuName = "ScriptableObjects/Abilities/FireBall", order = 1)]
public class FireBallSkillData : ProjectileSkillBaseData
{
    [Header("Fireball Stats")]
    [SerializeField] private Stat damageRadius;

    private Stat DamageRadius => damageRadius.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new FireBallSkill(source, controller, this, CooldownTime, DamageRadius);
    }
}
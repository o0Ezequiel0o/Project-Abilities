using UnityEngine;

[CreateAssetMenu(fileName = "Giant Orb", menuName = "ScriptableObjects/Abilities/GiantOrb", order = 1)]
public class GiantOrbSkillData : ProjectileSkillBaseData
{
    [Header("Giant Orb Homing Orbs Stats")]
    [SerializeField] private Stat homingOrbDamage;
    [SerializeField] private Stat homingOrbSpeed;
    [SerializeField] private Stat homingOrbRange;

    private Stat HomingOrbDamage => homingOrbDamage.DeepCopy();
    private Stat HomingOrbSpeed => homingOrbSpeed.DeepCopy();
    private Stat HomingOrbRange => homingOrbRange.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new GiantOrbSkill(source, controller, this, CooldownTime, HomingOrbDamage, HomingOrbSpeed, HomingOrbRange);
    }
}
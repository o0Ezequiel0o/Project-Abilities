using UnityEngine;

[CreateAssetMenu(fileName = "Lighting Bolt", menuName = "ScriptableObjects/Abilities/LightingBolt", order = 1)]
public class LightingBoltSkillData : ProjectileSkillBaseData
{
    [Header("Lighting Bolt Stats")]
    [SerializeField] private Stat spreadTargets;

    private Stat SpreadTargets => spreadTargets.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new LightingBoltSkill(source, controller, this, CooldownTime, SpreadTargets);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Sprint", menuName = "ScriptableObjects/Abilities/Sprint", order = 1)]
public class SprintSkillData : AbilityData
{
    [Space]
    [SerializeField] private Stat effectDuration;
    [SerializeField] private Stat extraSpeed;

    private Stat EffectDuration => effectDuration.DeepCopy();
    private Stat ExtraSpeed => extraSpeed.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new SprintSkill(source, controller, this, CooldownTime, EffectDuration, ExtraSpeed);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "ScriptableObjects/Abilities/Dash", order = 1)]
public class DashSkillData : AbilityData
{
    [field: SerializeField] public float Force { get; private set; }
    [field: SerializeField] public float DashDuration { get; private set; }

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new DashSkill(source, controller, this, CooldownTime);
    }
}
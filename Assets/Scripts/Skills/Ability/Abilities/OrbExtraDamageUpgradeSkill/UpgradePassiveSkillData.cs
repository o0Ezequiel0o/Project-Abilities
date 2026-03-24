using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Passive", menuName = "ScriptableObjects/Abilities/UpgradeSkill", order = 1)]
public class UpgradePassiveSkillData : AbilityData
{
    [field: SerializeField] public PassiveData Passive { get; private set; }
    [field: SerializeField] public int UpgradeTimes { get; private set; } = 1;

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new UpgradePassiveSkill(source, controller, this, CooldownTime);
    }
}
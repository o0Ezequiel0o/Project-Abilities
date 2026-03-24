using UnityEngine;

[CreateAssetMenu(fileName = "Reload", menuName = "ScriptableObjects/Abilities/Reload", order = 1)]
public class ReloadSkillData : AbilityData
{
    [field: SerializeField] public AbilityType AbilityToRecharge { get; private set; }
    [SerializeField] private Stat rechargeAmount;
    [SerializeField] private Stat rechargeTime;

    public Stat RechargeAmount => rechargeAmount.DeepCopy();
    public Stat RechargeTime => rechargeTime.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new ReloadSkill(source, controller, this, CooldownTime, RechargeAmount, RechargeTime);
    }
}
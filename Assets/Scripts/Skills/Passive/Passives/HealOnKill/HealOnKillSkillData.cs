using UnityEngine;

[CreateAssetMenu(fileName = "Heal On Kill", menuName = "ScriptableObjects/Passives/HealOnKill", order = 1)]
public class HealOnKillSkillData : PassiveData
{
    [SerializeField] private Stat healAmount;

    private Stat HealAmount => healAmount.DeepCopy();

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new HealOnKillSkill(source, passiveController, this, HealAmount);
    }
}
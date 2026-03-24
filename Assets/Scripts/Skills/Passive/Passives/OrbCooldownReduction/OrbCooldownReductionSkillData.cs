using UnityEngine;

[CreateAssetMenu(fileName = "Orb Cooldown Reduction", menuName = "ScriptableObjects/Passives/OrbCooldownReduction", order = 1)]
public class OrbCooldownReductionSkillData : PassiveData
{
    [SerializeField] private Stat flatCooldownReduction;

    private Stat FlatCooldownReduction => flatCooldownReduction.DeepCopy();

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new OrbCooldownReductionSkill(source, passiveController, this, FlatCooldownReduction);
    }
}
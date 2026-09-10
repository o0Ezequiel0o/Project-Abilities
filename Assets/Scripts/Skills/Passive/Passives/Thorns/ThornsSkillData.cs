using UnityEngine;

[CreateAssetMenu(fileName = "Thorns", menuName = "ScriptableObjects/Passives/Thorns", order = 1)]
public class ThornsSkillData : PassiveData
{
    [SerializeField] private Stat damagePerArmor;
    [field: SerializeField] public float MaxRange { get; private set; } = 2f;

    [field: Space]

    [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
    [field: SerializeField] public float ProcCoefficient { get; private set; } = 0f;

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new ThornsSkill(source, passiveController, this, damagePerArmor.DeepCopy());
    }
}
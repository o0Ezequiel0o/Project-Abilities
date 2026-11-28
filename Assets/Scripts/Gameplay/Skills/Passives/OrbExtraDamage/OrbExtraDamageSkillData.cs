using UnityEngine;

[CreateAssetMenu(fileName = "Orb Extra Damage", menuName = "ScriptableObjects/Passives/OrbExtraDamage", order = 1)]
public class OrbExtraDamageSkillData : PassiveData
{
    [Space]

    [SerializeField] private Stat extraDamage;
    [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
    [field: SerializeField] public float ProcCoefficient { get; private set; } = 0f;

    private Stat ExtraDamage => extraDamage.DeepCopy();

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new OrbExtraDamageSkill(source, passiveController, this, ExtraDamage);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Defense System", menuName = "ScriptableObjects/Passives/DefenseSystem", order = 1)]
public class DefenseSystemSkillData : PassiveData
{
    [field: Space]

    [field: SerializeField] public GameObject AreaVisual { get; private set; }

    [field: Space]

    [field: SerializeField] public LayerMask HitLayers { get; private set; }
    [field: SerializeField] public float DamageInterval { get; private set; }

    [field: Space]

    [field: SerializeField] public float MaxShieldDamageRatio { get; private set; }
    [field: SerializeField] public float MinDamage { get; private set; } = 1f;
    [SerializeField] private Stat radius;

    [field: Space]

    [field: SerializeField] public float ArmorPenetration { get; private set; } = 0f;
    [field: SerializeField] public float ProcCoefficient { get; private set; } = 1f;

    private Stat Radius => radius.DeepCopy();

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new DefenseSystemSkill(source, passiveController, this, Radius);
    }
}
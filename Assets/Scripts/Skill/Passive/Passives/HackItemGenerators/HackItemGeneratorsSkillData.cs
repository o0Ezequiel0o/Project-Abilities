using UnityEngine;

[CreateAssetMenu(fileName = "Hack Item Generators", menuName = "ScriptableObjects/Passives/HackItemGenerators", order = 1)]
public class HackItemGeneratorsSkillData : PassiveData
{
    [field: Space]

    [field: SerializeField] public float Radius { get; private set; } = 2f;
    [field: SerializeField] public LayerMask CheckLayers { get; private set; }
    [field: SerializeField] public float TimeRequired { get; private set; } = 15f;

    [Space]

    [SerializeField] private Stat chargeSpeed;

    [field: Header("Visual")]
    [field: SerializeField] public StatusBar ProgressBarPrefab { get; private set; }

    private Stat ChargeSpeed => chargeSpeed.DeepCopy();

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new HackItemGeneratorsSkill(source, passiveController, this, ChargeSpeed);
    }
}
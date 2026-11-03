using UnityEngine;

[CreateAssetMenu(fileName = "Ambush", menuName = "ScriptableObjects/Passives/Ambush", order = 1)]
public class AmbushSkillData : PassiveData
{
    [Space]

    [SerializeField] private Stat damageMultiplier;
    [field: SerializeField] public float TimeToActivate { get; private set; }

    [field: Header("Visual")]
    [field: SerializeField] public StatusEffectData ActiveIndicator { get; private set; }

    private Stat DamageMultiplier => damageMultiplier.DeepCopy();

    public override IPassive CreatePassive(GameObject source, PassiveController passiveController)
    {
        return new AmbushSkill(source, passiveController, this, DamageMultiplier);
    }
}
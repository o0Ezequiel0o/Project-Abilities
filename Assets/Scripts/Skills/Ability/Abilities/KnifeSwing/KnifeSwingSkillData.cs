using UnityEngine;

[CreateAssetMenu(fileName = "Swing Attack", menuName = "ScriptableObjects/Abilities/SwingAttack", order = 1)]
public class KnifeSwingSkillData : BoxAttackSkillData
{
    [field: Header("Attack")]
    [field: SerializeField] public int Charges { get; private set; }
    [SerializeField] private Stat swingCooldown;
    [SerializeField] private Stat extraDamage;
    [field: SerializeField] public float ExtraDamageProcCoefficient { get; private set; }
    [field: SerializeField] public StatusEffectData EffectToApply { get; private set; }

    private Stat SwingCooldown => swingCooldown.DeepCopy();
    private Stat ExtraDamage => extraDamage.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new KnifeSwingSkill(source, controller, this, CooldownTime, Damage, SwingCooldown, ExtraDamage);
    }
}
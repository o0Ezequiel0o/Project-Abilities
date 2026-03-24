using UnityEngine;

[CreateAssetMenu(fileName = "BoxAttackSkill", menuName = "ScriptableObjects/Abilities/BoxAttackSkill", order = 1)]
public class BoxAttackSkillData : AbilityData
{
    [field: Header("Zombie Atttack Stats")]
    [SerializeField] private Stat damage;
    [field: SerializeField] public float ProcCoefficient { get; private set; }
    [field: SerializeField] public float ArmorPenetration { get; private set; }
    [field: SerializeField] public float Knockback { get; private set; }

    [field: Space]

    [field: SerializeField] public LayerMask HitLayers { get; private set; }
    [field: SerializeField] public LayerMask BlockLayers { get; private set; }
    [field: SerializeField] public Vector2 Range { get; private set; }

    [field: Header("Visuals")]
    [field: SerializeField] public GameObject AttackEffect { get; private set; }

    [field: Space]

    [field: SerializeField] public float AttackEffectDuration { get; private set; }
    [field: SerializeField] public float AttackEffectCenterDistance { get; private set; }
    [field: SerializeField] public float AttackEffectRandomRotationOffset { get; private set; }

    protected Stat Damage => damage.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new BoxAttackSkill(source, controller, this, CooldownTime, Damage);
    }
}
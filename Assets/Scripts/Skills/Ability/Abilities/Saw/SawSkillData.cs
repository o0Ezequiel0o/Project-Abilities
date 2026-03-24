using UnityEngine;

[CreateAssetMenu(fileName = "Saw", menuName = "ScriptableObjects/Abilities/Saw", order = 1)]
public class SawSkillData : AbilityData
{
    [field: SerializeField] public float DamageRadius { get; private set; }
    [field: SerializeField] public float CastDistanceAway { get; private set; }

    [field: Space]

    [field: SerializeField] public LayerMask HitLayers { get; private set; }
    [field: SerializeField] public LayerMask BlockLayers { get; private set; }

    [field: Header("Hit Settings")]

    [SerializeField] private Stat damage;
    [SerializeField] private Stat damageCooldown;

    [field: Space]

    [field: SerializeField] public float ArmorPenetration { get; private set; }
    [field: SerializeField] public float ProcCoefficient { get; private set; }

    [field: Space]

    [field: SerializeField] public StatusEffectData StatusEffectToApply { get; private set; }
    [field: SerializeField] public int StatusEffectProcChance { get; private set; }

    [field: Header("Visual")]
    [field: SerializeField] public GameObject SawPrefab { get; private set; }

    private Stat Damage => damage.DeepCopy();
    private Stat DamageCooldown => damageCooldown.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new SawSkill(source, controller, this, CooldownTime, Damage, DamageCooldown);
    }
}
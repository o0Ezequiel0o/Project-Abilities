using UnityEngine;

[CreateAssetMenu(fileName = "Explode", menuName = "ScriptableObjects/Abilities/Explode", order = 1)]
public class ExplodeSkillData : AbilityData
{
    [Header("Stats")]
    [SerializeField] private Stat damage;
    [SerializeField] private Stat radius;

    [field: Space]

    [field: SerializeField] public LayerMask HitLayers { get; private set; }
    [field: SerializeField] public float Knockback { get; private set; }

    private Stat Damage => damage.DeepCopy();
    private Stat Radius => radius.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new ExplodeSkill(source, controller, this, CooldownTime, Radius, Damage);
    }
}
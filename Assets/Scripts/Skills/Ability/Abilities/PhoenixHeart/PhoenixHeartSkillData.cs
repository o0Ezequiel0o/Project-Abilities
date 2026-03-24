using UnityEngine;

[CreateAssetMenu(fileName = "Phoenix Heart", menuName = "ScriptableObjects/Abilities/PhoenixHeart", order = 1)]
public class PhoenixHeartSkillData : AbilityData
{
    [field: Header("Phoenix Heart")]
    [field: SerializeField] public LayerMask HitLayer { get; private set; }
    [SerializeField] private Stat hitRadius;
    [field: Space]
    [field: SerializeField] public StatusEffectData EffectToConsume { get; private set; }
    [SerializeField] private Stat healingPerStack;

    private Stat HitRadius => hitRadius.DeepCopy();
    private Stat HealingPerStack => healingPerStack.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new PhoenixHeartSkill(source, controller, this, CooldownTime, HitRadius, HealingPerStack);
    }
}
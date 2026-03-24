using UnityEngine;

[CreateAssetMenu(fileName = "Sniper", menuName = "ScriptableObjects/Abilities/Sniper", order = 1)]
public class SniperSkillData : AbilityData
{
    [field: Space]
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float CastDistanceAway { get; private set; }

    [Space]

    [SerializeField] private Stat speed;
    [SerializeField] private Stat damage;
    [SerializeField] private Stat maxRange;

    [Space]

    [SerializeField] private Stat doubleDamageChance;

    private Stat Speed => speed.DeepCopy();
    private Stat Damage => damage.DeepCopy();
    private Stat MaxRange => maxRange.DeepCopy();
    private Stat DoubleDamageChance => doubleDamageChance.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new SniperSkill(source, controller, this, CooldownTime, Damage, Speed, MaxRange, DoubleDamageChance);
    }
}
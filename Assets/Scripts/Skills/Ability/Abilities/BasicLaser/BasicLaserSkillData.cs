using UnityEngine;

[CreateAssetMenu(fileName = "Basic Laser", menuName = "ScriptableObjects/Abilities/BasicLaser", order = 1)]
public class BasicLaserSkillData : AbilityData
{
    [field: Header("Laser")]
    [field: SerializeField] public GameObject LaserPrefab { get; private set; }

    [field: Header("Stats")]
    [SerializeField] private Stat damage;
    [SerializeField] private Stat damageCooldown;

    [Space]

    [SerializeField] private Stat maxRange;
    [field: SerializeField] public float Radius { get; private set; }
    [field: SerializeField] public int MaxPierce { get; private set; }

    private Stat Damage => damage.DeepCopy();
    private Stat DamageCooldown => damageCooldown.DeepCopy();

    private Stat MaxRange => maxRange.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new BasicLaserSkill(source, controller, this, CooldownTime, Damage, DamageCooldown, MaxRange);
    }
}
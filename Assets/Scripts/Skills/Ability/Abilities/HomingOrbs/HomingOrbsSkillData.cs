using UnityEngine;

[CreateAssetMenu(fileName = "Homing Orbs", menuName = "ScriptableObjects/Abilities/HomingOrbs", order = 1)]
public class HomingOrbsSkillData : AbilityData
{
    [SerializeField] private Stat duration;

    [Header("Homing Orbs")]

    [field: SerializeField] public HomingOrb Projectile { get; private set; }
    [field: SerializeField] public float WarmUp { get; private set; }
    [field: SerializeField] public float MaxRange { get; private set; }
    [SerializeField] private Stat fireCooldown;
    [SerializeField] private Stat damage;
    [SerializeField] private Stat amount;

    [field: Space]

    [field: SerializeField] public float DetectRadius { get; private set; }
    [field: SerializeField] public LayerMask HitLayers { get; private set; }
    [field: SerializeField] public LayerMask BlockLayers { get; private set; }

    [field: Header("Visual")]

    [field: SerializeField] public float SpinSpeed { get; private set; }
    [field: SerializeField] public float Distance { get; private set; }

    private Stat FireCooldown => fireCooldown.DeepCopy();
    private Stat Duration => duration.DeepCopy();
    private Stat Damage => damage.DeepCopy();
    private Stat Amount => amount.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new HomingOrbsSkill(source, controller, this, CooldownTime, Duration, Amount, FireCooldown, Damage);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Fire Barrier", menuName = "ScriptableObjects/Abilities/FireBarrier", order = 1)]
public class FireBarrierSkillData : AbilityData
{
    [field: Header("Fire Barrier")]
    [field: SerializeField] public GameObject FireBarrierPrefab { get; private set; }
    [field: SerializeField] public float CastDistanceAway { get; private set; }

    [Header("Fire Barrier Stats")]
    [SerializeField] private Stat duration;
    [field: Space]
    [field: SerializeField] public float Scale { get; private set; }
    [field: SerializeField] public Vector2 Size { get; private set; }

    private Stat Duration => duration.DeepCopy();

    public override IAbility CreateAbility(GameObject source, AbilityController controller)
    {
        return new FireBarrierSkill(source, controller, this, CooldownTime, Duration);
    }
}
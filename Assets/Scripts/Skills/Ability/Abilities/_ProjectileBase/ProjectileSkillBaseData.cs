using UnityEngine;

public abstract class ProjectileSkillBaseData : AbilityData
{
    [field: Header("Projectile Base")]
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public float CastDistanceAway { get; private set; }

    [Header("Projectile Base Stats")]
    [SerializeField] private Stat maxRange;
    [SerializeField] private Stat damage;
    [SerializeField] private Stat speed;

    public Stat MaxRange => maxRange.DeepCopy();
    public Stat Damage => damage.DeepCopy();
    public Stat Speed => speed.DeepCopy();
}
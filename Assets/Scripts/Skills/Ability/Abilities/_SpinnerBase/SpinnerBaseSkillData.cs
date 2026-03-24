using UnityEngine;

public abstract class SpinnerBaseSkillData : AbilityData
{
    [field: SerializeField] public GameObject SpinObjectPrefab { get; private set; }
    [SerializeField] private Stat duration;

    [Space]

    [SerializeField] private Stat distance;
    [SerializeField] private Stat speed;
    [SerializeField] private Stat amount;

    protected Stat Distance => distance.DeepCopy();
    protected Stat Speed => speed.DeepCopy();
    protected Stat Amount => amount.DeepCopy();

    protected Stat Duration => duration.DeepCopy();
}
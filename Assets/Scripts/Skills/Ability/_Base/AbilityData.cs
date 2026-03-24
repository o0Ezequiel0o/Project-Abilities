using UnityEngine;

public abstract class AbilityData : ScriptableObject
{
    [field: Header("Display")]
    [field: SerializeField] public Sprite Icon { get; private set; }

    [field: Header("Toggling")]
    [field: SerializeField] public AbilityType AbilityType { get; private set; }
    [field: SerializeField] public bool CanHold { get; private set; }

    [Header("Main Settings")]
    [SerializeField] private Stat cooldownTime;

    protected Stat CooldownTime => cooldownTime.DeepCopy();

    public abstract IAbility CreateAbility(GameObject source, AbilityController controller);
}

public enum AbilityType
{
    Primary,
    Secondary,
    Utility,
    Ultimate
}
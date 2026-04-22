using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    [field: Header("Display")]
    [field: SerializeField] public Sprite Icon { get; private set; }

    [field: Header("Data")]
    [field: SerializeField, Min(1)] public int MaxStacks {private set; get;} = 1;
    [field: SerializeField] public StatusEffectType StatusEffectType {private set; get;}

    public abstract StatusEffect CreateEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source);
}

public enum StatusEffectType
{
    None,
    Buff,
    Debuff
}
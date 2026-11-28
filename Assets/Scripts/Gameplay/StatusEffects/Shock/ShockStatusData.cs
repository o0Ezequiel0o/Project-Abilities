using UnityEngine;

[CreateAssetMenu(fileName = "Shock", menuName = "ScriptableObjects/Status Effects/Shock", order = 1)]
public class ShockStatusData : StatusEffectData
{
    [field: Space]
    [field: SerializeField] public float Duration { get; private set; } = 5f;
    [Space]
    [SerializeField] private float moveSpeedMultiplier;
    [SerializeField] private float aimSpeedMultiplier;

    public override StatusEffect CreateEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source)
    {
        return new ShockStatus(statusEffectHandler, receiver, source, this, moveSpeedMultiplier, aimSpeedMultiplier);
    }
}
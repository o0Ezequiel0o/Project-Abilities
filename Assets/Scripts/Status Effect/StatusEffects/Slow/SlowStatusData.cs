using UnityEngine;

[CreateAssetMenu(fileName = "Slow", menuName = "ScriptableObjects/Status Effects/Slow", order = 1)]
public class SlowStatusData : StatusEffectData
{
    [field: Space]

    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeReferenceDropdown, SerializeReference] public IStackStat MoveSpeedMultiplier { get; private set; }

    public override StatusEffect CreateEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source)
    {
        return new SlowStatus(statusEffectHandler, receiver, source, this);
    }
}
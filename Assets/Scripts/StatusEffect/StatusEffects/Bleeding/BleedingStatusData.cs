using UnityEngine;

[CreateAssetMenu(fileName = "Bleeding", menuName = "ScriptableObjects/Status Effects/Bleeding", order = 1)]
public class BleedingStatusData : StatusEffectData
{
    [field: Space]

    [field: SerializeField] public int Ticks { get; private set; }
    [field: SerializeField] public float TickTime { get; private set; }

    [field: Space]

    [field: SerializeReferenceDropdown, SerializeReference] public IStackStat MaxHealthRatio { get; private set; }
    [field: SerializeReferenceDropdown, SerializeReference] public IStackStat MaxDamage { get; private set; }

    [field: Space]

    [field: SerializeField] public GameObject StainParticles { get; private set; }

    public override StatusEffect CreateEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source)
    {
        return new BleedingStatus(statusEffectHandler, receiver, source, this);
    }
}
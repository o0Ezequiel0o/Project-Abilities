using UnityEngine;

[CreateAssetMenu(fileName = "Burning", menuName = "ScriptableObjects/Status Effects/Burning", order = 1)]
public class BurningStatusData : StatusEffectData
{
    [field: Header("Visual")]
    [field: SerializeField] public GameObject Particles {private set; get;}
    [field: Header("Damage")]
    [field: SerializeField] public int Damage {private set; get;}
    [field: Space]
    [field: SerializeField] public float TickTime {private set; get;}
    [field: SerializeField] public int Ticks {private set; get;}

    [field: Header("Damage Increase")]
    [field: SerializeField] public int StacksRequired { get; private set; }
    [field: SerializeField] public float IncreasePerStacksRequired { get; private set; }

    public override StatusEffect CreateEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source)
    {
        return new BurningStatus(statusEffectHandler, receiver, source, this);
    }
}
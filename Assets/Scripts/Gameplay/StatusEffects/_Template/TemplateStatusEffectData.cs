using UnityEngine;

[CreateAssetMenu(fileName = "Status Effect Template", menuName = "ScriptableObjects/Status Effects/TemplateStatusEffectData", order = 1)]
public class TemplateStatusEffectData : StatusEffectData
{
    [field: Space]
    [field: SerializeField] public float TemplateVar {private set; get;}

    public override StatusEffect CreateEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source)
    {
        return new TemplateStatusEffect(statusEffectHandler, receiver, source, this);
    }
}
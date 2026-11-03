using UnityEngine;

[CreateAssetMenu(fileName = "Item Status Indicator", menuName = "ScriptableObjects/Status Effects/new Item Status Indicator", order = 1)]
public class ItemStatusIndicatorData : StatusEffectData
{
    public override StatusEffect CreateEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source)
    {
        return new ItemStatusIndicator(statusEffectHandler, receiver, source, this);
    }
}
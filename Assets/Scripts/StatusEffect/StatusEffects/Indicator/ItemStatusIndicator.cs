using UnityEngine;

public class ItemStatusIndicator : StatusEffect
{
    public override StatusEffectData Data => effectData;
    private readonly ItemStatusIndicatorData effectData;

    public ItemStatusIndicator(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source, ItemStatusIndicatorData effectData)
    {
        this.effectData = effectData;
    }

    public override void Initialize() {}

    public override void OnStacksApplied(int stacks) {}

    public override void OnStacksRemoved(int stacks) {}

    public override void OnUpdate() {}

    public override void OnLateUpdate() {}
    
    public override void OnRemove() {}

    public override void OnDestroy() {}
}
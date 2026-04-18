using UnityEngine;

public class TemplateStatusEffect : StatusEffect
{
    public override StatusEffectData Data => effectData;
    private readonly TemplateStatusEffectData effectData;

    private readonly StatusEffectHandler statusEffectHandler;

    private readonly GameObject receiver;
    private readonly GameObject source;

    public TemplateStatusEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source, TemplateStatusEffectData effectData)
    {
        this.statusEffectHandler = statusEffectHandler;

        this.receiver = receiver;
        this.source = source;

        this.effectData = effectData;
    }

    public override void OnApply() {}

    public override void OnStackApplied(int stacks) {}

    public override void OnUpdate() {}

    public override void OnLateUpdate() {}
    
    public override void OnRemove() {}

    public override void OnDestroy() {}
}
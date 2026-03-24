using UnityEngine;

public class TemplateStatusEffect : StatusEffect
{
    public override StatusEffectData Data => effectData;
    private TemplateStatusEffectData effectData;

    private StatusEffectHandler statusEffectHandler;

    private GameObject receiver;
    private GameObject source;

    public TemplateStatusEffect(StatusEffectHandler statusEffectHandler, GameObject receiver, GameObject source, TemplateStatusEffectData effectData)
    {
        this.statusEffectHandler = statusEffectHandler;

        this.receiver = receiver;
        this.source = source;

        this.effectData = effectData;
    }

    public override void OnApply() {}

    public override void OnStackApply() {}

    public override void OnUpdate() {}
    
    public override void OnRemove() {}
}
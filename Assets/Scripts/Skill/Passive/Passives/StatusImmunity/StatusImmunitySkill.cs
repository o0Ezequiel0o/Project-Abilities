using UnityEngine;

public class StatusImmunitySkill : PassiveBase
{
    public override PassiveData Data => data;
    private readonly StatusImmunitySkillData data;

    private readonly GameObject source;

    public StatusImmunitySkill(GameObject source, PassiveController passiveController, StatusImmunitySkillData data) : base(passiveController)
    {
        this.source = source;
        this.data = data;
    }

    public override void Awake()
    {
        if (source.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            for (int i = 0; i < data.Immunities.Count; i++)
            {
                statusEffectHandler.ApplyImmunityToStatusEffect(data.Immunities[i]);
            }
        }
    }

    public override void OnRemove()
    {
        if (source.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            for (int i = 0; i < data.Immunities.Count; i++)
            {
                statusEffectHandler.RemoveImmunityToStatusEffect(data.Immunities[i]);
            }
        }
    }
}
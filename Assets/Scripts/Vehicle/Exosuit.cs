using UnityEngine;

public class Exosuit : Vehicle
{
    [Header("Dependency")]
    [SerializeField] private Damageable damageable;
    [SerializeField] private StatusEffectHandler statusEffectHandler;

    private readonly int immunityID = GameInstance.GetUniqueID();

    protected override void OnEnterVehicle(GameObject source)
    {
        SubscribeToEvents(source);
    }

    protected override void OnExitVehicle(GameObject source)
    {
        UnsubscribeFromEvents(source);
    }

    private void Reset()
    {
        damageable = GetComponentInChildren<Damageable>();
        statusEffectHandler = GetComponentInChildren<StatusEffectHandler>();
    }

    private void SubscribeToEvents(GameObject source)
    {
        if (source.TryGetComponent(out Damageable damageable))
        {
            damageable.AddImmunitySource(immunityID);
            damageable.onDamageEvent.Subscribe(RedirectDamage);
        }
        if (source.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.AddImmunitySource(immunityID);
            statusEffectHandler.onApplyEffect.Subscribe(RedirectStatusEffect);
        }
    }

    private void UnsubscribeFromEvents(GameObject source)
    {
        if (source.TryGetComponent(out Damageable damageable))
        {
            damageable.RemoveImmunitySource(immunityID);
            damageable.onDamageEvent.Subscribe(RedirectDamage);
        }
        if (source.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.RemoveImmunitySource(immunityID);
            statusEffectHandler.onApplyEffect.Unsubscribe(RedirectStatusEffect);
        }
    }

    private void RedirectDamage(Damageable.DamageEvent damageEvent)
    {
        if (damageable == null) return;

        damageable.DealDamage(new DamageInfo(damageEvent), damageEvent.SourceUser, damageEvent.SourceObject, damageEvent.ProcChainBranch);
    }

    private void RedirectStatusEffect(StatusEffectHandler.EffectApplyInfo effectInfo)
    {
        if (statusEffectHandler == null) return;
        statusEffectHandler.ApplyEffect(effectInfo.data, effectInfo.source, effectInfo.stacks);
    }

    private class RedirectedHitData
    {
        public Damageable receiver;
        public GameObject sourceUser;
        public GameObject sourceObject;
    }
}
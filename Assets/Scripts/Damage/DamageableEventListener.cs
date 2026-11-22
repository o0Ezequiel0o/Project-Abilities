using UnityEngine;
using Zeke.Collections;

public class DamageableEventListener : MonoBehaviour
{
    public IOrderedActionSubscription<Damageable.IDealDamageEvent> OnDealDamage => onDealDamage;
    public IOrderedActionSubscription<Damageable.IDealtDamageEvent> OnDealtDamage => onDealtDamage;

    public IOrderedActionSubscription<Damageable.IKillEvent> OnKill => onKill;
    public IOrderedActionSubscription<Damageable.IHitEvent> OnHit => onHit;

    public IOrderedActionSubscription<Damageable.HealEvent.OnHealHealthEvent> OnHealHealth => onHealHealth;
    public IOrderedActionSubscription<Damageable.HealEvent.OnHealedHealthEvent> OnHealedHealth => onHealedHealth;

    private readonly OrderedAction<Damageable.IDealDamageEvent> onDealDamage = new OrderedAction<Damageable.IDealDamageEvent>();
    private readonly OrderedAction<Damageable.IDealtDamageEvent> onDealtDamage = new OrderedAction<Damageable.IDealtDamageEvent>();

    private readonly OrderedAction<Damageable.IKillEvent> onKill = new OrderedAction<Damageable.IKillEvent>();
    private readonly OrderedAction<Damageable.IHitEvent> onHit = new OrderedAction<Damageable.IHitEvent>();

    private readonly OrderedAction<Damageable.HealEvent.OnHealHealthEvent> onHealHealth = new OrderedAction<Damageable.HealEvent.OnHealHealthEvent>();
    private readonly OrderedAction<Damageable.HealEvent.OnHealedHealthEvent> onHealedHealth = new OrderedAction<Damageable.HealEvent.OnHealedHealthEvent>();

    public void CallDealDamageEvent(Damageable.IDealDamageEvent onDealDamageEvent) => onDealDamage.Invoke(onDealDamageEvent);
    public void CallDealtDamageEvent(Damageable.IDealtDamageEvent onDealtDamageEvent) => onDealtDamage.Invoke(onDealtDamageEvent);

    public void CallKillEvent(Damageable.IKillEvent onKillEvent) => onKill.Invoke(onKillEvent);
    public void CallHitEvent(Damageable.IHitEvent onHitEvent) => onHit.Invoke(onHitEvent);

    public void CallHealHealthEvent(Damageable.HealEvent.OnHealHealthEvent onHealHealthEvent) => onHealHealth.Invoke(onHealHealthEvent);
    public void CallHealedHealthEvent(Damageable.HealEvent.OnHealedHealthEvent onHealedHealthEvent) => onHealedHealth.Invoke(onHealedHealthEvent);
}
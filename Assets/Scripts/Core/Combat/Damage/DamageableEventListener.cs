using UnityEngine;
using Zeke.Collections;

public class DamageableEventListener : MonoBehaviour
{
    public IOrderedActionSubscription<Damageable.IDealDamageEvent> OnDealDamage => onDealDamage;
    public IOrderedActionSubscription<Damageable.IDealtDamageEvent> OnDealtDamage => onDealtDamage;

    public IOrderedActionSubscription<Damageable.IKillEvent> OnKill => onKill;
    public IOrderedActionSubscription<Damageable.IHitDamageEvent> OnHit => onHit;

    public IOrderedActionSubscription<Damageable.IHealHealthEvent> OnHealHealth => onHealHealth;
    public IOrderedActionSubscription<Damageable.IHealedHealthEvent> OnHealedHealth => onHealedHealth;

    public IOrderedActionSubscription<Damageable.IHitHealEvent> OnHealHit => onHealHit;

    private readonly OrderedAction<Damageable.IDealDamageEvent> onDealDamage = new OrderedAction<Damageable.IDealDamageEvent>();
    private readonly OrderedAction<Damageable.IDealtDamageEvent> onDealtDamage = new OrderedAction<Damageable.IDealtDamageEvent>();

    private readonly OrderedAction<Damageable.IKillEvent> onKill = new OrderedAction<Damageable.IKillEvent>();
    private readonly OrderedAction<Damageable.IHitDamageEvent> onHit = new OrderedAction<Damageable.IHitDamageEvent>();

    private readonly OrderedAction<Damageable.IHealHealthEvent> onHealHealth = new OrderedAction<Damageable.IHealHealthEvent>();
    private readonly OrderedAction<Damageable.IHealedHealthEvent> onHealedHealth = new OrderedAction<Damageable.IHealedHealthEvent>();

    private readonly OrderedAction<Damageable.IHitHealEvent> onHealHit = new OrderedAction<Damageable.IHitHealEvent>();

    public void CallDealDamageEvent(Damageable.IDealDamageEvent onDealDamageEvent) => onDealDamage.Invoke(onDealDamageEvent);
    public void CallDealtDamageEvent(Damageable.IDealtDamageEvent onDealtDamageEvent) => onDealtDamage.Invoke(onDealtDamageEvent);

    public void CallKillEvent(Damageable.IKillEvent onKillEvent) => onKill.Invoke(onKillEvent);
    public void CallHitEvent(Damageable.IHitDamageEvent onHitEvent) => onHit.Invoke(onHitEvent);

    public void CallHealHealthEvent(Damageable.IHealHealthEvent onHealHealthEvent) => onHealHealth.Invoke(onHealHealthEvent);
    public void CallHealedHealthEvent(Damageable.IHealedHealthEvent onHealedHealthEvent) => onHealedHealth.Invoke(onHealedHealthEvent);

    public void CallHealHitEvent(Damageable.IHitHealEvent onHealHitEvent) => onHealHit.Invoke(onHealHitEvent);
}
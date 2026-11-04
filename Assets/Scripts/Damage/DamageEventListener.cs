using UnityEngine;
using Zeke.Collections;

public class DamageEventListener : MonoBehaviour
{
    public IOrderedActionSubscription<Damageable.DamageEvent.OnDealDamageEvent> OnDealDamage => onDealDamage;
    public IOrderedActionSubscription<Damageable.DamageEvent.OnDealtDamageEvent> OnDealtDamage => onDealtDamage;

    public IOrderedActionSubscription<Damageable.DamageEvent.OnKillEvent> OnKill => onKill;
    public IOrderedActionSubscription<Damageable.DamageEvent.OnHitEvent> OnHit => onHit;

    private readonly OrderedAction<Damageable.DamageEvent.OnDealDamageEvent> onDealDamage = new OrderedAction<Damageable.DamageEvent.OnDealDamageEvent>();
    private readonly OrderedAction<Damageable.DamageEvent.OnDealtDamageEvent> onDealtDamage = new OrderedAction<Damageable.DamageEvent.OnDealtDamageEvent>();

    private readonly OrderedAction<Damageable.DamageEvent.OnKillEvent> onKill = new OrderedAction<Damageable.DamageEvent.OnKillEvent>();
    private readonly OrderedAction<Damageable.DamageEvent.OnHitEvent> onHit = new OrderedAction<Damageable.DamageEvent.OnHitEvent>();

    public void CallDealDamageEvent(Damageable.DamageEvent.OnDealDamageEvent onDealDamageEvent) => onDealDamage.Invoke(onDealDamageEvent);
    public void CallDealtDamageEvent(Damageable.DamageEvent.OnDealtDamageEvent onDealtDamageEvent) => onDealtDamage.Invoke(onDealtDamageEvent);

    public void CallKillEvent(Damageable.DamageEvent.OnKillEvent onKillEvent) => onKill.Invoke(onKillEvent);
    public void CallHitEvent(Damageable.DamageEvent.OnHitEvent onHitEvent) => onHit.Invoke(onHitEvent);
}
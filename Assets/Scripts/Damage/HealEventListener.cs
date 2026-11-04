using UnityEngine;
using Zeke.Collections;

public class HealEventListener : MonoBehaviour
{
    public IOrderedActionSubscription<Damageable.HealEvent.OnHealHealthEvent> OnHealHealth => onHealHealth;
    public IOrderedActionSubscription<Damageable.HealEvent.OnHealedHealthEvent> OnHealedHealth => onHealedHealth;

    private readonly OrderedAction<Damageable.HealEvent.OnHealHealthEvent> onHealHealth = new OrderedAction<Damageable.HealEvent.OnHealHealthEvent>();
    private readonly OrderedAction<Damageable.HealEvent.OnHealedHealthEvent> onHealedHealth = new OrderedAction<Damageable.HealEvent.OnHealedHealthEvent>();

    public void CallHealHealthEvent(Damageable.HealEvent.OnHealHealthEvent onHealHealthEvent) => onHealHealth.Invoke(onHealHealthEvent);
    public void CallHealedHealthEvent(Damageable.HealEvent.OnHealedHealthEvent onHealedHealthEvent) => onHealedHealth.Invoke(onHealedHealthEvent);
}
public abstract class Item
{
    public abstract ItemData Data { get; }

    public int stacks;

    public virtual void OnUpdate() { }

    public virtual void OnAdded() { }

    public virtual void OnRemoved() { }

    public virtual void OnStackAdded() { }

    public virtual void OnStackRemoved() { }

    public virtual void OnHealthHealed(Damageable.HealEvent healingEvent) { }

    public virtual void OnHealthReceived(Damageable.HealEvent healingEvent) { }

    public virtual void OnHit(Damageable.DamageEvent damageEvent) { }

    public virtual void OnHitTaken(Damageable.DamageEvent damageEvent) { }

    public virtual void OnDealDamage(Damageable.DamageEvent damageEvent) { }

    public virtual void OnTakeDamage(Damageable.DamageEvent damageEvent) { }

    public virtual void OnDamageDealt(Damageable.DamageEvent damageEvent) { }

    public virtual void OnDamageTaken(Damageable.DamageEvent damageEvent) { }

    public virtual void OnKill(Damageable.DamageEvent damageEvent) { }

    public virtual void OnDeath(Damageable.DamageEvent damageEvent) { }
}
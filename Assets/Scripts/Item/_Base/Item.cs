using UnityEngine;

public abstract class Item
{
    public static bool RollProc(float chance, float coefficient, int luck)
    {
        bool rollSucess = chance * coefficient > Random.Range(0f, 100f - Mathf.Epsilon);

        if (luck < 0 && rollSucess)
        {
            return RollProc(chance, coefficient, luck + 1);
        }
        if (luck > 0 && !rollSucess)
        {
            return RollProc(chance, coefficient, luck - 1);
        }

        return rollSucess;
    }

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
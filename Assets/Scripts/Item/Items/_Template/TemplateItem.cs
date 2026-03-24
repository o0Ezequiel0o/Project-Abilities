using UnityEngine;

public class TemplateItem : Item
{
    //Template
    public override ItemData Data => data;
    private TemplateItemData data;

    private ItemHandler itemHandler;
    private GameObject source;

    public TemplateItem(TemplateItemData data, ItemHandler itemHandler, GameObject source)
    {
        this.data = data;
        this.source = source;
        this.itemHandler = itemHandler;
    }

    public override void OnUpdate() { }

    public override void OnAdded() { }

    public override void OnRemoved() { }

    public override void OnStackAdded() { }

    public override void OnStackRemoved() { }

    public override void OnHealthHealed(Damageable.HealEvent healingEvent) { }

    public override void OnHealthReceived(Damageable.HealEvent healingEvent) { }

    public override void OnHit(Damageable.DamageEvent damageEvent) { }

    public override void OnHitTaken(Damageable.DamageEvent damageEvent) { }

    public override void OnDealDamage(Damageable.DamageEvent damageEvent) { }

    public override void OnTakeDamage(Damageable.DamageEvent damageEvent) { }

    public override void OnDamageDealt(Damageable.DamageEvent damageEvent) { }

    public override void OnDamageTaken(Damageable.DamageEvent damageEvent) { }

    public override void OnKill(Damageable.DamageEvent damageEvent) { }

    public override void OnDeath(Damageable.DamageEvent damageEvent) { }
}
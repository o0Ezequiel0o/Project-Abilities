using UnityEngine;
using Zeke.PoolableGameObjects;

public class MissileItem : Item
{
    //Template
    public override ItemData Data => data;
    private MissileItemData data;

    private ItemHandler itemHandler;
    private GameObject source;

    private readonly GameObjectPool<Missile> missilePool = new GameObjectPool<Missile>();

    public MissileItem(MissileItemData data, ItemHandler itemHandler, GameObject source)
    {
        this.data = data;
        this.source = source;
        this.itemHandler = itemHandler;
    }

    public override void OnAdded() { }

    public override void OnRemoved()
    {
        missilePool.Clear();
    }

    public override void OnStackAdded() { }

    public override void OnStackRemoved() { }

    public override void OnHit(Damageable.DamageEvent damageEvent)
    {
        if (damageEvent.SourceUser == null || damageEvent.SourceUser == source) return;

        if (damageEvent.ProcChainBranch.Contains(Data)) return;

        //the item should carry the proc chain so all projectiles should (?
    }
}
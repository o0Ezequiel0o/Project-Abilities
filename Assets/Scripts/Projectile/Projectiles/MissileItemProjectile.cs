using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public class MissileItemProjectile : Missile
{
    private List<ItemData> ProcChainCopy => new List<ItemData>(procChain);
    private readonly List<ItemData> procChain = new List<ItemData>();

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();
        procChain.Clear();
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, GameObject source, Teams team, List<ItemData> procChain)
    {
        this.procChain.AddRange(procChain);
        Launch(position, speed, direction, maxRange, damage, source, team);
    }

    protected override void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        bool damageRejected = false;

        if (receiver.TryGetComponent(out Damageable damageable))
        {
            damageRejected = damageable.DealDamage(new DamageInfo(Damage, procCoefficient, armorPenetration), SourceUser, gameObject, ProcChainCopy).damageRejected;
        }

        if (!damageRejected)
        {
            ApplyKnockback(receiver, Direction);
        }
    }
}
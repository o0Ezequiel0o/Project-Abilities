using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;
using Zeke.Items;

public class MissileItemProjectile : Missile
{
    private List<ItemData> ProcChainCopy => new List<ItemData>(procChain);
    private readonly List<ItemData> procChain = new List<ItemData>();

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();
        procChain.Clear();
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, GameObject source, Teams team, List<ItemData> procChain)
    {
        this.procChain.AddRange(procChain);
        Launch(position, speed, direction, maxRange, damageData, knockback, source, team);
    }

    protected override void Hit(GameObject receiver)
    {
        bool damageRejected = false;

        if (receiver.TryGetComponent(out Damageable damageable))
        {
            DamageInfo damageInfo = new DamageInfo(Damage, armorPenetration, procCoefficient)
            {
                direction = GetHitDirection(receiver)
            };

            damageRejected = damageable.DealDamage(damageInfo, SourceUser, gameObject, ProcChainCopy).damageRejected;
        }

        if (!damageRejected)
        {
            ApplyKnockback(receiver, Direction);
        }
    }
}
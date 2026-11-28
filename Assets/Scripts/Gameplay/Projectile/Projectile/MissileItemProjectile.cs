using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public class MissileItemProjectile : Missile
{
    private List<IProcSource> ProcChainCopy => new List<IProcSource>(procChain);
    private readonly List<IProcSource> procChain = new List<IProcSource>();

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();
        procChain.Clear();
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, GameObject source, Teams team, List<IProcSource> procChain)
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

            damageRejected = damageable.DealDamage(damageInfo, SourceUser, gameObject, ProcChainCopy).IsRejected;
        }

        if (!damageRejected)
        {
            ApplyKnockback(receiver, Direction);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;
using static Damageable;

public class FireBallProjectile : DamageProjectileBase
{
    [SerializeField] protected bool allyCollision;

    [Header("Fireball Settings")]
    public StatusEffectData statusEffectToApply;

    private float damageRadius;
    private readonly List<Collider2D> hits = new List<Collider2D>();

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, float damageRadius, GameObject source, Teams team)
    {
        this.damageRadius = damageRadius;
        Launch(position, speed, direction, maxRange, damageData, knockback, source, team);
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject receiver = hit.collider.gameObject;

        if (receiver == SourceUser) return;

        if (TeamManager.IsEnemy(Team, receiver) || allyCollision)
        {
            TeleportToHitPoint(hit.point);
            StopLoopingHits();
            Explode();
        }
    }

    protected override void OnMaxDistanceReached()
    {
        Explode();
    }

    private void Explode()
    {
        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayer, useLayerMask = true };
        Physics2D.OverlapCircle(TipPosition, damageRadius, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            Hit(hits[i].gameObject);
        }

        Despawn();
    }

    private void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        if (Physics2D.Linecast(TipPosition, receiver.transform.position, blockLayer)) return;

        DealDamage(receiver, OnDamageDealt);
    }

    private void OnDamageDealt(DamageEvent damageEvent)
    {
        if (damageEvent.damageRejected) return;

        GameObject receiver = damageEvent.Receiver.gameObject;

        if (receiver.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.ApplyEffect(statusEffectToApply, SourceUser);
        }

        ApplyKnockback(receiver, Direction);
    }
}
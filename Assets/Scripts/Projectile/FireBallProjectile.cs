using System.Collections.Generic;
using UnityEngine;

public class FireBallProjectile : DamageProjectileBase
{
    [Header("Fireball Settings")]
    public StatusEffectData statusEffectToApply;

    private float damageRadius;
    private readonly List<Collider2D> hits = new List<Collider2D>();

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, float damageRadius, GameObject source, Teams team)
    {
        this.damageRadius = damageRadius;
        Launch(position, speed, direction, maxRange, damage, source, team);
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        if (hit.collider.gameObject == SourceUser) return;

        TeleportToHitPoint(hit.point);
        StopLoopingHits();
        Explode();
    }

    protected override void OnMaxDistanceReached()
    {
        Explode();
    }

    private void Explode()
    {
        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayer };
        Physics2D.OverlapCircle(TipPosition, damageRadius, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            Hit(hits[i].gameObject);
        }

        Despawn();
    }

    void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

        if (Physics2D.Linecast(TipPosition, receiver.transform.position, blockLayer)) return;

        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        if (receiver.TryGetComponent(out StatusEffectHandler statusEffectHandler))
        {
            statusEffectHandler.ApplyEffect(statusEffectToApply, SourceUser);
        }

        ApplyKnockback(receiver, Direction);
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public class LightingBoltProjectile : DamageProjectileBase
{
    [SerializeField] private bool allyCollision;

    [Header("Lighting Bolt | Spread")]
    [SerializeField] private float spreadMaxRadius = 3f;

    private float spreadTargets;

    private readonly List<Collider2D> hits = new List<Collider2D>();
    private readonly HashSet<GameObject> ignoreTargets = new HashSet<GameObject>();

    public override void OnRetrievedFromPool()
    {
        base.OnRetrievedFromPool();
        ignoreTargets.Clear();
    }

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, DamageData damageData, float knockback, float spreadTargets, GameObject source, Teams team)
    {
        this.spreadTargets = spreadTargets;
        Launch(position, speed, direction, maxRange, damageData, knockback, source, team);
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        GameObject receiver = hit.collider.gameObject;

        if (receiver == SourceUser) return;

        if (TeamManager.IsEnemy(Team, receiver))
        {
            Hit(hit.transform.gameObject);
        }
        else if (!allyCollision) return;

        TeleportToHitPoint(hit.point);
        Despawn();
    }

    private void Hit(GameObject receiver)
    {
        bool damageRejected = DealDamage(receiver);

        if (damageRejected) return;

        ApplyKnockback(receiver, Direction);

        if (spreadTargets > 0)
        {
            spreadTargets -= 1;
            ignoreTargets.Add(receiver);
            SpreadToNearTargets(receiver);
        }
    }

    private void SpreadToNearTargets(GameObject receiver)
    {
        hits.Clear();

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayer, useLayerMask = true };
        Physics2D.OverlapCircle(receiver.transform.position, spreadMaxRadius, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if (TeamManager.IsAlly(Team, hits[i].gameObject)) continue;
            if (ignoreTargets.Contains(hits[i].gameObject)) continue;

            Hit(hits[i].gameObject);
            break;
        }
    }
}
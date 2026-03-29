using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

public class LightingBoltProjectile : DamageProjectileBase
{
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

    public void Launch(Vector3 position, float speed, Vector2 direction, float maxRange, float damage, float spreadTargets, GameObject source, Teams team)
    {
        this.spreadTargets = spreadTargets;
        Launch(position, speed, direction, maxRange, damage, source, team);
    }

    protected override void OnCollision(RaycastHit2D hit)
    {
        if (hit.collider.gameObject == SourceUser) return;

        Hit(hit.transform.gameObject);
        TeleportToHitPoint(hit.point);

        Despawn();
    }

    private void Hit(GameObject receiver)
    {
        if (TeamManager.IsAlly(Team, receiver)) return;

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

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayer };
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
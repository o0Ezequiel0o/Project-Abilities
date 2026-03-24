using System.Collections.Generic;
using UnityEngine;

public class LightingBoltProjectile : BasicProjectile
{
    [Header("Lighting Bolt Settings")]
    [SerializeField] private float spreadMaxRadius = 3f;

    private float spreadTargets;

    private readonly List<Collider2D> hits = new List<Collider2D>();
    private readonly HashSet<GameObject> ignoreTargets = new HashSet<GameObject>();

    public void SetSpreadTargets(float spreadTargets)
    {
        this.spreadTargets = spreadTargets + 1;
    }

    public override void OnPoolableGet()
    {
        base.OnPoolableGet();
        ignoreTargets.Clear();
    }

    protected override void OnHitInternal(GameObject receiver, bool damageRejected)
    {
        hits.Clear();

        if (spreadTargets <= 0 || damageRejected) return;

        ignoreTargets.Add(receiver);

        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayer };
        Physics2D.OverlapCircle(receiver.transform.position, spreadMaxRadius, contactFilter, hits);

        for (int i = 0; i < hits.Count; i++)
        {
            if (TeamManager.IsAlly(SourceUser, hits[i].gameObject)) continue;
            if (ignoreTargets.Contains(hits[i].gameObject)) continue;

            SpreadToTarget(hits[i].gameObject);
            break;
        }
    }

    void SpreadToTarget(GameObject target)
    {
        spreadTargets -= 1;
        Hit(target);
    }
}
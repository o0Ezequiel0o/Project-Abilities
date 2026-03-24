using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class OverlapTrigger
{
    [SerializeField] private LayerMask hitLayers;

    public List<Collider2D> Overlapping => collidersInside;

    public Action<Collider2D> onTriggerEnter;
    public Action<Collider2D> onTriggerExit;

    protected readonly List<Collider2D> hits = new List<Collider2D>();

    private readonly List<Collider2D> collidersInside = new List<Collider2D>();
    private readonly HashSet<Collider2D> collidersInsideHash = new HashSet<Collider2D>();

    public OverlapTrigger() {}

    public OverlapTrigger(LayerMask hitLayers)
    {
        this.hitLayers = hitLayers;
    }

    public bool IsOverlapping(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Collider2D collider))
        {
            return IsOverlapping(collider);
        }

        return false;
    }

    public bool IsOverlapping(Collider2D collider)
    {
        return collidersInsideHash.Contains(collider);
    }

    public void Update(Vector3 center)
    {
        Update(center, Vector3.zero);
    }

    public void Update(Vector3 center, Vector3 direction)
    {
        GetHitsInternal(center, direction, hitLayers);

        for (int i = collidersInside.Count - 1; i >= 0; i--)
        {
            if (!InHits(collidersInside[i]))
            {
                onTriggerExit?.Invoke(collidersInside[i]);

                collidersInsideHash.Remove(collidersInside[i]);
                collidersInside.Remove(collidersInside[i]);
            }
        }

        for (int i = 0; i < hits.Count; i++)
        {
            if (!collidersInsideHash.Contains(hits[i]))
            {
                onTriggerEnter?.Invoke(hits[i]);

                collidersInsideHash.Add(hits[i]);
                collidersInside.Add(hits[i]);
            }
        }
    }

    public void DrawGizmos(Vector3 center)
    {
        DrawGizmosInternal(center, Vector3.zero);
    }

    public void DrawGizmos(Vector3 center, Vector3 direction)
    {
        DrawGizmosInternal(center, direction);
    }

    protected abstract void GetHitsInternal(Vector3 center, Vector3 direction, LayerMask hitLayers);

    protected abstract void DrawGizmosInternal(Vector3 center, Vector3 direction);

    bool InHits(Collider2D collider)
    {
        for (int i = 0; i < hits.Count; i++)
        {
            if (collider == hits[i]) return true;
        }

        return false;
    }
}
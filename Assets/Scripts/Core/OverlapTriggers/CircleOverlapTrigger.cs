using UnityEngine;
using System;

[Serializable]
public class CircleOverlapTrigger : OverlapTrigger
{
    [SerializeField] private float radius;
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos = false;
    [SerializeField] private Color radiusColor = Color.yellow;

    protected override void GetHitsInternal(Vector3 center, Vector3 direction, LayerMask hitLayers)
    {
        hits.Clear();
        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers, useLayerMask = true };
        Physics2D.OverlapCircle(center, radius, contactFilter, hits);
    }

    protected override void DrawGizmosInternal(Vector3 center, Vector3 direction)
    {
        if (!drawGizmos) return;
        Gizmos.color = radiusColor;
        Gizmos.DrawWireSphere(center, radius);
    }
}
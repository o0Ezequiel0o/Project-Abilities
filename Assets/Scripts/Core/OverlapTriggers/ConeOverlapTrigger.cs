using UnityEngine;
using System;

[Serializable]
public class ConeOverlapTrigger : OverlapTrigger
{
    [SerializeField] private float radius;
    [SerializeField] private float angle;
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos = false;
    [SerializeField] private Color radiusColor = Color.yellow;
    [SerializeField] private Color angleColor = new Color(255f, 165f, 0f);

    public ConeOverlapTrigger() {}

    public ConeOverlapTrigger(float radius, float angle, LayerMask hitLayers) : base(hitLayers)
    {
        this.radius = radius;
        this.angle = angle;
    }

    protected override void GetHitsInternal(Vector3 center, Vector3 direction, LayerMask hitLayers)
    {
        ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers };
        Physics2D.OverlapCircle(center, radius, contactFilter, hits);

        for (int i = hits.Count - 1; i >= 0; i--)
        {
            Vector2 relativeDirection = hits[i].transform.position - center;

            if (Vector2.Angle(relativeDirection, direction) > angle)
            {
                hits.RemoveAt(i);
            }
        }
    }

    protected override void DrawGizmosInternal(Vector3 center, Vector3 direction)
    {
        if (!drawGizmos) return;
        Gizmos.color = radiusColor;
        Gizmos.DrawWireSphere(center, radius);
        Gizmos.color = angleColor;
        Gizmos.DrawLine(center, center + Quaternion.Euler(0, 0, angle) * direction * radius);
        Gizmos.DrawLine(center, center + Quaternion.Euler(0, 0, -angle) * direction * radius);
    }
}
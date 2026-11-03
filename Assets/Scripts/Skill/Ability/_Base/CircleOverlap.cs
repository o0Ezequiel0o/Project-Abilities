using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CircleOverlap : OverlapShape
    {
        [SerializeField] private float radius;

        public CircleOverlap() { }

        public CircleOverlap(CircleOverlap original)
        {
            radius = original.radius;
        }

        public override OverlapShape DeepCopy() => new CircleOverlap(this);

        public override List<Collider2D> GetHitsInternal(Vector2 position, float angle, LayerMask layerMask)
        {
            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = layerMask, useLayerMask = true};
            Physics2D.OverlapCircle(position, radius, contactFilter, hits);

            return hits;
        }
    }
}
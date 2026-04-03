using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class BoxOverlap : OverlapShape
    {
        [SerializeField] private Vector2 size;

        public BoxOverlap() { }

        public BoxOverlap(BoxOverlap original)
        {
            size = original.size;
        }

        public override OverlapShape DeepCopy() => new BoxOverlap(this);

        public override List<Collider2D> GetHitsInternal(Vector2 position, float angle, LayerMask layerMask)
        {
            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = layerMask, useLayerMask = true};
            Physics2D.OverlapBox(position, size, angle, contactFilter, hits);

            return hits;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public abstract class OverlapShape
    {
        protected readonly List<Collider2D> hits = new List<Collider2D>();

        public abstract OverlapShape DeepCopy();

        public List<Collider2D> GetHits(Vector2 position, float angle, LayerMask layerMask)
        {
            hits.Clear();
            return GetHitsInternal(position, angle, layerMask);
        }

        public abstract List<Collider2D> GetHitsInternal(Vector2 position, float angle, LayerMask layerMask);
    }
}
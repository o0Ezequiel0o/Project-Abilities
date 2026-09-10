using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    public class PullToCenter : AbilityModule
    {
        private readonly PullToCenterData data;

        private readonly Stat radius;
        private GameObject source;

        private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>();

        public PullToCenter(PullToCenterData data, Stat radius)
        {
            this.data = data;
            this.radius = radius;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            hits.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.CircleCast(source.transform.position, radius.Value, Vector2.zero, contactFilter, hits, 0f);

            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].collider.gameObject == source) continue;
                if (TeamManager.IsAlly(hits[i].collider.gameObject, source)) continue;

                float forceMultiplier = data.ScaleWithDistance ? Vector2.Distance(source.transform.position, hits[i].point) / radius.Value : 1f;

                Vector3 pullDirection = (source.transform.position - hits[i].transform.position).normalized;

                if (hits[i].collider.TryGetComponent(out Physics physics))
                {
                    physics.AddForce(data.Force * forceMultiplier * pullDirection);
                }
            }
        }

        public override void Upgrade()
        {
            radius.Upgrade();
        }
    }
}
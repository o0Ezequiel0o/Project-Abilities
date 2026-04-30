using System.Collections.Generic;
using UnityEngine;
using Zeke.PoolableGameObjects;
using Zeke.TeamSystem;

namespace Zeke.Items
{
    public class MagneticBootsItem : Item
    {
        public override ItemData Data => data;
        private readonly MagneticBootsItemData data;

        private readonly ItemHandler itemHandler;
        private readonly GameObject source;

        private Vector3 lastPosition;

        private float accumulatedDistance = 0f;

        private readonly GameObjectPool<HomingOrbProjectile> homingOrbs = new GameObjectPool<HomingOrbProjectile>();
        private readonly List<RaycastHit2D> rayHits = new List<RaycastHit2D>();

        public MagneticBootsItem(MagneticBootsItemData data, ItemHandler itemHandler, GameObject source)
        {
            this.data = data;
            this.source = source;
            this.itemHandler = itemHandler;
        }

        public override void Initialize()
        {
            lastPosition = source.transform.position;
        }

        public override void OnRemoved()
        {
            homingOrbs.Clear();
        }

        public override void OnUpdate()
        {
            accumulatedDistance += Vector3.Distance(lastPosition, source.transform.position);

            if (accumulatedDistance >= data.DistanceRequired)
            {
                FireHomingOrb();
                accumulatedDistance = 0f;
            }

            lastPosition = source.transform.position;
        }

        private void FireHomingOrb()
        {
            HomingOrbProjectile homingOrb = homingOrbs.Get(data.Prefab);
            homingOrb.Launch(source.transform.position, data.OrbSpeed, -source.transform.up, data.OrbRange, data.OrbDamage.GetValue(stacks), data.OrbPierce, source, TeamManager.GetTeam(source));

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.TargetLayers, useLayerMask = true };

            for (int i = 0; i < Physics2D.CircleCast(source.transform.position, data.FindTargetRange, Vector2.zero, contactFilter, rayHits, 0f); i++)
            {
                if (rayHits[i].collider.gameObject == source) continue;
                if (TeamManager.IsAlly(source, rayHits[i].collider.gameObject)) continue;

                homingOrb.SetTarget(rayHits[i].transform);
                break;
            }

            homingOrb.gameObject.SetActive(true);
        }
    }
}
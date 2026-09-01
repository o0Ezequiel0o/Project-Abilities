using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastTowardClosest : AbilityModule
    {
        private readonly CastTowardClosestData data;

        private readonly AbilityModule module;
        private readonly Stat targetRadius;

        private Transform pivot;
        private Transform newSpawn;

        private GameObject source;

        private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>(8);

        public CastTowardClosest(CastTowardClosestData data, AbilityModule module, Stat targetRadius)
        {
            this.data = data;
            this.module = module;
            this.targetRadius = targetRadius;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;

            pivot = new GameObject("CastTowardsClosest_Pivot").transform;
            newSpawn = new GameObject("CastPoition").transform;

            pivot.parent = source.transform;
            newSpawn.parent = pivot;

            pivot.localPosition = Vector3.zero;
            newSpawn.localPosition = data.Offset;

            module.OnInitialization(controller, newSpawn, source, ability);
        }

        public override bool CanActivate() => module.CanActivate();
        public override bool CanUpgrade() => module.CanUpgrade();

        public override void Activate(bool holding)
        {
            module.Activate(holding);
        }

        public override void Deactivate()
        {
            module.Deactivate();
        }

        public override void Update()
        {
            FaceTowardsTarget(pivot);
            module.Update();
        }

        public override void UpdateActive()
        {
            module.UpdateActive();
        }

        public override void UpdateInactive()
        {
            module.UpdateInactive();
        }

        public override void LateUpdate()
        {
            module.LateUpdate();
        }

        public override void Upgrade()
        {
            targetRadius.Upgrade();
            module.Upgrade();
        }

        public override void Destroy()
        {
            module.Destroy();

            if (pivot != null)
            {
                GameObject.Destroy(pivot.gameObject);
            }
            if (newSpawn != null)
            {
                GameObject.Destroy(newSpawn.gameObject);
            }
        }

        private void FaceTowardsTarget(Transform transform)
        {
            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.TargetLayer, useLayerMask = true };
            for (int i = 0; i < Physics2D.CircleCast(transform.position, targetRadius.Value, Vector2.zero, contactFilter, hits, 0f); i++)
            {
                GameObject receiver = hits[i].collider.gameObject;

                if (source == receiver) continue;
                if (TeamManager.IsAlly(source, receiver)) continue;

                Vector2 direction = (receiver.transform.position - source.transform.position).normalized;
                transform.rotation = Quaternion.Euler(0f, 0f, GetRotation(direction));
            }
        }

        protected float GetRotation(Vector2 direction)
        {
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        }
    }
}
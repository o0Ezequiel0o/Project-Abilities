using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class SpawnHomingOrbs : AbilityModule
    {
        [Header("Spawning")]
        [SerializeField] private HomingOrbProjectile prefab;
        [SerializeField] private float distance;
        [SerializeField] private float spinSpeed;

        [Header("Homing Orbs")]
        [SerializeField] protected Stat amount;
        [SerializeField] protected Stat damage;
        [SerializeField] protected Stat maxRange;
        [SerializeField] protected Stat pierce;
        [SerializeField] protected Stat fireCooldown;

        [Header("Targeting")]
        [SerializeField] private float detectRadius;
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private LayerMask blockLayers;

        [Header("Other")]
        [SerializeField] private float warmUp;

        private GameObject source;

        private Spinner<HomingOrbProjectile> spinnerInstance;

        private readonly List<Transform> targetsInRange = new List<Transform>();

        private bool spinnerCreatedThisFrame = false;
        private bool warmUpFinished = false;

        private float fireCooldownTimer = 0f;
        private float warmUpTimer = 0f;

        private List<HomingOrbProjectile> homingOrbs = new List<HomingOrbProjectile>();

        private readonly List<RaycastHit2D> targetsInLaunchPath = new List<RaycastHit2D>();
        private readonly List<Collider2D> unfilteredTargetsInRange = new List<Collider2D>();

        public SpawnHomingOrbs() { }

        public SpawnHomingOrbs(SpawnHomingOrbs original)
        {
            prefab = original.prefab;
            distance = original.distance;
            spinSpeed = original.spinSpeed;

            detectRadius = original.detectRadius;
            blockLayers = original.blockLayers;
            hitLayers = original.hitLayers;

            warmUp = original.warmUp;

            amount = original.amount.DeepCopy();
            damage = original.damage.DeepCopy();
            pierce = original.pierce.DeepCopy();
            maxRange = original.maxRange.DeepCopy();

            fireCooldown = original.fireCooldown.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new SpawnHomingOrbs(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;

            spinnerInstance = new Spinner<HomingOrbProjectile>();
            spinnerInstance.onInitialization += OnSpinnerInitialization;
        }

        public override bool CanActivate() => true;

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            spinnerInstance.DisablePivotChildren();
            spinnerInstance.InitializeSpinner(null, prefab, distance, spinSpeed, Mathf.FloorToInt(amount.Value));
        }

        public override void Update()
        {
            spinnerInstance.Update();

            if (spinnerCreatedThisFrame)
            {
                spinnerCreatedThisFrame = false;
                return;
            }

            UpdateWarmUp();
            UpdateTimers();
            UpdateFiringState();
        }

        public override void LateUpdate()
        {
            if (spinnerInstance.Pivot != null)
            {
                spinnerInstance.Pivot.transform.position = source.transform.position;
            }
        }

        public override void Upgrade()
        {
            amount.Upgrade();
            damage.Upgrade();
            maxRange.Upgrade();
            fireCooldown.Upgrade();
        }

        public override void Destroy()
        {
            spinnerInstance?.Destroy();
            spinnerInstance = null;
        }

        protected virtual void OnSpinnerInitialization(List<HomingOrbProjectile> spawnedObjects)
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                spawnedObjects[i].Launch(spawnedObjects[i].transform.position, 0f, Vector2.zero, Mathf.Infinity, damage.Value, source, TeamManager.GetTeam(source));
                spawnedObjects[i].ColliderEnabled = false;
            }

            spinnerCreatedThisFrame = true;
            homingOrbs = spawnedObjects;
            ResetWarmUp();
        }

        protected void DestroySpinner()
        {
            spinnerInstance?.Destroy();
            spinnerInstance = null;
        }

        private void UpdateWarmUp()
        {
            if (warmUpFinished) return;

            if (warmUpTimer >= warmUp)
            {
                warmUpFinished = true;
            }
        }

        private void UpdateTimers()
        {
            fireCooldownTimer += Time.deltaTime;
            warmUpTimer += Time.deltaTime;
        }

        private void UpdateFiringState()
        {
            if (fireCooldownTimer <= fireCooldown.Value) return;
            if (spinnerInstance.Pivot == null) return;

            if (spinnerInstance.Pivot.childCount <= 0) return;
            if (!warmUpFinished) return;

            fireCooldownTimer = 0f;

            UpdateTargetsInRange();
            TryFireClosestOrbToTargets(targetsInRange, source, hitLayers, blockLayers);
        }

        private void UpdateTargetsInRange()
        {
            targetsInRange.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, detectRadius, contactFilter, unfilteredTargetsInRange);

            for (int i = 0; i < unfilteredTargetsInRange.Count; i++)
            {
                if (unfilteredTargetsInRange[i].gameObject == source) continue;
                if (TeamManager.IsAlly(unfilteredTargetsInRange[i].gameObject, source)) continue;

                targetsInRange.Add(unfilteredTargetsInRange[i].transform);
            }
        }

        public bool TryFireClosestOrbToTargets(List<Transform> targets, GameObject source, LayerMask hitLayers, LayerMask blockLayers)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (TryGetClosestValidOrbToTarget(targets[i], source, hitLayers, blockLayers, out HomingOrbProjectile homingOrb))
                {
                    FireOrb(homingOrb, targets[i], source);
                    return true;
                }
            }

            return false;
        }

        private bool TryGetClosestValidOrbToTarget(Transform target, GameObject source, LayerMask hitLayers, LayerMask blockLayers, out HomingOrbProjectile closestOrb)
        {
            closestOrb = null;
            float closestDistance = float.PositiveInfinity;

            for (int i = homingOrbs.Count - 1; i >= 0; i--)
            {
                if (homingOrbs[i] == null)
                {
                    homingOrbs.RemoveAt(i);
                    continue;
                }

                float distance = (target.transform.position - homingOrbs[i].transform.position).sqrMagnitude;

                if (distance < closestDistance && ValidOrbLaunch(homingOrbs[i], target, source, hitLayers, blockLayers))
                {
                    closestOrb = homingOrbs[i];
                    closestDistance = distance;
                }
            }

            return closestOrb != null;
        }

        private void FireOrb(HomingOrbProjectile homingOrb, Transform target, GameObject source)
        {
            Vector3 direction = (target.transform.position - homingOrb.transform.position).normalized;

            homingOrbs.Remove(homingOrb);
            spinnerInstance.RemoveFromPivot(homingOrb.transform);

            homingOrb.Launch(homingOrb.transform.position, 5f, direction, maxRange.Value, damage.Value, pierce.ValueInt, source, TeamManager.GetTeam(source));

            homingOrb.SetTarget(target);
            homingOrb.ColliderEnabled = true;
        }

        protected bool InLayerMask(GameObject hit, LayerMask layerMask)
        {
            return (layerMask & 1 << hit.layer) != 0;
        }

        private bool ValidOrbLaunch(HomingOrbProjectile homingOrb, Transform target, GameObject source, LayerMask hitLayers, LayerMask blockLayers)
        {
            targetsInLaunchPath.Clear();

            Vector3 direction = (target.transform.position - homingOrb.transform.position).normalized;
            float distance = Vector3.Distance(homingOrb.transform.position, target.transform.position);

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = hitLayers | blockLayers, useLayerMask = true };
            Physics2D.CircleCast(homingOrb.transform.position, homingOrb.Radius, direction, contactFilter, targetsInLaunchPath, distance);

            for (int i = 0; i < targetsInLaunchPath.Count; i++)
            {
                if (targetsInLaunchPath[i].transform.gameObject == source) return false;
                if (InLayerMask(targetsInLaunchPath[i].transform.gameObject, blockLayers)) return false;
            }

            return true;
        }

        private void ResetWarmUp()
        {
            warmUpFinished = false;
            warmUpTimer = 0f;
        }
    }
}
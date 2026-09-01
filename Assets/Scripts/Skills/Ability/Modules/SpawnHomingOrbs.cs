using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    public class SpawnHomingOrbs : AbilityModule
    {
        private readonly SpawnHomingOrbsData data;

        protected readonly Stat amount;
        protected readonly Stat maxRange;
        protected readonly Stat pierce;
        protected readonly Stat damage;
        protected readonly Stat fireCooldown;

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

        public SpawnHomingOrbs(SpawnHomingOrbsData data, Stat amount, Stat maxRange, Stat pierce, Stat damage, Stat fireCooldown)
        {
            this.data = data;
            this.amount = amount;
            this.maxRange = maxRange;
            this.pierce = pierce;
            this.damage = damage;
            this.fireCooldown = fireCooldown;
        }

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
            spinnerInstance.InitializeSpinner(null, data.Prefab, data.Distance, data.SpinSpeed, Mathf.FloorToInt(amount.Value));
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
                DamageData damageData = new DamageData(damage.Value, data.ArmorPenetration, data.ProcCoefficient);
                spawnedObjects[i].Launch(spawnedObjects[i].transform.position, 0f, Vector2.zero, Mathf.Infinity, damageData, data.Knockback, pierce.ValueInt, source, TeamManager.GetTeam(source));
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

            if (warmUpTimer >= data.WarmUp)
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
            TryFireClosestOrbToTargets(targetsInRange, source, data.HitLayers, data.BlockLayers);
        }

        private void UpdateTargetsInRange()
        {
            targetsInRange.Clear();

            ContactFilter2D contactFilter = new ContactFilter2D() { layerMask = data.HitLayers, useLayerMask = true };
            Physics2D.OverlapCircle(source.transform.position, data.DetectRadius, contactFilter, unfilteredTargetsInRange);

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

            DamageData damageData = new DamageData(damage.Value, data.ArmorPenetration, data.ProcCoefficient);
            homingOrb.Launch(homingOrb.transform.position, 5f, direction, maxRange.Value, damageData, pierce.ValueInt, source, TeamManager.GetTeam(source));

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
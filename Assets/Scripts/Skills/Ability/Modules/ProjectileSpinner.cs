using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ProjectileSpinner : GenericSpinner<SpinnerProjectile>
    {
        [SerializeField] private Stat damage;

        public ProjectileSpinner() { }

        public ProjectileSpinner(ProjectileSpinner original) : base(original)
        {
            damage = original.damage.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new ProjectileSpinner(this);

        public override void Activate(bool holding)
        {
            InitializeSpinner(distance.Value, speed.Value, Mathf.FloorToInt(amount.Value));
        }

        public override void Deactivate()
        {
            spinnerInstance.DisablePivotChildren();
        }

        public override void UpdateActive()
        {
            base.UpdateActive();
        }

        public override void Upgrade()
        {
            base.Upgrade();
            damage.Upgrade();
        }

        protected override void OnSpinnerInitialization(List<SpinnerProjectile> spawnedObjects)
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                spawnedObjects[i].Launch(spawnedObjects[i].transform.position, 0f, Vector2.zero, Mathf.Infinity, damage.Value, source, TeamManager.GetTeam(source));
            }
        }
    }
}
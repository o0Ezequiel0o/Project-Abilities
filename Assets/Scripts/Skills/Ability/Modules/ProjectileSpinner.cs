using System.Collections.Generic;
using UnityEngine;
using System;
using Zeke.TeamSystem;
using static DamageProjectileBase;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class ProjectileSpinner : GenericSpinner<SpinnerProjectile>
    {
        [SerializeField] private Stat damage;
        [SerializeField] private Stat pierce;

        [Space]

        [SerializeField] private float armorPenetration = 0f;
        [SerializeField] private float procCoefficient = 1f;

        public ProjectileSpinner() { }

        public ProjectileSpinner(ProjectileSpinner original) : base(original)
        {
            armorPenetration = original.armorPenetration;
            procCoefficient = original.procCoefficient;

            damage = original.damage.DeepCopy();
            pierce = original.pierce.DeepCopy();
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
            pierce.Upgrade();
        }

        protected override void OnSpinnerInitialization(List<SpinnerProjectile> spawnedObjects)
        {
            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                DamageData damageData = new DamageData(damage.Value, armorPenetration, procCoefficient);
                spawnedObjects[i].Launch(spawnedObjects[i].transform.position, 0f, Vector2.zero, Mathf.Infinity, damageData, pierce.ValueInt, source, TeamManager.GetTeam(source));
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Zeke.TeamSystem;

namespace Zeke.Abilities.Modules
{
    public class ProjectileSpinner : GenericSpinner<SpinnerProjectile>
    {
        private readonly ProjectileSpinnerData data;

        private readonly Stat damage;
        private readonly Stat pierce;

        public ProjectileSpinner(ProjectileSpinnerData data, Stat distance, Stat amount, Stat speed, Stat damage, Stat pierce) : base(data, distance, amount, speed)
        {
            this.data = data;
            this.damage = damage;
            this.pierce = pierce;
        }

        public override void Activate(bool holding)
        {
            InitializeSpinner(distance.Value, speed.Value, Mathf.FloorToInt(amount.Value));
        }

        public override void Deactivate()
        {
            for (int i = 0; i < spinnerInstance.Pool.BusyCount; i++)
            {
                SpinnerProjectile projectile = spinnerInstance.Pool.GetActive(i);
                projectile.Despawn();
            }
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
                DamageData damageData = new DamageData(damage.Value, data.ArmorPenetration, data.ProcCoefficient);
                spawnedObjects[i].Launch(spawnedObjects[i].transform.position, 0f, Vector2.zero, Mathf.Infinity, damageData, pierce.ValueInt, source, TeamManager.GetTeam(source));
            }
        }
    }
}
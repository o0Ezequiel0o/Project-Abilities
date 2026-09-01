using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireBasicLaser : AbilityModule
    {
        private readonly FireBasicLaserData data;

        private readonly Stat damage;
        private readonly Stat maxRange;
        private readonly Stat damageCooldown;

        private Transform spawn;
        private GameObject source;

        private Laser laserInstance = null;

        public FireBasicLaser(FireBasicLaserData data, Stat damage, Stat maxRange, Stat damageCooldown)
        {
            this.data = data;
            this.damage = damage;
            this.maxRange = maxRange;
            this.damageCooldown = damageCooldown;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.spawn = spawn;
            this.source = source;

            GameObject laserGOInstance = GameObject.Instantiate(data.Prefab, source.transform.position, Quaternion.identity);

            if (laserGOInstance.TryGetComponent(out laserInstance))
            {
                laserInstance.SetLaserValues(source, damage.Value, data.Pierce, damageCooldown.Value, data.ArmorPenetration, data.ProcCoefficient);
            }

            laserGOInstance.SetActive(false);
        }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            if (laserInstance == null) return;

            if (!laserInstance.gameObject.activeSelf)
            {
                laserInstance.gameObject.SetActive(true);
            }
        }

        public override void Deactivate()
        {
            if (laserInstance == null) return;

            if (laserInstance.gameObject.activeSelf)
            {
                laserInstance.gameObject.SetActive(false);
            }
        }

        public override void UpdateActive()
        {
            if (laserInstance == null) return;

            laserInstance.UpdateLaser(spawn.position, spawn.rotation, spawn.up, data.Radius, maxRange.Value);
        }

        public override void Upgrade()
        {
            damage.Upgrade();
            maxRange.Upgrade();
            damageCooldown.Upgrade();

            if (laserInstance != null)
            {
                laserInstance.SetLaserValues(source, damage.Value, data.Pierce, damageCooldown.Value, data.ArmorPenetration, data.ProcCoefficient);
            }
        }

        public override void Destroy()
        {
            if (laserInstance == null) return;
            GameObject.Destroy(laserInstance.gameObject);
        }
    }
}
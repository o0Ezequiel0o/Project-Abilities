using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class FireBasicLaser : AbilityModule
    {
        [SerializeField] private GameObject prefab;

        [Space]

        [SerializeField] private Stat damage;
        [SerializeField] private Stat maxRange;

        [Space]

        [SerializeField] private float radius;
        [SerializeField] private int pierce;
        [SerializeField] private Stat damageCooldown;

        private Transform spawn;
        private GameObject source;
        private ModularAbilityController controller;

        private Laser laserInstance = null;
        private bool laserEnabledThisFrame = false;

        public FireBasicLaser(FireBasicLaser original)
        {
            prefab = original.prefab;
            radius = original.radius;
            pierce = original.pierce;

            damage = original.damage.DeepCopy();
            damageCooldown = original.damageCooldown.DeepCopy();
            maxRange = original.maxRange.DeepCopy();
        }

        public override AbilityModule CreateDeepCopy() => new FireBasicLaser(this);

        public override void OnInitialization(ModularAbilityController controller, Transform spawn, GameObject source, ModularAbility ability)
        {
            this.spawn = spawn;
            this.source = source;
            this.controller = controller;

            GameObject laserGOInstance = GameObject.Instantiate(prefab, source.transform.position, Quaternion.identity);

            if (laserGOInstance.TryGetComponent(out laserInstance))
            {
                laserInstance.SetLaserValues(source, damage.Value, pierce, damageCooldown.Value);
            }

            laserGOInstance.SetActive(false);
        }

        public override bool CanActivate()
        {
            return laserInstance != null;
        }

        public override bool CanDeactivate()
        {
            return !laserEnabledThisFrame;
        }

        public override bool CanUpgrade() => true;

        public override void Activate(bool holding)
        {
            laserEnabledThisFrame = true;
        }

        public override void Deactivate()
        {
            DisableLaser();
        }

        public override void LateUpdate()
        {
            if (laserInstance == null) return;

            if (laserEnabledThisFrame)
            {
                laserInstance.UpdateLaser(spawn.position, spawn.rotation, spawn.up, radius, maxRange.Value);
                if (!laserInstance.gameObject.activeSelf) laserInstance.gameObject.SetActive(true);

                laserEnabledThisFrame = false;
            }
            else
            {
                DisableLaser();
            }
        }

        public override void Upgrade()
        {
            damage.Upgrade();
            maxRange.Upgrade();
            damageCooldown.Upgrade();

            if (laserInstance != null)
            {
                laserInstance.SetLaserValues(source, damage.Value, pierce, damageCooldown.Value);
            }
        }

        public override void Destroy()
        {
            if (laserInstance == null) return;
            GameObject.Destroy(laserInstance.gameObject);
        }

        private void DisableLaser()
        {
            if (laserInstance == null) return;

            if (laserInstance.gameObject.activeSelf)
            {
                laserInstance.gameObject.SetActive(false);
            }
        }
    }
}
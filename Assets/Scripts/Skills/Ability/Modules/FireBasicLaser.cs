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

        private Laser laserInstance = null;

        public FireBasicLaser() { }

        public FireBasicLaser(FireBasicLaser original)
        {
            prefab = original.prefab;
            radius = original.radius;
            pierce = original.pierce;

            damage = original.damage.DeepCopy();
            maxRange = original.maxRange.DeepCopy();
            damageCooldown = original.damageCooldown.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new FireBasicLaser(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.spawn = spawn;
            this.source = source;

            GameObject laserGOInstance = GameObject.Instantiate(prefab, source.transform.position, Quaternion.identity);

            if (laserGOInstance.TryGetComponent(out laserInstance))
            {
                laserInstance.SetLaserValues(source, damage.Value, pierce, damageCooldown.Value);
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

            laserInstance.UpdateLaser(spawn.position, spawn.rotation, spawn.up, radius, maxRange.Value);
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
    }
}
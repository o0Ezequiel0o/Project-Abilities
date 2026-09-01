using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class GenericSpinner<T> : AbilityModule where T : Component
    {
        private readonly GenericSpinnerData<T> data;

        protected readonly Stat distance;
        protected readonly Stat amount;
        protected readonly Stat speed;

        protected GameObject source;
        protected Ability ability;
        protected AbilityController controller;

        protected Spinner<T> spinnerInstance;

        public GenericSpinner(GenericSpinnerData<T> data, Stat distance, Stat amount, Stat speed)
        {
            this.data = data;
            this.distance = distance;
            this.amount = amount;
            this.speed = speed;
        }

        protected virtual void OnSpinnerInitialization(List<T> spawnedObjects) { }

        public override bool CanActivate() => true;
        public override bool CanUpgrade() => true;

        public override void Activate(bool holding) { }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            this.source = source;
            this.ability = ability;
            this.controller = controller;

            spinnerInstance = new Spinner<T>();
            spinnerInstance.onInitialization += OnSpinnerInitialization;
        }

        protected void InitializeSpinner(float distance, float speed, int amount)
        {
            if (data.Prefab.TryGetComponent(out T prefabComponent))
            {
                spinnerInstance.InitializeSpinner(null, prefabComponent, distance, speed, amount);
            }
        }

        protected void DestroySpinner()
        {
            spinnerInstance?.Destroy();
            spinnerInstance = null;
        }

        public override void UpdateActive()
        {
            spinnerInstance?.Update();
        }

        public override void LateUpdate()
        {
            if (spinnerInstance.Pivot != null)
            {
                spinnerInstance.Pivot.transform.position = source.transform.position;
            }
        }

        public override void Destroy()
        {
            spinnerInstance?.Destroy();
            spinnerInstance = null;
        }

        public override void Upgrade()
        {
            distance.Upgrade();
            speed.Upgrade();
            amount.Upgrade();
        }
    }
}
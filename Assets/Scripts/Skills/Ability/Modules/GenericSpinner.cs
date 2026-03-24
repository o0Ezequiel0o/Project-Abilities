using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class GenericSpinner<T> : AbilityModule where T : Component
    {
        [SerializeField] private GameObject prefab;

        [Space]

        [SerializeField] protected Stat distance;
        [SerializeField] protected Stat amount;
        [SerializeField] protected Stat speed;

        protected GameObject source;
        protected Ability ability;
        protected AbilityController controller;

        protected Spinner<T> spinnerInstance;

        public GenericSpinner(GenericSpinner<T> original)
        {
            prefab = original.prefab;

            distance = original.distance.DeepCopy();
            amount = original.amount.DeepCopy();
            speed = original.speed.DeepCopy();
        }

        protected virtual void OnSpinnerInitialization(List<T> spawnedObjects) { }

        public override AbilityModule DeepCopy() => new GenericSpinner<T>(this);

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
            if (prefab.TryGetComponent(out T prefabComponent))
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
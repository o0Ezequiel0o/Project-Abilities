using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastWhileActive : AbilityModule
    {
        [SerializeField] private Stat interval;
        [SerializeReferenceDropdown, SerializeReference] private AbilityModule module;

        private bool activatedThisFrame = false;

        private float timer = 0f;

        public CastWhileActive() { }

        public CastWhileActive(CastWhileActive original)
        {
            interval = original.interval.DeepCopy();
            module = original.module.DeepCopy();
        }

        public override AbilityModule DeepCopy() => new CastWhileActive(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            module.OnInitialization(controller, spawn, source, ability);
        }

        public override bool CanActivate() => module.CanActivate();
        public override bool CanUpgrade() => module.CanUpgrade();

        public override void Activate(bool holding)
        {
            activatedThisFrame = true;
        }

        public override void Deactivate()
        {
            module.Deactivate();
        }

        public override void Update()
        {
            module.Update();
        }

        public override void LateUpdate()
        {
            module.LateUpdate();
        }

        public override void UpdateActive()
        {
            module.UpdateActive();

            if (activatedThisFrame)
            {
                activatedThisFrame = false;
                return;
            }

            timer += Time.deltaTime;

            if (!module.CanActivate()) return;

            if (timer > interval.Value)
            {
                module.Activate(false);
                module.Deactivate();

                timer = 0f;
            }
        }

        public override void UpdateUnactive()
        {
            module.UpdateUnactive();
        }

        public override void Upgrade()
        {
            interval.Upgrade();
            module.Upgrade();
        }

        public override void Destroy()
        {
            module.Destroy();
        }
    }
}
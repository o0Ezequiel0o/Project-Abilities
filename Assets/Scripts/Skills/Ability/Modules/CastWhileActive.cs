using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class CastWhileActive : AbilityModule
    {
        [SerializeField] private Stat unactiveLength;
        [SerializeField] private Stat activeLength;
        [SerializeField] private InternalLoopState startState;
        [SerializeReferenceDropdown, SerializeReference] private AbilityModule module;

        private InternalLoopState loopState = InternalLoopState.Unactive;

        private float timer = 0f;

        public CastWhileActive() { }

        public CastWhileActive(CastWhileActive original)
        {
            startState = original.startState;

            unactiveLength = original.unactiveLength.DeepCopy();
            activeLength = original.activeLength.DeepCopy();
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
            loopState = startState;
            timer = 0f;

            if (startState == InternalLoopState.Active)
            {
                module.Activate(holding);
            }
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
            if (loopState == InternalLoopState.Unactive)
            {
                float oldTimerValue = timer;
                timer += Time.deltaTime;

                if (unactiveLength.Value > 0f && oldTimerValue <= 0f)
                {
                    module.UpdateUnactive();
                }

                if (!module.CanActivate()) return;

                if (timer > unactiveLength.Value)
                {
                    module.Activate(false);

                    if (activeLength.Value <= 0f)
                    {
                        module.Deactivate();
                    }
                    else
                    {
                        loopState = InternalLoopState.Active;
                    }

                    timer = 0f;
                }
            }
            else
            {
                timer += Time.deltaTime;

                if (activeLength.Value > 0f)
                {
                    module.UpdateActive();
                }

                if (timer > activeLength.Value)
                {
                    module.Deactivate();

                    if (unactiveLength.Value <= 0f && activeLength.Value > 0f)
                    {
                        module.Activate(false);
                    }
                    else
                    {
                        loopState = InternalLoopState.Unactive;
                    }

                    timer = 0f;
                }
            }
        }

        public override void Upgrade()
        {
            unactiveLength.Upgrade();
            module.Upgrade();
        }

        public override void Destroy()
        {
            module.Destroy();
        }

        private enum InternalLoopState
        {
            Active,
            Unactive
        }
    }
}
using UnityEngine;

namespace Zeke.Abilities.Modules
{
    public class CastWhileActive : AbilityModule
    {
        private readonly CastWhileActiveData data;

        private readonly Stat inactiveLength;
        private readonly Stat activeLength;
        private readonly AbilityModule module;

        private InternalLoopState loopState = InternalLoopState.Inactive;

        private float timer = 0f;

        public CastWhileActive(CastWhileActiveData data, Stat inactiveLength, Stat activeLength, AbilityModule module)
        {
            this.data = data;
            this.inactiveLength = inactiveLength;
            this.activeLength = activeLength;
            this.module = module;
        }

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            module.OnInitialization(controller, spawn, source, ability);
        }

        public override bool CanActivate() => module.CanActivate();
        public override bool CanUpgrade() => module.CanUpgrade();

        public override void Activate(bool holding)
        {
            loopState = data.StartState;
            timer = 0f;

            if (data.StartState == InternalLoopState.Active)
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
            if (loopState == InternalLoopState.Inactive)
            {
                float oldTimerValue = timer;
                timer += Time.deltaTime;

                if (inactiveLength.Value > 0f && oldTimerValue <= 0f)
                {
                    module.UpdateInactive();
                }

                if (!module.CanActivate()) return;

                if (timer > inactiveLength.Value)
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

                    if (inactiveLength.Value <= 0f && activeLength.Value > 0f)
                    {
                        module.Activate(false);
                    }
                    else
                    {
                        loopState = InternalLoopState.Inactive;
                    }

                    timer = 0f;
                }
            }
        }

        public override void Upgrade()
        {
            inactiveLength.Upgrade();
            module.Upgrade();
        }

        public override void Destroy()
        {
            module.Destroy();
        }

        public enum InternalLoopState
        {
            Active,
            Inactive
        }
    }
}
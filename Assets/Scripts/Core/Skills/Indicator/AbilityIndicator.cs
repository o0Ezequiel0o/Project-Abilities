using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Indicators
{
    public class AbilityIndicator
    {
        public readonly AbilityIndicatorData data;

        private List<AbilityIndicatorModule> modules;

        public float FirstHideTime => firstHideTime;
        public float LastHideTime => lastHideTime;

        private float firstHideTime = 0f;
        private float lastHideTime = 0f;

        private float timer = 0f;

        public AbilityIndicator(AbilityIndicatorData data)
        {
            this.data = data;
        }

        public void Initialize(List<AbilityIndicatorModule> modules)
        {
            this.modules = modules;

            firstHideTime = FindFirstHideTime();
            lastHideTime = FindLastHideTime();
        }

        private float FindFirstHideTime()
        {
            float smallestHideTime = float.PositiveInfinity;

            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i] != null && modules[i].data.HideTime < smallestHideTime)
                {
                    smallestHideTime = modules[i].data.HideTime;
                }
            }

            if (smallestHideTime == float.PositiveInfinity)
            {
                smallestHideTime = 0f;
            }

            return smallestHideTime;
        }

        private float FindLastHideTime()
        {
            float biggestHideTime = 0f;

            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i] != null && modules[i].data.HideTime > biggestHideTime)
                {
                    biggestHideTime = modules[i].data.HideTime;
                }
            }

            return biggestHideTime;
        }

        public void Update()
        {
            timer += Time.deltaTime;

            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Update(timer);
            }
        }

        public void LateUpdate()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].LateUpdate();
            }
        }

        public void Disable()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Disable();
            }
        }

        public void Reset(GameObject source, Transform castTransform)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Reset(source, castTransform);
            }

            timer = 0f;
        }

        public void Destroy()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Destroy();
            }
        }
    }
}
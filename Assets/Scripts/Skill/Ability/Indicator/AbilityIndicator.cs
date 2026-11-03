using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Indicators
{
    public class AbilityIndicator
    {
        private readonly List<AbilityIndicatorModule> modules = new List<AbilityIndicatorModule>();

        public float FirstHideTime => firstHideTime;
        public float LastHideTime => lastHideTime;

        private float firstHideTime = 0f;
        private float lastHideTime = 0f;

        private float timer = 0f;

        public void Initialize(GameObject source, Transform spawn, List<AbilityIndicatorModule> modules, AbilityIndicatorSettings settings)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                this.modules.Add(modules[i].DeepCopy());
                this.modules[^1]?.Initialize(source, spawn, settings);
            }

            firstHideTime = FindFirstHideTime();
            lastHideTime = FindLastHideTime();
        }

        private float FindFirstHideTime()
        {
            float smallestHideTime = float.PositiveInfinity;

            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i] != null && modules[i].HideTime < smallestHideTime)
                {
                    smallestHideTime = modules[i].HideTime;
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
                if (modules[i] != null && modules[i].HideTime > biggestHideTime)
                {
                    biggestHideTime = modules[i].HideTime;
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
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].Reset();
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
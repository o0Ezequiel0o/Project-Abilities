using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Modules
{
    [Serializable]
    public class AlternatingCast : AbilityModule
    {
        [SerializeField] private TravelType travelType = TravelType.Ascending;
        [SerializeReference, SerializeReferenceDropdown] public List<AbilityModule> modules;

        private AbilityModule selectedModule;
        private int indexTravelDirection = 1;
        private int currentIndex = 0;

        public AlternatingCast() { }

        public AlternatingCast(AlternatingCast original)
        {
            travelType = original.travelType;
            modules = new List<AbilityModule>();

            for (int i = 0; i < original.modules.Count; i++)
            {
                modules.Add(original.modules[i]?.DeepCopy());
            }
        }

        public override AbilityModule DeepCopy() => new AlternatingCast(this);

        public override void OnInitialization(AbilityController controller, Transform spawn, GameObject source, Ability ability)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i].OnInitialization(controller, spawn, source, ability);
            }
        }

        public override void Activate(bool holding)
        {
            if (modules.Count == 0) return;

            if (modules.Count == 1)
            {
                selectedModule = modules[0];
                selectedModule?.Activate(holding);
                return;
            }

            if (currentIndex < 0)
            {
                OnIndexReachedZero();
            }

            if (currentIndex >= modules.Count)
            {
                OnIndexReachedCount();
            }

            selectedModule = modules[currentIndex];
            selectedModule?.Activate(holding);

            currentIndex += indexTravelDirection;
        }

        public override void Deactivate()
        {
            selectedModule?.Deactivate();
        }

        public override bool CanActivate()
        {
            if (selectedModule == null) return true;
            return selectedModule.CanActivate();
        }

        public override bool CanUpgrade()
        {
            if (selectedModule == null) return true;
            return selectedModule.CanUpgrade();
        }

        public override void Update()
        {
            selectedModule?.Update();
        }

        public override void UpdateActive()
        {
            selectedModule?.UpdateActive();
        }

        public override void UpdateUnactive()
        {
            selectedModule?.UpdateUnactive();
        }

        public override void LateUpdate()
        {
            selectedModule?.LateUpdate();
        }

        public override void Upgrade()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i]?.Upgrade();
            }
        }

        public override void Destroy()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                modules[i]?.Destroy();
            }
        }

        private void OnIndexReachedZero()
        {
            if (travelType == TravelType.Alternating)
            {
                if (indexTravelDirection == -1)
                {
                    indexTravelDirection = 1;
                    currentIndex += 2;
                }
            }
        }

        private void OnIndexReachedCount()
        {
            if (travelType == TravelType.Alternating)
            {
                if (indexTravelDirection == 1)
                {
                    indexTravelDirection = -1;
                    currentIndex -= 2;
                }
            }
            else
            {
                currentIndex = 0;
            }
        }

        private enum TravelType
        {
            Ascending,
            Alternating
        }
    }
}
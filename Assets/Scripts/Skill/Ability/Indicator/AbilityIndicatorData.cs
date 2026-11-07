using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Indicators
{
    [CreateAssetMenu(fileName = "New Ability Indicator", menuName = "ScriptableObjects/Ability/Create Ability Indicator", order = 1)]
    public class AbilityIndicatorData : ScriptableObject
    {
        [SerializeField] private AbilityIndicatorSettings settings;
        [SerializeField] private List<AbilityIndicatorModuleData> modules;

        public AbilityIndicator CreateAbilityIndicator(GameObject source, Transform castTransform)
        {
            AbilityIndicator abilityIndicator = new AbilityIndicator(this);
            List<AbilityIndicatorModule> modulesInstance = new List<AbilityIndicatorModule>();

            for (int i = 0; i < modules.Count; i++)
            {
                modulesInstance.Add(modules[i].CreateModule());
                modulesInstance[^1].Initialize(source, castTransform, settings);
            }

            abilityIndicator.Initialize(modulesInstance);

            return abilityIndicator;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Indicators
{
    [CreateAssetMenu(fileName = "New Ability Indicator", menuName = "ScriptableObjects/Ability/Create Ability Indicator", order = 1)]
    public class AbilityIndicatorData : ScriptableObject
    {
        [SerializeField] private AbilityIndicatorSettings settings;
        [SerializeField] private List<AbilityIndicatorModule> modules;

        public AbilityIndicator CreateAbilityIndicator(GameObject source, Transform castTransform)
        {
            AbilityIndicator abilityIndicator = new AbilityIndicator();
            abilityIndicator.Initialize(source, castTransform, modules, settings);

            return abilityIndicator;
        }
    }
}
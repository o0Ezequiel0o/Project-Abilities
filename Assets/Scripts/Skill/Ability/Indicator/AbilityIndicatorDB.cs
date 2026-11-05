using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Indicators
{
    [CreateAssetMenu(fileName = "Ability Indicator DB", menuName = "ScriptableObjects/Ability/New Ability Indicator DB", order = 1)]
    public class AbilityIndicatorDB : ScriptableObject
    {
        [SerializeField] private Dictionary<AbilityData, AbilityIndicatorData> indicators = new Dictionary<AbilityData, AbilityIndicatorData>();

        public bool TryGetAbilityIndicator(AbilityData abilityData, out AbilityIndicatorData abilityIndicatorData)
        {
            return indicators.TryGetValue(abilityData, out abilityIndicatorData);
        }
    }
}
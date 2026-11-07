using System.Collections.Generic;
using UnityEngine;

namespace Zeke.Abilities.Indicators
{
    public class AbilityIndicatorPool : GameService<AbilityIndicatorPool>
    {
        [SerializeField] private AbilityIndicatorDB abilityIndicatorDB;

        private Dictionary<AbilityIndicatorData, Stack<AbilityIndicator>> abilityIndicators = new Dictionary<AbilityIndicatorData, Stack<AbilityIndicator>>();

        public AbilityIndicator Get(AbilityIndicatorData indicatorData, GameObject source, Transform castPosition)
        {
            if (abilityIndicators.TryGetValue(indicatorData, out Stack<AbilityIndicator> indicators))
            {
                if (indicators.Count > 0)
                {
                    AbilityIndicator abilityIndicator = indicators.Pop();
                    abilityIndicator.Reset(source, castPosition);

                    return abilityIndicator;
                }
            }

            return indicatorData.CreateAbilityIndicator(source, castPosition);
        }

        public void Return(AbilityIndicator indicator)
        {
            if (abilityIndicators.TryGetValue(indicator.data, out Stack<AbilityIndicator> indicators))
            {
                if (indicators == null)
                {
                    abilityIndicators[indicator.data] = new Stack<AbilityIndicator>();
                }

                indicators.Push(indicator);
                return;
            }

            abilityIndicators.Add(indicator.data, new Stack<AbilityIndicator>());
            abilityIndicators[indicator.data].Push(indicator);
        }
    }
}
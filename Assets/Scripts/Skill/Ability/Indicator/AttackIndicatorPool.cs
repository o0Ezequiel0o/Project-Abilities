using System.Collections.Generic;
using UnityEngine;
using Zeke.Abilities.Indicators;

public class AbilityIndicatorPool : GameService<AbilityIndicatorPool>
{
    [SerializeField] private AbilityIndicatorDB abilityIndicatorDB;

    private Dictionary<AbilityIndicatorData, AbilityIndicator> abilityIndicators = new Dictionary<AbilityIndicatorData, AbilityIndicator>();


}
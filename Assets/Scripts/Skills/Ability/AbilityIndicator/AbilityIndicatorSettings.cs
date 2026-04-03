using UnityEngine;

namespace Zeke.Abilities.Indicators
{
    [CreateAssetMenu(fileName = "Ability Indicator Settings", menuName = "ScriptableObjects/Ability/Ability Indicator Settings", order = 1)]
    public class AbilityIndicatorSettings : ScriptableObject
    {
        [field: SerializeField] public AttackIndicator Prefab { get; private set; }
        [field: SerializeField] public Color FillColor { get; private set; }
        [field: SerializeField] public Color BackgroundColor { get; private set; }
    }
}
using UnityEngine;

namespace Zeke.Abilities.Ascendancy
{
    [CreateAssetMenu(fileName = "New Ascendancy Data", menuName = "ScriptableObjects/Ascendancy/Create Ascendancy Data", order = 1)]
    public class AscendancyData : ScriptableObject
    {
        [field: Header("Display")]
        [field: SerializeField] public string Title { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        [field: Space]

        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: Header("Settings")]
        [field: SerializeField] public AbilityData AbilityData { get; private set; }
    }
}
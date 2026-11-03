using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zeke.Abilities.Ascendancy
{
    [CreateAssetMenu(fileName = "New Ascendances Data", menuName = "ScriptableObjects/Ascendancy/Create Ascendances Data", order = 1)]
    public class AscendanciesData : ScriptableObject
    {
        [SerializeField] private Dictionary<AbilityData, AscendanceElement> elements;

        public IReadOnlyDictionary<AbilityData, AscendanceElement> Elements => elements;
    }

    [Serializable]
    public class AscendanceElement
    {
        [field: SerializeField, Min(1)] public int RequiredLevel { get; private set; }
        [SerializeField] private List<AscendancyData> options;

        public IReadOnlyList<AscendancyData> Options => options;
    }
}
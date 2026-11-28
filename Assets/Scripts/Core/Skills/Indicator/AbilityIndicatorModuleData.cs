using UnityEngine;
using System;

namespace Zeke.Abilities.Indicators
{
    [Serializable]
    public class AbilityIndicatorModuleData
    {
        [field: SerializeField] public AttackIndicatorData AttackIndicator { get; private set; }

        [field: SerializeField] public StartingPoint SpawnPosition { get; private set; }
        [field: SerializeField] public Vector2 Offset { get; private set; }
        [field: SerializeField] public float AngleOffset { get; private set; }

        [field: SerializeField] public float ShowTime { get; private set; }
        [field: SerializeField] public float HideTime { get; private set; }

        public enum StartingPoint
        {
            [InspectorName("Cast Position")]
            CastPosition,
            [InspectorName("Source Center")]
            Source
        }

        public AbilityIndicatorModule CreateModule()
        {
            return new AbilityIndicatorModule(this);
        }
    }
}

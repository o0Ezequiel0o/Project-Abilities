using UnityEngine;

[CreateAssetMenu(fileName = "New Preset", menuName = "AI Presets/Bodyguard/New Preset")]
public class BodyguardAISettings : ScriptableObject
{
    [field: Header("Targeting")]
    [field: SerializeField] public LayerMask TargetLayers { get; private set; }
    [field: SerializeField] public LayerMask BlockLayers { get; private set; }

    [field: Header("Protecting")]
    [field: SerializeField] public float MinFollowDistance { get; private set; }
    [field: SerializeField] public float ReturnLockStateTime { get; private set; }
    [field: SerializeField] public float MaxDistanceFromProtectTarget { get; private set; }

    [field: Header("Attacking")]
    [field: SerializeField] public float EngageRange { get; private set; }
    [field: SerializeField] public AbilityType AttackType { get; private set; }

    [field: Header("Fleeting")]
    [field: SerializeField] public float FleetingRange { get; private set; }

    [field: Header("Performance")]
    [field: SerializeField] public int AvoidanceDirections { get; private set; } = 8;
}
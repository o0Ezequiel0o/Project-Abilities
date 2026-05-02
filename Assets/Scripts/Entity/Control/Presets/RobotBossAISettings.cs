using UnityEngine;
using Zeke.Abilities;

[CreateAssetMenu(fileName = "New Preset", menuName = "AI Presets/Robot Boss/New Preset")]
public class RobotBossAISettings : ScriptableObject
{
    [field: Header("Targeting")]
    [field: SerializeField] public LayerMask TargetLayers { get; private set; }
    [field: SerializeField] public LayerMask BlockLayers { get; private set; }

    [field: Header("Attack")]
    [field: SerializeField] public AbilityType PrimaryAttackType { get; private set; }
    [field: SerializeField] public float PrimaryStartRange { get; private set; }
    [field: SerializeField] public float PrimaryAttackRecover { get; private set; }

    [field: Space]

    [field: SerializeField] public AbilityType SecondaryAttackType { get; private set; }
    [field: SerializeField] public float SecondaryStartRange { get; private set; }
    [field: SerializeField] public float SecondaryAttackRange { get; private set; }

    [field: Space]

    [field: SerializeField] public float MinStartAttackAngle { get; private set; }

    [field: Header("Sounds")]
    [field: SerializeField] public Sound WindUpSound { get; private set; }
}
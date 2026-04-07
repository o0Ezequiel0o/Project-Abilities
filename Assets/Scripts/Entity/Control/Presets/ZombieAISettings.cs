using UnityEngine;
using Zeke.Abilities;

[CreateAssetMenu(fileName = "New Preset", menuName = "AI Presets/Zombie/New Preset")]
public class ZombieAISettings : ScriptableObject
{
    [field: Header("Targeting")]
    [field: SerializeField] public LayerMask TargetLayers { get; private set; }
    [field: SerializeField] public LayerMask BlockLayers { get; private set; }

    [field: Header("Attack")]
    [field: SerializeField] public AbilityType AttackType { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }

    [field: Space]

    [field: SerializeField] public float MinStartAttackAngle { get; private set; }
    [field: SerializeField] public float AttackRecover { get; private set; }

    [field: Header("Sounds")]
    [field: SerializeField] public Sound WindUpSound { get; private set; }
}
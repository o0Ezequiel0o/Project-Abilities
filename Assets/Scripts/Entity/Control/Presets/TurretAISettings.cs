using UnityEngine;

[CreateAssetMenu(fileName = "New Preset", menuName = "AI Presets/Turret/New Preset")]
public class TurretAISettings : ScriptableObject
{
    [field: Header("Targeting")]
    [field: SerializeField] public LayerMask TargetLayers { get; private set; }
    [field: SerializeField] public LayerMask BlockLayers { get; private set; }

    [field: Header("Attacking")]
    [field: SerializeField] public AbilityType AttackType { get; private set; }
    [field: SerializeField] public float Range { get; private set; }
}
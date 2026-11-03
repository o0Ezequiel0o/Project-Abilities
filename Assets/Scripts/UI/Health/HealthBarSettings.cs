using UnityEngine;

[CreateAssetMenu(fileName = "Health Bar Settings", menuName = "Health Bar Settings", order = 1)]
public class HealthBarSettings : ScriptableObject
{
    [field: SerializeField] public Color HealthColor { get; private set; }
    [field: SerializeField] public Color ShieldColor { get; private set; }

    [field: Space]

    [field: SerializeField] public float ChipTime { get; private set; }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Numbers Config", menuName = "ScriptableObjects/DamageNumbers/Config", order = 1)]
public class DamageNumbersConfig : ScriptableObject
{
    [field: Header("Settings")]
    [field: SerializeField] public Color DefaultColor { get; private set; } = Color.red;

    [field: Header("Despawning")]
    [field: SerializeField, Range(0f, 1f)] public float AlphaStartTime { get; private set; } = 0.7f;
    [field: SerializeField, Range(0f, 1f)] public float StartAlpha { get; private set; } = 1f;

    [field: Header("Animation")]
    [field: SerializeField] public float FloatSpeed { get; private set; } = 5f;
}
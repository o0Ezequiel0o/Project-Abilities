using UnityEngine;

[CreateAssetMenu(fileName = "Spawnable", menuName = "ScriptableObjects/Spawnable", order = 1)]
public class Spawnable : ScriptableObject, IWeighted
{
    [field: SerializeField] public Sprite Icon { get; private set; }

    [field: Header("Settings")]
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [field: Space]

    [field: SerializeField, Min(0)] public float MinDifficulty { get; private set; } = 0f;
    [field: SerializeField, Min(0)] public float MaxDifficulty { get; private set; } = float.PositiveInfinity;

    [field: Space]

    [field: SerializeField, Min(1)] public int Weight { get; private set; } = 50;
    [field: SerializeField, Min(1)] public int Cost { get; private set; } = 4;
}
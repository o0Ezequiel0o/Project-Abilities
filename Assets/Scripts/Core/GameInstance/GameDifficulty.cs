using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "New Difficulty", order = 1)]
public class GameDifficulty : ScriptableObject
{
    [field: Header("Enemies")]
    [field: SerializeField] public float StartingDifficulty { get; private set; } = 0f;
    [field: SerializeField] public float DifficultyScaleRate { get; private set; } = 0f;
    [field: SerializeField] public float DifficultyRampUp { get; private set; } = 0f;

    [field: Header("Economy")]
    [field: SerializeField] public float PriceScalePerSecond { get; private set; } = 0f;
}
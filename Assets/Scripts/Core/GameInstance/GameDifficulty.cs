using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "New Difficulty", order = 1)]
public class GameDifficulty : ScriptableObject
{
    [field: Header("Display")]
    [field: SerializeField] public string Name { get; private set; }

    [field: Header("Enemies")]
    [field: SerializeField] public float StartingDifficulty { get; private set; } = 0f;
    [field: SerializeField] public float DifficultyScaleRate { get; private set; } = 0f;
    [field: SerializeField] public float DifficultyRampUp { get; private set; } = 0f;

    [field: Header("Economy")]
    [field: SerializeField] public float PriceScalePerSecond { get; private set; } = 0f;

    public void LoadDifficulty(GameDifficulty difficulty)
    {
        Name = difficulty.Name;

        StartingDifficulty = difficulty.StartingDifficulty;
        DifficultyScaleRate = difficulty.DifficultyScaleRate;
        DifficultyRampUp = difficulty.DifficultyRampUp;

        PriceScalePerSecond = difficulty.PriceScalePerSecond;
    }
}
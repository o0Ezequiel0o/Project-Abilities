using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "New Difficulty", order = 1)]
public class GameDifficulty : ScriptableObject
{
    [field: Header("Display")]
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField, TextArea(5, 5)] public string Description { get; private set; }

    [field: Space]

    [field: SerializeField] public Color Color { get; private set; } = Color.white;

    [field: Header("Difficulty")]
    [field: SerializeField] public float StartingDifficulty { get; private set; } = 0f;
    [field: SerializeField] public float DifficultyScaleRate { get; private set; } = 0f;
    [field: SerializeField] public float DifficultyRampUp { get; private set; } = 0f;

    [field: Space]

    [Tooltip("How much the difficulty increases based on your current level when you pass to the next one.")]
    [field: SerializeField] public float DifficultyPerLevel { get; private set; } = 3f;

    [field: Header("Economy")]
    [field: SerializeField] public float PriceScalePerSecond { get; private set; } = 0f;

    public void LoadDifficulty(GameDifficulty difficulty)
    {
        Name = difficulty.Name;
        Icon = difficulty.Icon;

        StartingDifficulty = difficulty.StartingDifficulty;
        DifficultyScaleRate = difficulty.DifficultyScaleRate;
        DifficultyRampUp = difficulty.DifficultyRampUp;

        DifficultyPerLevel = difficulty.DifficultyPerLevel;

        PriceScalePerSecond = difficulty.PriceScalePerSecond;
    }
}
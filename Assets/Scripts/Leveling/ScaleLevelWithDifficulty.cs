using UnityEngine;

public class ScaleLevelWithDifficulty : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private LevelHandler levelHandler;
    [SerializeField] private ScaleLevelWithDifficultySettings settings;

    private void Reset()
    {
        levelHandler = GetComponentInChildren<LevelHandler>();
    }

    private void Start()
    {
        int levels = Mathf.FloorToInt(settings.LevelPerDifficulty * GameInstance.Difficulty);

        for (int i = 0; i < levels; i++)
        {
            levelHandler.GiveExperience(levelHandler.ExperienceRequired);
        }
    }
}
using UnityEngine;

public class ScaleLevelWithDifficulty : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private LevelHandler levelHandler;

    [Header("Settings")]
    [SerializeField] private int experiencePerDifficulty = 20;

    private void Reset()
    {
        levelHandler = GetComponentInChildren<LevelHandler>();
    }

    private void Start()
    {
        int experience = Mathf.FloorToInt(experiencePerDifficulty * GameInstance.Difficulty);
        levelHandler.GiveExperience(experience);
    }
}
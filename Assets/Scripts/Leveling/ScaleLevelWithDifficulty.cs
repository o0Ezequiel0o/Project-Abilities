using UnityEngine;
using Zeke.PoolableGameObjects;

public class ScaleLevelWithDifficulty : MonoBehaviour, IPoolableGameObjectListener
{
    [Header("Dependency")]
    [SerializeField] private LevelHandler levelHandler;
    [SerializeField] private ScaleLevelWithDifficultySettings settings;

    public void OnRetrievedFromPool()
    {
        ScaleLevel();
    }

    public void OnSentToPool() { }

    private void Reset()
    {
        levelHandler = GetComponentInChildren<LevelHandler>();
    }

    private void Awake()
    {
        int levels = Mathf.FloorToInt(settings.LevelPerDifficulty * GameInstance.Difficulty);

        for (int i = 0; i < levels; i++)
        {
            levelHandler.GiveExperience(levelHandler.ExperienceRequired);
        }
    }

    private void ScaleLevel()
    {
        int levels = Mathf.FloorToInt(settings.LevelPerDifficulty * GameInstance.Difficulty);

        for (int i = 0; i < levels; i++)
        {
            levelHandler.GiveExperience(levelHandler.ExperienceRequired);
        }
    }
}
using System;
using UnityEngine;
using Zeke.Collections;

public class LevelHandler : MonoBehaviour
{
    [field: SerializeField] public Stat ExperienceMultiplier { get; private set; }

    public int Level { get; private set; } = 1;

    public int Experience => experience;
    public int ExperienceRequired => experienceRequired;

    public readonly OrderedAction<LevelUpInfo> onLevelUp = new OrderedAction<LevelUpInfo>();
    public readonly OrderedAction<int> onReceiveExperience = new OrderedAction<int>();

    private int experience = 0;
    private int experienceRequired = 0;

    private IUpgradable[] upgradableComponents;

    public readonly struct LevelUpInfo
    {
        public readonly int oldLevel;
        public readonly int newLevel;

        public readonly int levels;

        public LevelUpInfo(int oldLevel, int newLevel, int levels)
        {
            this.oldLevel = oldLevel;
            this.newLevel = newLevel;
            this.levels = levels;
        }
    }

    public void GiveExperience(int experienceGained)
    {
        int experienceReceived = Mathf.FloorToInt(experienceGained * ExperienceMultiplier.Value);
        experience += experienceReceived;
        onReceiveExperience?.Invoke(experienceReceived);

        int levelsToGain = CalculateLevelsToGain();

        if (levelsToGain > 0)
        {
            experience -= CalculateExperienceForLevels(levelsToGain);
            IncreaseLevel(levelsToGain);
        }
    }

    private int CalculateLevelsToGain()
    {
        double a = 75.0;
        double b = 150.0 * Level - 125.0;
        double c = -experience;

        double discriminant = (b * b) - (4.0 * a * c);
        if (discriminant < 0) return 0;

        double k = (-b + Math.Sqrt(discriminant)) / (2.0 * a);
        return Mathf.FloorToInt((float)k);
    }

    private int CalculateExperienceForLevels(int k)
    {
        long totalXP = 75L * k * k + (150L * Level - 125L) * k;
        return (int)totalXP;
    }

    public void IncreaseLevel(int levels)
    {
        Level += levels;

        CalculateNextLevelExperience();
        UpgradeComponents(levels);

        onLevelUp?.Invoke(new LevelUpInfo(Level - levels, Level, levels));
    }

    private void Awake()
    {
        upgradableComponents = GetComponentsInChildren<IUpgradable>();
        CalculateNextLevelExperience();
    }

    private void UpgradeComponents(int levels)
    {
        for (int i = 0; i < upgradableComponents.Length; i++)
        {
            upgradableComponents[i].Upgrade(levels);
        }
    }

    private void CalculateNextLevelExperience()
    {
        experienceRequired = (150 * Level) - 50;
    }
}
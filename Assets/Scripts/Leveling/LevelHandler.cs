using System;
using UnityEngine;
using Zeke.Collections;

public class LevelHandler : MonoBehaviour
{
    [field: SerializeField] public Stat ExperienceMultiplier { get; private set; }

    public int Level { get; private set; } = 1;

    public int Experience => experience;
    public int ExperienceRequired => experienceRequired;

    public readonly OrderedAction<int> onLevelUp = new OrderedAction<int>();
    public readonly OrderedAction<int> onReceiveExperience = new OrderedAction<int>();

    private int experience = 0;
    private int experienceRequired = 0;

    private IUpgradable[] upgradableComponents;

    public void GiveExperience(int experienceGained)
    {
        int experienceReceived = Mathf.FloorToInt(experienceGained * ExperienceMultiplier.Value);

        experience += Mathf.FloorToInt(experienceReceived);
        onReceiveExperience?.Invoke(experienceReceived);

        while (experience >= experienceRequired)
        {
            LevelUp();
            HandleOverflowXP();
        }
    }

    public void IncreaseLevel(int levels)
    {
        for (int i = 0; i < levels; i++)
        {
            Level += 1;
            UpgradeComponents();
            onLevelUp?.Invoke(Level);
        }

        CalculateNextLevelExperience();
    }

    private void Awake()
    {
        CalculateNextLevelExperience();
        upgradableComponents = GetComponentsInChildren<IUpgradable>();
    }

    private void LevelUp()
    {
        IncreaseLevel(1);
    }

    private void HandleOverflowXP()
    {
        experience -= experienceRequired;
    }

    private void UpgradeComponents()
    {
        for (int i = 0; i < upgradableComponents.Length; i++)
        {
            upgradableComponents[i].Upgrade();
        }
    }

    private void CalculateNextLevelExperience()
    {
        experienceRequired = (100 * Level) + 50 * (Level - 1);
    }
}
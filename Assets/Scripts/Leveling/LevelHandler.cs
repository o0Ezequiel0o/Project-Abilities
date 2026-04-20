using UnityEngine;
using Zeke.Collections;

public class LevelHandler : MonoBehaviour
{
    [field: SerializeField] public Stat ExperienceMultiplier { get; private set; }

    public int Level { get; private set; }

    public int Experience => experience;
    public int ExperienceRequired => experienceRequired;

    public OrderedAction<int> onLevelUp = new OrderedAction<int>();
    public OrderedAction<int> onReceiveExperience = new OrderedAction<int>();

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
        }
    }

    private void Awake()
    {
        Level = 1;
        CalculateNextLevelExperience();
        upgradableComponents = GetComponentsInChildren<IUpgradable>();
    }

    private void LevelUp()
    {
        Level += 1;
        UpgradeComponents();

        onLevelUp?.Invoke(Level);
        experience -= experienceRequired;

        CalculateNextLevelExperience();
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
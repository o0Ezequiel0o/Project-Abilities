using UnityEngine.UI;
using UnityEngine;
using Zeke.UI;
using TMPro;
using Zeke.Abilities;

public class CharacterSelectViewer : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private CharacterSelector characterSelector;
    [SerializeField] private RectTransform root;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Icon")]
    [SerializeField] private Image icon;

    [Header("Stats")]
    [SerializeField] private StatusBar durabilityBar;
    [SerializeField] private StatusBar offensiveBar;
    [SerializeField] private StatusBar defensiveBar;
    [SerializeField] private StatusBar mobilityBar;
    [SerializeField] private StatusBar aoeBar;

    [Header("Elements")]
    [SerializeField] private UIWindow skillWindowPrefab;

    private void Awake()
    {
        characterSelector.onSpawnableSelected += UpdateDisplay;
    }

    private void UpdateDisplay(Spawnable spawnable)
    {
        ClearRootElements();

        nameText.SetText(spawnable.Name);
        descriptionText.SetText(spawnable.Description);

        icon.sprite = spawnable.Icon;

        durabilityBar.UpdateBar(spawnable.DurabilityRating, 100f);
        offensiveBar.UpdateBar(spawnable.OffensiveRating, 100f);
        defensiveBar.UpdateBar(spawnable.DefensiveRating, 100f);
        mobilityBar.UpdateBar(spawnable.MobilityRating, 100f);
        aoeBar.UpdateBar(spawnable.AoeRating, 100f);

        if (spawnable.Prefab.TryGetComponent(out AbilityController abilityController))
        {
            foreach (AbilityType abilityType in abilityController.StartAbilities.Keys)
            {
                CreateAbilityWindow(abilityController.StartAbilities[abilityType], abilityType);
            }
        }
        if (spawnable.Prefab.TryGetComponent(out PassiveController passiveController))
        {
            for (int i = 0; i < passiveController.DefaultPassives.Count; i++)
            {
                CreatePassiveWindow(passiveController.DefaultPassives[i], i);
            }
        }
    }

    private void CreateAbilityWindow(AbilityData abilityData, AbilityType abilityType)
    {
        UIWindow abilityWindow = Instantiate(skillWindowPrefab, root);

        abilityWindow.TryGetElement<Image>("Icon").sprite = abilityData.Icon;
        abilityWindow.TryGetElement<TextMeshProUGUI>("Name").text = abilityData.Name;
        abilityWindow.TryGetElement<TextMeshProUGUI>("Description").text = abilityData.Description;

        abilityWindow.TryGetElement<TextMeshProUGUI>("Type").text = abilityType.ToString();

        string cooldownText = $"[CD: {abilityData.CooldownTime:0.##}s]".Replace(",", ".");
        abilityWindow.TryGetElement<TextMeshProUGUI>("Cooldown").text = cooldownText;
    }

    private void CreatePassiveWindow(PassiveData passiveData, int index)
    {
        UIWindow abilityWindow = Instantiate(skillWindowPrefab, root);

        abilityWindow.TryGetElement<Image>("Icon").sprite = passiveData.Icon;
        abilityWindow.TryGetElement<TextMeshProUGUI>("Name").text = passiveData.Name;
        abilityWindow.TryGetElement<TextMeshProUGUI>("Description").text = passiveData.Description;

        abilityWindow.TryGetElement<TextMeshProUGUI>("Type").text = $"Passive {index + 1}";

        abilityWindow.TryGetElement<TextMeshProUGUI>("Cooldown").text = "";
    }

    private void ClearRootElements()
    {
        foreach (Transform children in root.transform)
        {
            Destroy(children.gameObject);
        }
    }
}
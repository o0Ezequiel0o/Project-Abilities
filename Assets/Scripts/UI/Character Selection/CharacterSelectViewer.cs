using UnityEngine.UI;
using UnityEngine;
using Zeke.UI;
using TMPro;
using Zeke.Abilities;

public class CharacterSelectViewer : MonoBehaviour
{
    [SerializeField] private CharacterSelector characterSelector;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private RectTransform root;

    [Space]

    [SerializeField] private UIWindow abilityWindowPrefab;

    private void Awake()
    {
        characterSelector.onSpawnableSelected += UpdateDisplay;
    }

    private void UpdateDisplay(Spawnable spawnable)
    {
        ClearRootElements();

        nameText.text = spawnable.Name;

        if (spawnable.Prefab.TryGetComponent(out AbilityController abilityController))
        {
            for (int i = 0; i < abilityController.SpawnAbilities.Count; i++)
            {
                CreateAbilityWindow(abilityController.SpawnAbilities[i]);
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

    private void CreateAbilityWindow(AbilityData abilityData)
    {
        UIWindow abilityWindow = Instantiate(abilityWindowPrefab, root);

        abilityWindow.TryGetElement<Image>("Icon").sprite = abilityData.Icon;
        abilityWindow.TryGetElement<TextMeshProUGUI>("Name").text = abilityData.Name;
        abilityWindow.TryGetElement<TextMeshProUGUI>("Description").text = abilityData.Description;

        abilityWindow.TryGetElement<TextMeshProUGUI>("Type").text = abilityData.AbilityType.ToString();

        string cooldownText = $"[CD: {abilityData.CooldownTime:0.##}s]".Replace(",", ".");
        abilityWindow.TryGetElement<TextMeshProUGUI>("Cooldown").text = cooldownText;
    }

    private void CreatePassiveWindow(PassiveData passiveData, int index)
    {
        UIWindow abilityWindow = Instantiate(abilityWindowPrefab, root);

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
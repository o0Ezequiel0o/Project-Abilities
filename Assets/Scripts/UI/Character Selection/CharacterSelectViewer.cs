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
    }

    private void CreateAbilityWindow(AbilityData abilityData)
    {
        UIWindow abilityWindow = Instantiate(abilityWindowPrefab, root);

        abilityWindow.TryGetElement<Image>("Icon").sprite = abilityData.Icon;
        abilityWindow.TryGetElement<TextMeshProUGUI>("Type").text = abilityData.AbilityType.ToString();
        abilityWindow.TryGetElement<TextMeshProUGUI>("Name").text = abilityData.Name.ToString();
        abilityWindow.TryGetElement<TextMeshProUGUI>("Description").text = abilityData.Description.ToString();
    }

    private void ClearRootElements()
    {
        foreach (Transform children in root.transform)
        {
            Destroy(children.gameObject);
        }
    }
}
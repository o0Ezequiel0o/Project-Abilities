using UnityEngine;
using Zeke.Abilities;

public class AbilityScreenLoader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AbilityControllerInterface window;

    private AbilityController abilityController;

    private bool playerHasRequiredComponents = false;

    private void Awake()
    {
        Run.Instance.onPlayerAdded += OnPlayerAdded;
    }

    private void OnPlayerAdded(Player newPlayer)
    {
        Run.Instance.onPlayerAdded -= OnPlayerAdded;

        playerHasRequiredComponents = newPlayer.TryGetComponent(out abilityController);

        if (!playerHasRequiredComponents) return;

        SubscribeToEvents();
        LoadInterfaceData();
    }

    private void Update()
    {
        if (playerHasRequiredComponents)
        {
            UpdateSkillsInterface();
        }
    }

    private void SubscribeToEvents()
    {
        abilityController.onAbilityAdded += window.AddAbilitySlot;
        abilityController.onAbilityRemoved += window.RemoveAbilitySlot;

        abilityController.onAbilityOverriden += OnAbilityOverriden;
    }

    private void UpdateSkillsInterface()
    {
        foreach (AbilityController.AbilityTypeSlot abilityTypeSlot in abilityController.AbilityTypeSlots.Values)
        {
            if (abilityTypeSlot.CurrentAbilitySlot == null) continue;
            window.UpdateAbilitySlotRender(abilityTypeSlot.CurrentAbilitySlot);
        }
    }

    private void LoadInterfaceData()
    {
        window.LoadData(abilityController.AbilityTypeSlots);
    }

    private void OnAbilityOverriden(AbilityController.AbilityOverrideData abilityOverrideData)
    {
        window.RemoveAbilitySlot(abilityOverrideData.overridenAbility);
        window.AddAbilitySlot(abilityOverrideData.overrideAbility);
    }
}
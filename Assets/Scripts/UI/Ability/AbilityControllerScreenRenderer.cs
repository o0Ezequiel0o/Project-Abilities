using UnityEngine;
using Zeke.Abilities;

[RequireComponent(typeof(AbilityController))]
public class AbilityControllerScreenRenderer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AbilityControllerInterface interfacePrefab;

    private AbilityControllerInterface interfaceInstance;
    private AbilityController abilityController;

    private void Awake()
    {
        abilityController = GetComponent<AbilityController>();
    }

    private void Start()
    {
        SpawnInterfaceInCanvas();
        SubscribeToEvents();
        LoadInterfaceData();
    }

    private void Update()
    {
        UpdateSkillsInterface();
    }

    private void SpawnInterfaceInCanvas()
    {
        interfaceInstance = Instantiate(interfacePrefab, Run.Instance.ScreenCanvas.transform);
    }

    private void SubscribeToEvents()
    {
        abilityController.onAbilityAdded += interfaceInstance.AddAbilitySlot;
        abilityController.onAbilityRemoved += interfaceInstance.RemoveAbilitySlot;

        abilityController.onAbilityOverriden += OnAbilityOverriden;
    }

    private void UpdateSkillsInterface()
    {
        foreach (AbilityController.AbilityTypeSlot abilityTypeSlot in abilityController.AbilityTypeSlots.Values)
        {
            if (abilityTypeSlot.CurrentAbilitySlot == null) continue;

            interfaceInstance.UpdateAbilitySlotRender(abilityTypeSlot.CurrentAbilitySlot);
        }
    }

    private void LoadInterfaceData()
    {
        interfaceInstance.LoadData(abilityController.AbilityTypeSlots);
    }

    private void OnAbilityOverriden(AbilityController.AbilityOverrideData abilityOverrideData)
    {
        interfaceInstance.RemoveAbilitySlot(abilityOverrideData.overridenAbility);
        interfaceInstance.AddAbilitySlot(abilityOverrideData.overrideAbility);
    }

    private void OnDestroy()
    {
        if (interfaceInstance == null) return;
        Destroy(interfaceInstance.gameObject);
    }
}
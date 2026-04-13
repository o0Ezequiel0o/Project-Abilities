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
        interfaceInstance = Instantiate(interfacePrefab, GameInstance.ScreenCanvas.transform);
    }

    private void SubscribeToEvents()
    {
        abilityController.onAbilityAdded += interfaceInstance.AddAbilitySlot;
        abilityController.onAbilityRemoved += interfaceInstance.RemoveAbilitySlot;
    }

    private void UpdateSkillsInterface()
    {
        for (int i = 0; i < abilityController.Abilities.Count; i++)
        {
            interfaceInstance.UpdateAbilitySlotRender(abilityController.Abilities[i]);
        }
    }

    private void LoadInterfaceData()
    {
        interfaceInstance.LoadData(abilityController.Abilities);
    }

    private void OnDestroy()
    {
        if (interfaceInstance == null) return;
        Destroy(interfaceInstance.gameObject);
    }
}
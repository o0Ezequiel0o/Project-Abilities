using UnityEngine;
using Zeke.Abilities;

[RequireComponent(typeof(StatusEffectHandler))]
public class AbilityControllerRenderer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AbilityControllerInterface interfacePrefab;

    private AbilityControllerInterface interfaceInstance;
    private AbilityController abilityController;

    void Awake()
    {
        abilityController = GetComponent<AbilityController>();
    }

    void Start()
    {
        SpawnInterfaceInCanvas();
        SubscribeToEvents();
        LoadInterfaceData();
    }

    void Update()
    {
        UpdateSkillsInterface();
    }

    void SpawnInterfaceInCanvas()
    {
        interfaceInstance = Instantiate(interfacePrefab, GameInstance.ScreenCanvas.transform);
    }

    void SubscribeToEvents()
    {
        abilityController.onAbilityAdded += interfaceInstance.AddAbilitySlot;
        abilityController.onAbilityRemoved += interfaceInstance.RemoveAbilitySlot;
    }

    void UpdateSkillsInterface()
    {
        for (int i = 0; i < abilityController.Abilities.Count; i++)
        {
            interfaceInstance.UpdateAbilitySlot(abilityController.Abilities[i]);
        }
    }

    void LoadInterfaceData()
    {
        interfaceInstance.LoadData(abilityController.Abilities);
    }

    void OnDestroy()
    {
        if (interfaceInstance == null) return;
        Destroy(interfaceInstance);
    }
}
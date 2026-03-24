using UnityEngine;

[RequireComponent(typeof(StatusEffectHandler))]
public class StatusEffectRenderer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private StatusEffectInterface interfacePrefab;

    private StatusEffectInterface interfaceInstance;
    private StatusEffectHandler statusEffectHandler;

    void Awake()
    {
        statusEffectHandler = GetComponent<StatusEffectHandler>();
    }

    void Start()
    {
        SpawnInterfaceInCanvas();
        SubscribeToEvents();
        LoadInterfaceData();
    }

    void SubscribeToEvents()
    {
        statusEffectHandler.onEffectApplied += interfaceInstance.AddStatusEffectSlot;
        statusEffectHandler.onEffectRemoved += interfaceInstance.RemoveStatusEffectSlot;

        statusEffectHandler.onStacksApplied += interfaceInstance.UpdateStatusEffectSlot;
        statusEffectHandler.onStacksRemoved += interfaceInstance.UpdateStatusEffectSlot;
    }

    void SpawnInterfaceInCanvas()
    {
        interfaceInstance = Instantiate(interfacePrefab, GameInstance.ScreenCanvas.transform);
    }

    void LoadInterfaceData()
    {
        interfaceInstance.LoadData(statusEffectHandler.StatusEffects);
    }

    void OnDestroy()
    {
        if (interfaceInstance == null) return;
        Destroy(interfaceInstance);
    }
}
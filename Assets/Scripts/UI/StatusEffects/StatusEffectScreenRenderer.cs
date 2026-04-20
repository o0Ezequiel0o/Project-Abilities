using UnityEngine;

[RequireComponent(typeof(StatusEffectHandler))]
public class StatusEffectScreenRenderer : MonoBehaviour
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
        statusEffectHandler.onEffectApplied.Subscribe(interfaceInstance.AddStatusEffectSlot);
        statusEffectHandler.onEffectRemoved.Subscribe(interfaceInstance.RemoveStatusEffectSlot);

        statusEffectHandler.onStacksApplied.Subscribe(interfaceInstance.UpdateStatusEffectSlot);
        statusEffectHandler.onStacksRemoved.Subscribe(interfaceInstance.UpdateStatusEffectSlot);
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
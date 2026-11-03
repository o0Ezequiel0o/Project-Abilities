using UnityEngine;

[RequireComponent(typeof(StatusEffectHandler))]
public class StatusEffectScreenRenderer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private StatusEffectInterface interfacePrefab;

    private StatusEffectInterface interfaceInstance;
    private StatusEffectHandler statusEffectHandler;

    private void Awake()
    {
        statusEffectHandler = GetComponent<StatusEffectHandler>();
    }

    private void Start()
    {
        SpawnInterfaceInCanvas();
        SubscribeToEvents();
        LoadInterfaceData();
    }

    private void SubscribeToEvents()
    {
        statusEffectHandler.onEffectApplied.Subscribe(interfaceInstance.AddStatusEffectSlot);
        statusEffectHandler.onEffectRemoved.Subscribe(interfaceInstance.RemoveStatusEffectSlot);

        statusEffectHandler.onStacksApplied.Subscribe(interfaceInstance.UpdateStatusEffectSlot);
        statusEffectHandler.onStacksRemoved.Subscribe(interfaceInstance.UpdateStatusEffectSlot);
    }

    private void SpawnInterfaceInCanvas()
    {
        interfaceInstance = Instantiate(interfacePrefab, GameInstance.ScreenCanvas.transform);
    }

    private void LoadInterfaceData()
    {
        interfaceInstance.LoadData(statusEffectHandler.StatusEffects);
    }

    private void OnDestroy()
    {
        if (interfaceInstance == null) return;
        Destroy(interfaceInstance);
    }
}
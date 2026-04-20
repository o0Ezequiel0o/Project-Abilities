using UnityEngine;

public class StatusEffectRenderer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private StatusEffectInterface interfacePrefab;
    [SerializeField] private Transform follow;
    [SerializeField] private Vector3 offset;

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

    private void LateUpdate()
    {
        interfaceInstance.transform.position = transform.position + offset;
    }

    private void SubscribeToEvents()
    {
        statusEffectHandler.onEffectApplied += interfaceInstance.AddStatusEffectSlot;
        statusEffectHandler.onEffectRemoved += interfaceInstance.RemoveStatusEffectSlot;

        statusEffectHandler.onStacksApplied += interfaceInstance.UpdateStatusEffectSlot;
        statusEffectHandler.onStacksRemoved += interfaceInstance.UpdateStatusEffectSlot;
    }

    private void SpawnInterfaceInCanvas()
    {
        interfaceInstance = Instantiate(interfacePrefab, GameInstance.WorldCanvas.transform);
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
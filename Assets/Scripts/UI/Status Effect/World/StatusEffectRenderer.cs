using UnityEngine;

[RequireComponent(typeof(StatusEffectHandler))]
public class StatusEffectRenderer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform follow;
    [SerializeField] private Vector3 offset;

    private StatusEffectInterface interfaceInstance;
    private StatusEffectHandler statusEffectHandler;

    private void Awake()
    {
        statusEffectHandler = GetComponent<StatusEffectHandler>();
    }

    private void OnEnable()
    {
        interfaceInstance = StatusEffectRendererManagerPool.Instance.Get();

        SubscribeToEvents();
        LoadInterfaceData();

        interfaceInstance.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (interfaceInstance != null && !StatusEffectRendererManagerPool.IsNull)
        {
            UnsubscribeFromEvents();
            StatusEffectRendererManagerPool.Instance.StaggeredDisableAndReturn(interfaceInstance);

            interfaceInstance = null;
        }
    }

    private void OnDestroy()
    {
        if (interfaceInstance != null && !StatusEffectRendererManagerPool.IsNull)
        {
            UnsubscribeFromEvents();
            StatusEffectRendererManagerPool.Instance.StaggeredDisableAndReturn(interfaceInstance);

            interfaceInstance = null;
        }
    }

    private void LateUpdate()
    {
        interfaceInstance.transform.position = transform.position + offset;
    }

    private void SubscribeToEvents()
    {
        statusEffectHandler.onEffectApplied.Subscribe(interfaceInstance.AddStatusEffectSlot);
        statusEffectHandler.onEffectRemoved.Subscribe(interfaceInstance.RemoveStatusEffectSlot);

        statusEffectHandler.onStacksApplied.Subscribe(interfaceInstance.UpdateStatusEffectSlot);
        statusEffectHandler.onStacksRemoved.Subscribe(interfaceInstance.UpdateStatusEffectSlot);
    }

    private void UnsubscribeFromEvents()
    {
        statusEffectHandler.onEffectApplied.Unsubscribe(interfaceInstance.AddStatusEffectSlot);
        statusEffectHandler.onEffectRemoved.Unsubscribe(interfaceInstance.RemoveStatusEffectSlot);

        statusEffectHandler.onStacksApplied.Unsubscribe(interfaceInstance.UpdateStatusEffectSlot);
        statusEffectHandler.onStacksRemoved.Unsubscribe(interfaceInstance.UpdateStatusEffectSlot);
    }

    private void LoadInterfaceData()
    {
        interfaceInstance.LoadData(statusEffectHandler.StatusEffects);
    }
}
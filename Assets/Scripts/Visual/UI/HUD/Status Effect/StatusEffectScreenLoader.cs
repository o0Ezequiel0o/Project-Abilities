using UnityEngine;

public class StatusEffectScreenLoader : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private StatusEffectInterface window;

    private StatusEffectHandler statusEffectHandler;

    private void Awake()
    {
        Run.Instance.onPlayerAdded += OnPlayerAdded;
    }

    private void OnPlayerAdded(Player newPlayer)
    {
        Run.Instance.onPlayerAdded -= OnPlayerAdded;

        bool playerHasRequiredComponents = newPlayer.TryGetComponent(out statusEffectHandler);

        if (!playerHasRequiredComponents) return;

        SubscribeToEvents();
        LoadInterfaceData();
    }

    private void SubscribeToEvents()
    {
        statusEffectHandler.onEffectApplied.Subscribe(window.AddStatusEffectSlot);
        statusEffectHandler.onEffectRemoved.Subscribe(window.RemoveStatusEffectSlot);

        statusEffectHandler.onStacksApplied.Subscribe(window.UpdateStatusEffectSlot);
        statusEffectHandler.onStacksRemoved.Subscribe(window.UpdateStatusEffectSlot);
    }

    private void LoadInterfaceData()
    {
        window.LoadData(statusEffectHandler.StatusEffects);
    }
}
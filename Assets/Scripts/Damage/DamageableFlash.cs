using UnityEngine;

public class DamageableFlash : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private SpriteFlashController flashController;

    [Header("Settings")]
    [SerializeField] private TriggerEvent triggerEvent;

    [Space]

    [SerializeField] private float flashAmount = 1f;
    [SerializeField] private float flashDuration = 0.25f;

    private void Reset()
    {
        flashController = GetComponentInChildren<SpriteFlashController>();
    }

    void Awake()
    {
        SubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        if (!TryGetComponent(out Damageable damageable)) return;

        switch (triggerEvent)
        {
            case TriggerEvent.Hit:
                damageable.onHitTaken += StartFlash;
                break;

            case TriggerEvent.Damage:
                damageable.onDamageTaken += StartFlash;
                break;
        }
    }

    void StartFlash(Damageable.DamageEvent _)
    {
        flashController.StartFlash(flashAmount, flashDuration);
    }

    private enum TriggerEvent
    {
        Hit,
        Damage
    }
}
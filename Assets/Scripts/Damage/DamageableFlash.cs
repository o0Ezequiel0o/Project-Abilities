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

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (!TryGetComponent(out Damageable damageable)) return;

        switch (triggerEvent)
        {
            case TriggerEvent.Hit:
                damageable.onHitTaken.Subscribe(StartFlash);
                break;

            case TriggerEvent.Damage:
                damageable.onTakenDamage.Subscribe(StartFlash);
                break;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (!TryGetComponent(out Damageable damageable)) return;

        damageable.onHitTaken.Unsubscribe(StartFlash);
        damageable.onTakenDamage.Unsubscribe(StartFlash);
    }

    private void StartFlash(Damageable.IDealtDamageEvent _) => StartFlash();
    private void StartFlash(Damageable.IHitDamageEvent _) => StartFlash();

    private void StartFlash()
    {
        {
            flashController.StartFlash(flashAmount, flashDuration);
        }
    }

    private enum TriggerEvent
    {
        Hit,
        Damage
    }
}
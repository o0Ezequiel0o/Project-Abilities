using UnityEngine;

public class DamageableNumbers : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float displaySize;

    [Space]

    [SerializeField] private float yStart = 0.5f;
    [SerializeField] private float yLowest = -0.5f;
    [SerializeField] private float yLower = 0.3f;

    private float currentYoffset = 0f;

    private void Reset()
    {
        displaySize = 1f;
        yStart = 0.5f;
        yLowest = -0.5f;
        yLower = 0.3f;
    }

    private void Awake()
    {
        SubscribeToEvents();
        currentYoffset = yStart;
    }

    private void Update()
    {
        UpdateCurrentOffset(currentYoffset + (DamageNumbersManager.Config.FloatSpeed * Time.deltaTime));
    }

    private void SubscribeToEvents()
    {
        if (TryGetComponent(out Damageable damageable))
        {
            damageable.onTakenDamage += DisplayDamageNumber;
        }
    }

    private void DisplayDamageNumber(Damageable.DamageEvent damageEvent)
    {
        Vector2 spawnOffset = new Vector2(0f, currentYoffset);

        DamageNumbersManager.DisplayDamageNumber(transform.position, gameObject, damageEvent.UncappedDamageDealt, displaySize, spawnOffset);
        UpdateCurrentOffset(currentYoffset - yLower);
    }

    private void UpdateCurrentOffset(float newOffsetValue)
    {
        currentYoffset = Mathf.Clamp(newOffsetValue, yLowest, yStart);
    }
}
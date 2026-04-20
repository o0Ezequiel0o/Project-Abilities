using UnityEngine;

public class HealthBarRenderer : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private Damageable damageable;

    [Header("Settings")]
    [SerializeField] private HealthBar healthBarPrefab;
    [SerializeField] private Transform center;
    [Space]
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Vector2 size = Vector2.one;
    [SerializeField] private float duration = 5f;

    private HealthBar healthBar;
    private float durationTimer;

    private void Reset()
    {
        damageable = GetComponentInChildren<Damageable>();
        center = GetComponentInChildren<Transform>();
    }

    private void Start()
    {
        if (damageable == null) return;

        SpawnHealthBar();
        SubscribeToEvents();
        ResetHealthBarDurationTimer();
    }

    private void LateUpdate()
    {
        if (healthBar == null) return;

        HealthBarFollowTargetPosition();
        UpdateHealthBarDurationTimer();
    }

    private void OnDestroy()
    {
        if (healthBar == null) return;
        Destroy(healthBar.gameObject);
    }

    private void SpawnHealthBar()
    {
        Canvas worldCanvas = GameInstance.WorldCanvas;

        if (worldCanvas == null) return;

        healthBar = Instantiate(healthBarPrefab, worldCanvas.transform).GetComponent<HealthBar>();

        if (healthBar.TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.sizeDelta *= size;
        }

        HideHealthBar();
    }

    private void SubscribeToEvents()
    {
        damageable.onTakenDamage += OnDamageTaken;
        damageable.onAnyHealthUpdate += UpdateHealthBar;
    }

    private void OnDamageTaken(Damageable.DamageEvent damageEvent)
    {
        if (healthBar == null) return;

        if (!healthBar.gameObject.activeSelf)
        {
            ShowHealthBar();
        }

        ResetHealthBarDurationTimer();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        float healthRatio = damageable.Health / damageable.MaxHealth.Value;
        float shieldRatio = damageable.Shield / damageable.MaxShield.Value;

        healthBar.UpdateBar(healthRatio, shieldRatio);
    }

    private void HealthBarFollowTargetPosition()
    {
        healthBar.transform.position = (center.position + offset);
    }

    private void UpdateHealthBarDurationTimer()
    {
        durationTimer += Time.deltaTime;

        if (durationTimer >= duration)
        {
            HideHealthBar();
        }
    }

    private void ResetHealthBarDurationTimer()
    {
        durationTimer = 0f;
    }

    private void ShowHealthBar()
    {
        healthBar.gameObject.SetActive(true);
        ResetHealthBarDurationTimer();
    }

    private void HideHealthBar()
    {
        healthBar.gameObject.SetActive(false);
        ResetHealthBarDurationTimer();
    }
}